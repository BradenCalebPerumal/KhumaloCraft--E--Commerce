using System.ComponentModel.DataAnnotations;

namespace CLDV6211_ST10287165_POE_P1.Models
{
    public class Customer
    {
        [Key]
        public int CustId { get; set; }
        public string CustEmail { get; set; }
        public string CustPasswordHash { get; set; }
    }
}
