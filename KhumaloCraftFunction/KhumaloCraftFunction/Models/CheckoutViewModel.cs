using System.ComponentModel.DataAnnotations;

namespace CLDV6211_ST10287165_POE_P1.Models
{
    public class CheckoutViewModel
    {
        [Key]
        public int CheckoutViewModelKey { get; set; }
        public List<CartItem> CartItems { get; set; }
        public Customer Customer { get; set; }
        public string ShippingAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string? CreditCardNumber { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }
        public string PaymentMethod { get; set; }  // Add payment method selection
    }
}
