using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CLDV6211_ST10287165_POE_P1.Models;

namespace ST10287165_POE_FUNCTIONS.Models
{
    public class CLDV6211_ST10287165_POE_P1Context : DbContext
    {
        public CLDV6211_ST10287165_POE_P1Context(DbContextOptions<CLDV6211_ST10287165_POE_P1Context> options)
            : base(options)
        {
        }

        public DbSet<Product> Product { get; set; } = default!;
        public DbSet<User> User { get; set; } = default!;
        public DbSet<Customer> Customer { get; set; } = default!;



        public DbSet<Client> Client { get; set; } = default!;
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<CheckoutViewModel> CheckoutViewModel { get; set; }
        public DbSet<Admin> Admins { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
    }

}
