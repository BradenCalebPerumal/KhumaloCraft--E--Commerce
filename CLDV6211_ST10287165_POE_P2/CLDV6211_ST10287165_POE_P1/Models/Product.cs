using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV6211_ST10287165_POE_P1.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        [Required(ErrorMessage = "Please select an image type.")]
        public string? ImageType { get; set; }

        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile? ImageUpload { get; set; }

        public int ClientId { get; set; }

        // Navigation property for the client who added this product
        public Client? Client { get; set; }

        [Required]
        public int Quantity { get; set; } // New property to track stock level
    }
}
