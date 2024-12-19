namespace Ecommerce.Models.DTOs
{
    public class CategoryResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProductDTO> Products { get; set; }

        public static CategoryResponseDTO FromCategory(Category category)
        {
            return new CategoryResponseDTO
            {
                Id = category.Id,
                Name = category.Name,
                Products = category.Products?.Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description
                }).ToList() ?? new List<ProductDTO>()
            };
        }
    }
}
