using System.Threading.Tasks;

namespace Bookstore.Domain.Carts
{
    public interface IShoppingCartService
    {
        Task<ShoppingCart> GetShoppingCartAsync(string correlationId);

        Task AddToShoppingCartAsync(AddToShoppingCartDto addToShoppingCartDto);

        Task DeleteShoppingCartItemAsync(DeleteShoppingCartItemDto deleteShoppingCartItemDto);
    }

    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository shoppingCartRepository;

        public ShoppingCartService(IShoppingCartRepository shoppingCartRepository)
        {
            this.shoppingCartRepository = shoppingCartRepository;
        }

        public async Task<ShoppingCart> GetShoppingCartAsync(string shoppingCartCorrelationId)
        {
            return await shoppingCartRepository.GetAsync(shoppingCartCorrelationId);
        }

        public async Task AddToShoppingCartAsync(AddToShoppingCartDto dto)
        {
            await AddToShoppingCartAsync(dto.CorrelationId, dto.BookId, dto.Quantity, true);
        }

        private async Task AddToShoppingCartAsync(string correlationId, int bookId, int quantity, bool wantToBuy)
        {
            var shoppingCart = await shoppingCartRepository.GetAsync(correlationId);

            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart(correlationId);

                await shoppingCartRepository.AddAsync(shoppingCart);
            }

            if (wantToBuy)
            {
                shoppingCart.AddItemToShoppingCart(bookId, quantity);
            }
            else
            {
                shoppingCart.AddItemToWishlist(bookId);
            }

            await shoppingCartRepository.SaveChangesAsync();
        }

        public async Task DeleteShoppingCartItemAsync(DeleteShoppingCartItemDto dto)
        {
            var shoppingCart = await shoppingCartRepository.GetAsync(dto.CorrelationId);

            shoppingCart.RemoveShoppingCartItemById(dto.ShoppingCartItemId);

            await shoppingCartRepository.SaveChangesAsync();
        }
    }
}