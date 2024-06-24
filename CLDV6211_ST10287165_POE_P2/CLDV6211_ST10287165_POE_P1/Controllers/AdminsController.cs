using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLDV6211_ST10287165_POE_P1.Data;
using CLDV6211_ST10287165_POE_P1.Models;

namespace CLDV6211_ST10287165_POE_P1.Controllers
{
    public class AdminsController : Controller
    {
        private readonly CLDV6211_ST10287165_POE_P1Context _context;

        public AdminsController(CLDV6211_ST10287165_POE_P1Context context)
        {
            _context = context;
        }

        // GET: Admins
        public async Task<IActionResult> Index()
        {
            var adminID = HttpContext.Session.GetInt32("AdminID");
            if (adminID == null)
            {
                return RedirectToAction("AdminLogin", "Admins"); // Redirect to login if not logged in
            }

            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.AdminID == adminID);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.AdminID == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminID,AdminEmail,AdminPasswordHash")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminID,AdminEmail,AdminPasswordHash")] Admin admin)
        {
            if (id != admin.AdminID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.AdminID))
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
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.AdminID == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Check if the user is authenticated using session ID
            if (HttpContext.Session.GetString("IsAdminLoggedIn") == "true")
            {
                var admin = await _context.Admins.FindAsync(id);

                // Check if the admin exists
                if (admin != null)
                {
                    _context.Admins.Remove(admin);

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Set IsAdminLoggedIn session variable to false
                    HttpContext.Session.SetString("IsAdminLoggedIn", "false");
                }
            }

            // Redirect the user to the homepage
            return RedirectToAction("Index", "Home");
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.AdminID == id);
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            bool showSignupLink = !_context.Admins.Any(); // Check if there are any admins in the database
            ViewBag.ShowSignupLink = showSignupLink;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin(string email, string password)
        {
            var user = await _context.Admins.FirstOrDefaultAsync(u => u.AdminEmail == email && u.AdminPasswordHash == password);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid login attempt";
                return View();
            }

            _context.SaveChanges();


            // Set a session flag here
            HttpContext.Session.SetString("IsAdminLoggedIn", "true");
            HttpContext.Session.SetInt32("AdminID", user.AdminID);  // Storing customer ID in the session

            return RedirectToAction("AdminDashboard", "Admins");
        }


        [HttpGet]
        public IActionResult AdminSignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminSignUp(string email, string password)
        {
            var userExists = await _context.Admins.AnyAsync(u => u.AdminEmail == email);
            if (userExists)
            {
                ViewBag.ErrorMessage = "User already exists.";
                return View();
            }

            // Here, you should hash the password before saving it to the database.
            var admin = new Admin { AdminEmail = email, AdminPasswordHash = password };
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            // Optionally, sign the user in automatically after registration
            // Redirect to the login page for now

            return RedirectToAction("AdminLogin");
        }
        public IActionResult Logoutt()
        {
            // If you want to clear the cart items from the database as well
            var custId = HttpContext.Session.GetInt32("AdminID");
            if (custId.HasValue)
            {

                _context.SaveChanges();
            }

            // Clear all session data
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
        /*public async Task<IActionResult> Profile()
        {
            var adminId = HttpContext.Session.GetInt32("AdminID");
            if (!adminId.HasValue)
            {
                // Admin ID not found in session, redirect to login or handle accordingly
                return RedirectToAction("AdminLogin");
            }

            var admin = await _context.Admins.FindAsync(adminId.Value);
            if (admin == null)
            {
                // Admin not found in the database, handle accordingly
                return NotFound();
            }

            return View(admin);
        }*/

        // GET: Admin/Edit
        // GET: Admin/Profile
        // GET: Admins/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var adminId = HttpContext.Session.GetInt32("AdminID");
            if (!adminId.HasValue)
            {
                return RedirectToAction("AdminLogin", "Admins");
            }

            var admin = await _context.Admins.FindAsync(adminId.Value);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(int id, [Bind("AdminID,AdminEmail,AdminPasswordHash")] Admin admin)
        {
            if (id != admin.AdminID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.AdminID))
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
            return View(admin);
        }
        /*
                private bool AdminExists(int id)
                {
                    return _context.Admins.Any(e => e.AdminID == id);
                }*/

        [HttpGet]
        public IActionResult NewAdmin()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> NewAdmin(string email, string password)
        {
            var userExists = await _context.Admins.AnyAsync(u => u.AdminEmail == email);
            if (userExists)
            {
                ViewBag.ErrorMessage = "User already exists.";
                return View();
            }

            // Here, you should hash the password before saving it to the database.
            var admin = new Admin { AdminEmail = email, AdminPasswordHash = password };
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            // Optionally, sign the user in automatically after registration
            // Redirect to the login page for now
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AdminDashboard()
        {
            var orderCount = await _context.Orders.CountAsync();
            var customerCount = await _context.Customer.CountAsync();

            ViewBag.OrderCount = orderCount;
            ViewBag.CustomerCount = customerCount;

            return View();






        }
    }
}