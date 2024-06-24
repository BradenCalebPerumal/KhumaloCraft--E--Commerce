using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLDV6211_ST10287165_POE_P1.Data;
using CLDV6211_ST10287165_POE_P1.Models;
using Microsoft.Extensions.Logging; // Ensure this namespace is included for logging

namespace CLDV6211_ST10287165_POE_P1.Controllers
{
    public class CustomersController : Controller
    {
        private readonly CLDV6211_ST10287165_POE_P1Context _context;
        private readonly ILogger<CustomersController> _logger;
        public CustomersController(CLDV6211_ST10287165_POE_P1Context context, ILogger<CustomersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customer.ToListAsync());
        }
        public async Task<IActionResult> ShoppingCart()
        {
            return View(await _context.Customer.ToListAsync());
        }
        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
        public async Task<IActionResult> Details1()
        {
            int? customerId = HttpContext.Session.GetInt32("CustId");
            if (customerId == null)
            {
                return RedirectToAction("Login", "Customers");
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustId == customerId);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }


        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustId,CustEmail,CustPasswordHash")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit1()
        {
            int? customerId = HttpContext.Session.GetInt32("CustId");
            if (customerId == null)
            {
                return RedirectToAction("Login", "Customers");
            }

            var customer = await _context.Customer.FindAsync(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustId,CustEmail,CustPasswordHash")] Customer customer)
        {
            if (id != customer.CustId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit1( Customer customer)
        {
            int? customerId = HttpContext.Session.GetInt32("CustId");
            if (customerId == null)
            {
                return RedirectToAction("Login", "Customers");
            }

            if (customerId != customer.CustId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customer.FindAsync(id);
            if (customer != null)
            {
                _context.Customer.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customer.Any(e => e.CustId == id);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Customer.FirstOrDefaultAsync(u => u.CustEmail == email && u.CustPasswordHash == password);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid login attempt";
                return View();
            }
            var existingCartItems = _context.CartItems.Where(c => c.CustId == user.CustId).ToList();
            _context.CartItems.RemoveRange(existingCartItems);
            _context.SaveChanges();


            // Set a session flag here
            HttpContext.Session.SetString("IsLoggedIn", "true");
            HttpContext.Session.SetInt32("CustId", user.CustId);  // Storing customer ID in the session
            HttpContext.Session.SetInt32("CartCount", 0);
            return RedirectToAction("Index", "Products");
        }


        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(string email, string password)
        {
            var userExists = await _context.Customer.AnyAsync(u => u.CustEmail == email);
            if (userExists)
            {
                ViewBag.ErrorMessage = "User already exists.";
                return View();
            }

            // Here, you should hash the password before saving it to the database.
            var customer = new Customer { CustEmail = email, CustPasswordHash = password };
            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();

            // Optionally, sign the user in automatically after registration
            // Redirect to the login page for now
            return RedirectToAction("Login");
        }
        public IActionResult Logoutt()
        {
            int custId = HttpContext.Session.GetInt32("CustId").Value; // Ensure you have this value stored
            var cartItems = _context.CartItems.Where(c => c.CustId == custId).ToList();

            foreach (var item in cartItems)
            {
                var product = _context.Product.Find(item.ProductId);
                if (product != null)
                {
                    product.Quantity += item.Quantity; // Return the quantity to the inventory
                }
            }
            _context.SaveChanges();

            HttpContext.Session.Clear(); // Clears the session
            return RedirectToAction("Index", "Home"); // Redirect to home page or login page
        }



      


    

    }

}
