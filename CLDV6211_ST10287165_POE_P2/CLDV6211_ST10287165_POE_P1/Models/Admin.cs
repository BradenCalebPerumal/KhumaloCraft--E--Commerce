using System.ComponentModel.DataAnnotations;

namespace CLDV6211_ST10287165_POE_P1.Models
{
    public class Admin
    {
        [Key]
        public int AdminID { get; set; }
        public string AdminEmail { get; set; }
        public string AdminPasswordHash { get; set; }
    }
}