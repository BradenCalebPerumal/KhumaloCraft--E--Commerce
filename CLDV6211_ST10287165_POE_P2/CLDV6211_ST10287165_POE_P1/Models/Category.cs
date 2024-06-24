using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV6211_ST10287165_POE_P1.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        // Remove NotMapped if you want to keep the relationship but make it optional
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}