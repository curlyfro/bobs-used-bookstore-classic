﻿using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.Owin;
using Bookstore.Data;
using Bookstore.Domain.Customers;
using Bookstore.Web.Helpers;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

namespace Bookstore.Web
{
    public static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IAppBuilder app)
        {
            if (BookstoreConfiguration.GetSetting("Services/Authentication") == "aws")
            {
                ConfigureCognitoAuthentication(app);
            }
            else
            {
                ConfigureLocalAuthentication(app);
            }
        }

        private static void ConfigureLocalAuthentication(IAppBuilder app)
        {
            app.UseMiddlewareFromContainer<LocalAuthenticationMiddleware>();
        }

        private static void ConfigureCognitoAuthentication(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = BookstoreConfiguration.GetSetting("Authentication/Cognito/LocalClientId"),
                MetadataAddress = BookstoreConfiguration.GetSetting("Authentication/Cognito/MetadataAddress"),
                ResponseType = OpenIdConnectResponseType.Code,
                RedeemCode = true,
                Scope = "openid profile",
                SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                UseTokenLifetime = false,
                SaveTokens = true,
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "cognito:username",
                    RoleClaimType = "cognito:groups"
                },
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    RedirectToIdentityProvider = x =>
                    {
                        x.Options.RedirectUri = x.Request.GetReturnUrl();
                        x.ProtocolMessage.RedirectUri = x.Request.GetReturnUrl();

                        return Task.CompletedTask;
                    },
                    AuthorizationCodeReceived = x =>
                    {
                        x.RedirectUri = x.Request.GetReturnUrl();
                        x.TokenEndpointRequest.RedirectUri = x.Request.GetReturnUrl();

                        return Task.CompletedTask;
                    },
                    SecurityTokenValidated = async x =>
                    {
                        var scope = x.OwinContext.GetAutofacLifetimeScope();
                        var service = scope.Resolve<ICustomerService>();

                        x.Request.User = new ClaimsPrincipal(x.AuthenticationTicket.Identity);

                        var identity = (ClaimsIdentity)x.Request.User.Identity;

                        var dto = new CreateOrUpdateCustomerDto(
                            identity.GetSub(),
                            identity.Name,
                            identity.FindFirst(y => y.Type.Contains("givenname")).Value,
                            identity.FindFirst(y => y.Type.Contains("surname")).Value);

                        await service.CreateOrUpdateCustomerAsync(dto);
                    }
                }
            });
        }
    }
}