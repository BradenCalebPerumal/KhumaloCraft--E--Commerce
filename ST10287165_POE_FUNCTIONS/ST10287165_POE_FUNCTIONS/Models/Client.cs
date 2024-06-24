using System.ComponentModel.DataAnnotations;

namespace CLDV6211_ST10287165_POE_P1.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        // Nullable ClientFirstName
        public string? ClientFirstName { get; set; }

        public string Email { get; set; }

        public string IdentityNum { get; set; }

        public string LastName { get; set; }

        public string CellNum { get; set; }


        // Navigation property for the products this client added
        public ICollection<Product>?Products  { get; set; }
    }
}
