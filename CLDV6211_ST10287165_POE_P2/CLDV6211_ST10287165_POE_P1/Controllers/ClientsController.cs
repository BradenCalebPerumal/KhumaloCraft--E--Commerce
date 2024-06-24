using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLDV6211_ST10287165_POE_P1.Data;
using CLDV6211_ST10287165_POE_P1.Models;
using System.Security.Claims;

namespace CLDV6211_ST10287165_POE_P1.Controllers
{
    public class ClientsController : Controller
    {
        private readonly CLDV6211_ST10287165_POE_P1Context _context;

        public ClientsController(CLDV6211_ST10287165_POE_P1Context context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            return View(await _context.Client.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,Username,Password")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        [HttpGet]
        public IActionResult Editp()
        {
            int? clientId = HttpContext.Session.GetInt32("ClientId");
            if (clientId == null)
            {
                return RedirectToAction("Login", "Clients");
            }

            var client = _context.Client.Find(clientId);
            if (client == null)
            {
                return NotFound();
            }

            // Count the number of products for the client
            var clientProductsCount = _context.Product
                .Where(p => p.ClientId == clientId)
                .Count();

            // Pass the count to the view
            ViewBag.ProductsCount = clientProductsCount;

            return View(client);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editp(Client client)
        {
            int? clientId = HttpContext.Session.GetInt32("ClientId");

            // Pass the count to the view
            Console.WriteLine("Editp POST method called.");
            if (ModelState.IsValid)
            {
                try
                {
                    var existingClient = _context.Client.Find(client.ClientId);
                    if (existingClient == null)
                    {
                        return NotFound();
                    }

                    // Copy updated properties
                    existingClient.Username = client.Username;
                    existingClient.Password = client.Password;
                    existingClient.ClientFirstName = client.ClientFirstName;
                    existingClient.LastName = client.LastName;
                    existingClient.Email = client.Email;
                    existingClient.CellNum = client.CellNum;
                    existingClient.IdentityNum = client.IdentityNum;

                    _context.Update(existingClient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Dashboard));
            }

            Console.WriteLine("Model state is invalid. Errors:");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            // Count the number of products for the client
            var clientProductsCount = _context.Product
                .Where(p => p.ClientId == client.ClientId)
                .Count();
            ViewBag.ProductsCount = clientProductsCount;

            return View(client);
        }
        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Client.FindAsync(id);
            if (client != null)
            {
                _context.Client.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.ClientId == id);
        }
        [HttpGet]
        public IActionResult Loginn()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SIgnUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SIgnUp(Client model)
        {
            var userExists = await _context.Client.AnyAsync(u => u.Username == model.Username);
            if (userExists)
            {
                ViewBag.ErrorMessage = "Client already exists.";
                return View();
            }

            // Here, you should hash the password before saving it to the database.
            //  model.Password = HashPassword(model.Password); // Ensure you implement HashPassword method.

            _context.Client.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Loginn"); // Make sure this is the correct action name.
        }
    
    [HttpPost]
    public async Task<IActionResult> Loginn(string email, string password)
    {
        var client = await _context.Client.FirstOrDefaultAsync(u => u.Username == email && u.Password == password);

        if (client == null)
        {
            ViewBag.ErrorMessage = "Invalid login attempt";
            return View();
        }

            // Set a session flag here
            HttpContext.Session.SetInt32("ClientId", client.ClientId);
         if (!string.IsNullOrEmpty(client.ClientFirstName))
        {
            HttpContext.Session.SetString("ClientFirstName", client.ClientFirstName);
        }
        else
        {
            HttpContext.Session.SetString("ClientFirstName", string.Empty); // Or handle it as you see fit
        }
            HttpContext.Session.SetString("ClientLastName", client.LastName);
            HttpContext.Session.SetString("IsClientLoggedIn", "true");
        HttpContext.Session.SetInt32("ClientId", client.ClientId);
        return RedirectToAction("Dashboard", "Clients");
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); // Clears all session data
        return RedirectToAction("Index", "Home"); // Redirect to home page
    }
        // GET: /Home/Dashboard
        public IActionResult Dashboard()
        {
            // Retrieve the user's ID from session
            var userId = HttpContext.Session.GetInt32("ClientId");

            if (userId.HasValue)
            {
                // Find the user in the database by ID
                var client = _context.Client.Find(userId.Value);

                if (client != null)
                {
                    // Get the user's full name
                    var fullName = $"{client.ClientFirstName} {client.LastName}";
                    ViewBag.FullName = fullName;
                }
                else
                {
                    // Handle case where user is not found in the database
                    ViewBag.FullName = "Unknown";
                }
            }
            else
            {
                // Handle case where user ID is not stored in session
                ViewBag.FullName = "Unknown";
            }

            return View();
        }

        public IActionResult ListedProducts()
        {
            // Check if the session flag indicates that the user is logged in
            var isClientLoggedIn = HttpContext.Session.GetString("IsClientLoggedIn");
            if (isClientLoggedIn != "true")
            {
                // If not logged in, redirect to the login page
                return RedirectToAction("Login", "Clients");
            }

            // Retrieve the client ID from the session
            var clientId = HttpContext.Session.GetInt32("ClientId");

            // Proceed with retrieving client's products using clientId
            var clientProducts = _context.Product
                .Where(p => p.ClientId == clientId)
                .Include(p => p.Category)
                .ToList();

            if (!clientProducts.Any())
            {
                // If no products listed, display a message
                ViewBag.Message = "No products listed.";
            }
         
            return View(clientProducts);
        }


        // Display products with edit and delete options
        public IActionResult EditProducts()
        {
            var isClientLoggedIn = HttpContext.Session.GetString("IsClientLoggedIn");
            if (isClientLoggedIn != "true")
            {
                return RedirectToAction("Login", "Clients");
            }

            var clientId = HttpContext.Session.GetInt32("ClientId");
            var clientProducts = _context.Product.Where(p => p.ClientId == clientId).ToList();

            return View(clientProducts);
        }
        // Render the edit form for a specific product
        public IActionResult EditProduct(int id)
        {
            var product = _context.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        // Handle the update of product information
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Update(product);
                _context.SaveChanges();
                return RedirectToAction("EditProducts");
            }

            return View(product);
        }

        // Handle the deletion of a product
        [HttpPost]
        public IActionResult DeleteProduct(int id, string confirmation)
        {
            if (confirmation == "delete")
            {
                var product = _context.Product.Find(id);
                if (product != null)
                {
                    _context.Product.Remove(product);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("EditDisplay","Products");
        }






    }
}

