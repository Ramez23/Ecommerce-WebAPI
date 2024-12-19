namespace Ecommerce.Models
{
    public enum PaymentMethods
    {
        cash,
        creditCard
    }

    public class Order
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; }
        public AppUser User { get; set; }   
        public ICollection<Product> Products { get; set; }
        public PaymentMethods Method { get; set; }
    }
}
