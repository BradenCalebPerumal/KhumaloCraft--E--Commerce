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
using Microsoft.IdentityModel.Tokens;

namespace CLDV6211_ST10287165_POE_P1.Controllers
{
    public class CartItemsController : Controller
    {
        private readonly CLDV6211_ST10287165_POE_P1Context _context;

        public CartItemsController(CLDV6211_ST10287165_POE_P1Context context)
        {
            _context = context;
        }

        // GET: CartItems
        public async Task<IActionResult> Index()
        {
            var cLDV6211_ST10287165_POE_P1Context = _context.CartItems.Include(c => c.Customer).Include(c => c.Product);
            return View(await cLDV6211_ST10287165_POE_P1Context.ToListAsync());
        }

        // GET: CartItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .Include(c => c.Customer)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CartItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: CartItems/Create
        public IActionResult Create()
        {
            ViewData["CustId"] = new SelectList(_context.Customer, "CustId", "CustId");
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ImageType");
            return View();
        }

        // POST: CartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartItemId,ProductId,Quantity,CustId")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustId"] = new SelectList(_context.Customer, "CustId", "CustId", cartItem.CustId);
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ImageType", cartItem.ProductId);
            return View(cartItem);
        }

        // GET: CartItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            ViewData["CustId"] = new SelectList(_context.Customer, "CustId", "CustId", cartItem.CustId);
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ImageType", cartItem.ProductId);
            return View(cartItem);
        }

        // POST: CartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartItemId,ProductId,Quantity,CustId")] CartItem cartItem)
        {
            if (id != cartItem.CartItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemExists(cartItem.CartItemId))
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
            ViewData["CustId"] = new SelectList(_context.Customer, "CustId", "CustId", cartItem.CustId);
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "ImageType", cartItem.ProductId);
            return View(cartItem);
        }
        // GET: CartItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .Include(c => c.Customer)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CartItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }


        private bool CartItemExists(int id)
        {
            return _context.CartItems.Any(e => e.CartItemId == id);
        }


        public IActionResult AddToCart(int productId, int quantity)
        {
            if (!HttpContext.Session.GetInt32("CustId").HasValue)
            {
                TempData["Error"] = "Please log in to add items to your cart.";
                return RedirectToAction("Login", "Customers");
            }

            int custId = HttpContext.Session.GetInt32("CustId").Value;
            var product = _context.Product.Find(productId);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Index", "Products");
            }

            if (product.Quantity < quantity)
            {
                TempData["Error"] = "Not enough stock available.";
                return RedirectToAction("Index", "Products");
            }

            product.Quantity -= quantity; // Decrease product quantity

            var cartItem = _context.CartItems.FirstOrDefault(c => c.ProductId == productId && c.CustId == custId);
            if (cartItem == null)
            {
                _context.CartItems.Add(new CartItem { ProductId = productId, Quantity = quantity, CustId = custId });
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            _context.SaveChanges();

            UpdateCartCount(custId);

            return RedirectToAction("CartView");
        }
        private void UpdateCartCount(int custId)
        {
            var newCartCount = _context.CartItems.Where(c => c.CustId == custId).Sum(c => c.Quantity);
            HttpContext.Session.SetInt32("CartCount", newCartCount);
        }
        public IActionResult GetCartCount()
        {
            int cartCount = HttpContext.Session.GetInt32("CartCount") ?? 0;
            return Json(new { cartCount = cartCount });
        }
        public IActionResult CartView()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                TempData["Error"] = "Please log in to view your cart.";
                return RedirectToAction("Login", "Customers");
            }

            var custId = HttpContext.Session.GetInt32("CustId").Value;  // Assumes CustId is always set if IsLoggedIn is true
            var cartItems = _context.CartItems.Include(c => c.Product).Where(c => c.CustId == custId).ToList();

            return View(cartItems);  // Return the view with the cart items
        }
        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();

                // Update cart count in session
                UpdateCartCountInSession();
            }

            return RedirectToAction("CartView");  // Redirects to the cart view to show updated cart
        }
       
  

        public class CartItemUpdateModel
        {
            public int CartItemId { get; set; }
            public int Quantity { get; set; }
        }
        



/*
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var customerId = HttpContext.Session.GetInt32("CustId");
            if (customerId == null)
            {
                TempData["Error"] = "User not logged in";
                return RedirectToAction("CartView");
            }

            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                TempData["Error"] = "Cart item not found";
                return RedirectToAction("CartView");
            }

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            // Update the cart count in the session
            UpdateCartCountInSession();

            return RedirectToAction("CartView");
        }
*/
        private void UpdateCartCountInSession()
        {
            try
            {
                // Check if the customer ID is available in the session.
                if (HttpContext.Session.GetInt32("CustId") is int custId)
                {
                    // Retrieve all cart items for the logged-in customer and calculate the sum of the quantities.
                    var cartCount = _context.CartItems.Where(c => c.CustId == custId).Sum(c => c.Quantity);

                    // Update the session with the new cart count.
                    HttpContext.Session.SetInt32("CartCount", cartCount);
                }
                else
                {
                    // If there is no customer ID in the session, remove the cart count from the session.
                    HttpContext.Session.Remove("CartCount");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it according to your logging strategy.
                // You might want to consider how critical this method is for your application's functionality
                // and handle the exception appropriately (e.g., by setting the session variable to 0 or sending an error response).
                Console.WriteLine("Error updating cart count: " + ex.Message);
                // Optionally reset the cart count in the session to 0 to avoid showing stale data
                HttpContext.Session.SetInt32("CartCount", 0);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            Console.WriteLine($"Received request to update cart item {cartItemId} to quantity {quantity}.");

            var cartItem = await _context.CartItems.Include(ci => ci.Product).FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);
            if (cartItem == null)
            {
                Console.WriteLine("Cart item not found.");
                return Json(new { success = false, message = "Cart item not found." });
            }

            Console.WriteLine($"Original cart item quantity: {cartItem.Quantity}, Product stock: {cartItem.Product.Quantity}");

            // Assuming you need to update the product stock too
            int quantityChange = quantity - cartItem.Quantity;
            if (cartItem.Product.Quantity < quantityChange)
            {
                Console.WriteLine("Insufficient stock available.");
                return Json(new { success = false, message = "Insufficient stock available." });
            }

            cartItem.Quantity = quantity;
            cartItem.Product.Quantity -= quantityChange;  // Adjust stock based on the new quantity set

            Console.WriteLine($"Updated cart item quantity to {cartItem.Quantity}, Updated product stock to {cartItem.Product.Quantity}");

            try
            {
                await _context.SaveChangesAsync();
                UpdateCartCount(cartItem.CustId);
                var updatedCartCount = HttpContext.Session.GetInt32("CartCount");
                Console.WriteLine("Database updated successfully.");
                return Json(new { success = true, message = "Quantity updated successfully.", cartCount = updatedCartCount });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating database: {ex.Message}");
                return Json(new { success = false, message = "Error updating quantity." });
            }
        }



        [HttpPost]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var cartItem = await _context.CartItems.Include(ci => ci.Product).FirstOrDefaultAsync(ci => ci.CartItemId == id);
    if (cartItem == null)
    {
        return Json(new { success = false, message = "Item not found." });
    }

    cartItem.Product.Quantity += cartItem.Quantity; // Restore the quantity to product

    _context.CartItems.Remove(cartItem);
    await _context.SaveChangesAsync();
    UpdateCartCount(cartItem.CustId);

    return Json(new { success = true });
}

        [HttpGet]
        public IActionResult GetCartItemQuantity(int productId)
        {
            int custId = HttpContext.Session.GetInt32("CustId").Value;
           

            var cartItem = _context.CartItems
                .Where(ci => ci.ProductId == productId && ci.CustId == custId)
                .FirstOrDefault();

            int quantity = cartItem != null ? cartItem.Quantity : 0;

            return Json(new { quantity = quantity });
        }
        [HttpPost]
        public IActionResult ValidateCart()
        {
            int custId = HttpContext.Session.GetInt32("CustId").Value;
         

            var cartItems = _context.CartItems
                .Where(ci => ci.CustId == custId)
                .Select(ci => new
                {
                    ci.CartItemId,
                    ci.ProductId,
                    ci.Quantity,
                    AvailableQuantity = _context.Product.Where(p => p.Id == ci.ProductId).Select(p => p.Quantity).FirstOrDefault()
                })
                .ToList();

            var invalidItems = cartItems.Where(ci => ci.Quantity > ci.AvailableQuantity).ToList();

            return Json(new { success = invalidItems.Count == 0, invalidItems = invalidItems });
        }

    }
}