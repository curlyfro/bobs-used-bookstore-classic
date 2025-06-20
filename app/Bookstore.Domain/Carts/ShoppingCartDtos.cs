namespace Bookstore.Domain.Carts
{
    public class AddToShoppingCartDto
    {
        public AddToShoppingCartDto(string correlationId, int bookId, int quantity)
        {
            this.CorrelationId = correlationId;
            this.BookId = bookId;
            this.Quantity = quantity;
        }

        public string CorrelationId { get; }
        public int BookId { get; }
        public int Quantity { get; }
    }

    public class DeleteShoppingCartItemDto
    {
        public DeleteShoppingCartItemDto(string correlationId, int shoppingCartItemId)
        {
            this.CorrelationId = correlationId;
            this.ShoppingCartItemId = shoppingCartItemId;
        }

        public string CorrelationId { get; }
        public int ShoppingCartItemId { get; }
    }
}
