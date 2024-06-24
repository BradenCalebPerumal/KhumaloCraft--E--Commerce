using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV6211_ST10287165_POE_P1.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("Customer")]
        public int CustId { get; set; }
        public Customer Customer { get; set; }
    }
}