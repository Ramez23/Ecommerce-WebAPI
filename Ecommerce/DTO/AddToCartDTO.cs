namespace Ecommerce.DTO
{
    public class AddToCartDTO
    {
        public string UserId { get; set; }  
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
