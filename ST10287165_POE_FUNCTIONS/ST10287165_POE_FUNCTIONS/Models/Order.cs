using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV6211_ST10287165_POE_P1.Models
{
    public class Order
    {


        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }
        [Key]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }  // "Pending", "Processed"

        [ForeignKey("Customer")]
        public int CustId { get; set; }
        public virtual Customer Customer { get; set; }

        public string ShippingAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public string PaymentMethod { get; set; }  // Added this field

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}