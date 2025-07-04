﻿using System;
using System.Web;
using System.Web.Mvc;
using Bookstore.Data;

namespace Bookstore.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        public ActionResult Login(string redirectUri = null)
        {
            if(string.IsNullOrWhiteSpace(redirectUri)) return RedirectToAction("Index", "Home");

            return Redirect(redirectUri);
        }

        public ActionResult LogOut()
        {
            return BookstoreConfiguration.GetSetting("Services/Authentication") == "aws" ? CognitoSignOut() : LocalSignOut();
        }

        private ActionResult LocalSignOut()
        {
            if (HttpContext.Request.Cookies["LocalAuthentication"] != null)
            {
                HttpContext.Response.Cookies.Add(new HttpCookie("LocalAuthentication") { Expires = DateTime.Now.AddDays(-1) });
            }

            return RedirectToAction("Index", "Home");
        }

        private ActionResult CognitoSignOut()
        {
            if (Request.Cookies[".AspNet.Cookies"] != null)
            {
                Response.Cookies.Add(new HttpCookie(".AspNet.Cookies") { Expires = DateTime.Now.AddDays(-1) });
            }

            var domain = BookstoreConfiguration.GetSetting("Authentication/Cognito/CognitoDomain");
            var clientId = BookstoreConfiguration.GetSetting("Authentication/Cognito/LocalClientId");
            var logoutUri = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}/";

            return Redirect($"{domain}/logout?client_id={clientId}&logout_uri={logoutUri}");
        }
    }
}
