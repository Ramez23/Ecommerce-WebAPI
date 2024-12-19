namespace Ecommerce.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }    
        public ICollection<CartProduct> CartProducts { get; set; }
    }
}
