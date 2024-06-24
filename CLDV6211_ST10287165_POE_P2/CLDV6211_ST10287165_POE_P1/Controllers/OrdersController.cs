using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLDV6211_ST10287165_POE_P1.Data;
using CLDV6211_ST10287165_POE_P1.Models;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using CLDV6211_ST10287165_POE_P1.Migrations;
using System.Diagnostics;
using Azure;
using Newtonsoft.Json;
using System.Text;

namespace CLDV6211_ST10287165_POE_P1.Controllers
{
    public class OrdersController : Controller
    {
        private readonly CLDV6211_ST10287165_POE_P1Context _context;

        public OrdersController(CLDV6211_ST10287165_POE_P1Context context)
        {
            _context = context;
        }

        // GET: Orders
      
        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CustId"] = new SelectList(_context.Customer, "CustId", "CustId");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderDate,OrderStatus,CustId,ShippingAddress,City,PostalCode,Country")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustId"] = new SelectList(_context.Customer, "CustId", "CustId", order.CustId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustId"] = new SelectList(_context.Customer, "CustId", "CustId", order.CustId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,OrderDate,OrderStatus,CustId,ShippingAddress,City,PostalCode,Country")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CustId"] = new SelectList(_context.Customer, "CustId", "CustId", order.CustId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
        public async Task<IActionResult> OrderHistory()
        {
            int custId = HttpContext.Session.GetInt32("CustId").GetValueOrDefault();
            var orders = await _context.Orders
                .Where(o => o.CustId == custId && o.OrderStatus == "Processed")
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();
            return View(orders);
        }

        // Client side: View all orders
        public async Task<IActionResult> Index()
        {
            var customerId = HttpContext.Session.GetInt32("Custd");
            var orders = await _context.Orders.ToListAsync();
            return View(orders);
        }

        // Client side: Process order
        [HttpPost]
        public async Task<IActionResult> ProcessOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null && order.OrderStatus == "Pending")
            {
                order.OrderStatus = "Processed";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }



        public IActionResult ViewOrders()
        {
            int clientId = HttpContext.Session.GetInt32("ClientId").GetValueOrDefault();  // Get the logged-in client ID

            var orders = _context.OrderDetails
                .Where(od => od.Product.ClientId == clientId)  // Filter by ClientId associated with the product
                .Select(od => od.Order)
                .Distinct()
                .ToList();

            return View(orders);
        }
        // GET action for editing an order
        public IActionResult EditOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST action for editing an order
        [HttpPost]
        public IActionResult EditOrder(Order model)
        {
            if (ModelState.IsValid)
            {
                _context.Update(model);
                _context.SaveChanges();
                return RedirectToAction("ViewOrders");
            }
            return View(model);
        }

        public IActionResult ViewProcessedOrders()
        {
            int customerId = HttpContext.Session.GetInt32("CustomerId").GetValueOrDefault();

            // Include the OrderDetails to access Price and Quantity
            var orders = _context.Orders
                .Where(o => o.CustId == customerId && o.OrderStatus == "Processed")
                .Include(o => o.OrderDetails)  // Make sure to include this
                .ToList();

            return View(orders);
        }
        /*public IActionResult OrderConfirmation(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product) // Include this if you want to show product details in the confirmation
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound(); // Handle cases where the order is not found
            }

            return View(order);
        }*/
        /* public async Task<IActionResult> OrderConfirmation(int id)
         {
             using (var client = new HttpClient())
             {

                 try
                 {
                     var response = await client.GetAsync($"https://st10287165poefunctions20240622212132.azurewebsites.net/api/DetailsOrchestrator?id={id}");
                     response.EnsureSuccessStatusCode();
                     var content = await response.Content.ReadAsStringAsync();
                     // Process the response content
                     return View("OrderConfirmation", content);
                 }
                 catch (HttpRequestException ex)
                 {
                     //var content = await response.Content.ReadAsStringAsync();
                     // Handle error (e.g., log the error, show a user-friendly message, etc.)
                     return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

                 }
             }
         }
 */
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound(); // Handle cases where the order is not found
            }

            // You might want to send a confirmation or a specific detail to the Durable Function
            var confirmationDetails = new { OrderId = order.OrderId, Confirmed = true };

            // Serialize minimal data to JSON to pass to the Durable Function
            var confirmationJson = JsonConvert.SerializeObject(confirmationDetails);

            // Assuming you have a function to call the Durable Function
            var durableFunctionResult = await CallDurableFunction(confirmationJson);

            return View(order); // or return a specific view that shows confirmation details
        }

        private async Task<string> CallDurableFunction(string confirmationJson)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("https://khumalocraftfunction20240624184822.azurewebsites.net/", new StringContent(confirmationJson, Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }



        [HttpGet]
        public IActionResult Checkout()
        {
            var customerId = HttpContext.Session.GetInt32("CustId"); // Retrieve the customer ID from session
            if (customerId == null)
            {
                Console.WriteLine("No customer ID found in session.");
                return RedirectToAction("Login", "Customers"); // Redirect to login if no session found.
            }

            var cartItems = GetCartItems(customerId.Value); // Use the method to get cart items
            if (cartItems.Count == 0)
            {
                Console.WriteLine("No items in the cart for customer ID " + customerId.Value);
                return View("EmptyCart"); // Return an empty cart view if no items are in the cart
            }

            var customer = _context.Customer.Find(customerId.Value); // Retrieve the customer
            if (customer == null)
            {
                Console.WriteLine("Customer not found for ID " + customerId.Value);
                return RedirectToAction("Login", "Customers"); // Redirect to login if customer not found
            }

            var order = new Order
            {
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                CustId = customerId.Value,
                Customer = customer, // Set the fully populated customer
                OrderDetails = cartItems.Select(ci => new OrderDetail
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price
                }).ToList()
            };

            Console.WriteLine("Passing order to the view for customer ID " + customerId.Value);
            return View(order); // Pass the order to the view
        }

        /* [HttpPost]
         public IActionResult Checkout(Order order)
         {
             var customerId = HttpContext.Session.GetInt32("CustId"); // Retrieve the customer ID from session
             if (customerId == null)
             {
                 Console.WriteLine("No customer ID found in session.");
                 return RedirectToAction("Login", "Customers"); // Redirect to login if no session found.
             }

             var cartItems = GetCartItems(customerId.Value); // Retrieve cart items again
             if (cartItems.Count == 0)
             {
                 Console.WriteLine("No items in the cart for customer ID " + customerId.Value);
                 return View("EmptyCart"); // Return an empty cart view if no items are in the cart
             }

             if (ModelState.IsValid)
             {
                 Console.WriteLine("ModelState is valid.");
                 order.CustId = customerId.Value; // Ensure customer ID is set correctly
                 order.Customer = _context.Customer.Find(customerId.Value); // Ensure the Customer is set correctly
                 order.OrderDate = DateTime.Now; // Set the order date to the current date and time
                 order.PaymentMethod = "COD"; // Ensure payment method is set to COD

                 // Populate OrderDetails from cart items
                 order.OrderDetails = cartItems.Select(ci => new OrderDetail
                 {
                     ProductId = ci.ProductId,
                     Quantity = ci.Quantity,
                     Price = ci.Product.Price
                 }).ToList();

                 Console.WriteLine("Adding order to database...");
                 _context.Orders.Add(order);
                 _context.SaveChanges();
                 Console.WriteLine("Order saved to database.");

                 HttpContext.Session.Remove("CartItem"); // Optionally clear cart after checkout.
                 return RedirectToAction("OrderConfirmation", new { id = order.OrderId });
             }

             Console.WriteLine("ModelState is not valid.");
             // Log the validation errors if the model state is not valid
             foreach (var value in ModelState.Values)
             {
                 foreach (var error in value.Errors)
                 {
                     Console.WriteLine($"Validation error: {error.ErrorMessage}");
                 }
             }

             return View(order);
         }
 */
        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            var customerId = HttpContext.Session.GetInt32("CustId"); // Retrieve the customer ID from session
            if (customerId == null)
            {
                Console.WriteLine("No customer ID found in session.");
                return RedirectToAction("Login", "Customers"); // Redirect to login if no session found.
            }

            var cartItems = GetCartItems(customerId.Value); // Retrieve cart items again
            if (cartItems.Count == 0)
            {
                Console.WriteLine("No items in the cart for customer ID " + customerId.Value);
                return View("EmptyCart"); // Return an empty cart view if no items are in the cart
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState is valid.");
                order.CustId = customerId.Value; // Ensure customer ID is set correctly
                order.Customer = _context.Customer.Find(customerId.Value); // Ensure the Customer is set correctly
                order.OrderDate = DateTime.Now; // Set the order date to the current date and time
                order.OrderStatus = "Pending";
                order.PaymentMethod = "COD"; // Ensure payment method is set to COD

                // Populate OrderDetails from cart items
                order.OrderDetails = cartItems.Select(ci => new OrderDetail
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price
                }).ToList();

                Console.WriteLine("Adding order to database...");
                _context.Orders.Add(order);
                _context.SaveChanges();
                Console.WriteLine("Order saved to database.");

                // Optionally clear cart after checkout.
                foreach (var cartItem in cartItems)
                {
                    _context.CartItems.Remove(cartItem);
                }
                _context.SaveChanges();

                // Update cart count
                var cartCount = GetCartCount(customerId.Value);
                HttpContext.Session.SetInt32("CartCount", cartCount);

                return RedirectToAction("OrderConfirmation", new { id = order.OrderId });
            }

            Console.WriteLine("ModelState is not valid.");
            // Log the validation errors if the model state is not valid
            foreach (var value in ModelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
            }

            return View(order);
        }

     

        private List<CartItem> GetCartItems(int customerId)
        {
            // Fetch cart items from the database where the customer ID matches
            // and include Product data for display or further operations.
            return _context.CartItems
                           .Where(ci => ci.CustId == customerId)
                           .Include(ci => ci.Product) // Ensure you include navigation properties if needed
                           .ToList();
        }
        public int GetCartCount(int customerId)
        {
            return _context.CartItems
                           .Where(ci => ci.CustId == customerId)
                           .Sum(ci => ci.Quantity);
        }



        public async Task<IActionResult> CustOrders()
        {
            if (!HttpContext.Session.GetInt32("CustId").HasValue)
            {
                return RedirectToAction("Login", "Customers"); // Redirect to login if CustId is not in session
            }

            int custId = HttpContext.Session.GetInt32("CustId").Value;

            var orders = await _context.Orders
                                .Where(o => o.CustId == custId)
                                .Include(o => o.OrderDetails)
                                    .ThenInclude(od => od.Product)
                                .ToListAsync();

            return View(orders);
        }

        // GET: Orders/AllOrders
        /*    public async Task<IActionResult> AllOrders()
            {
                var orders = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .ToListAsync();

                ViewBag.OrderCount = orders.Count;
                return View(orders);
            }*/
        public async Task<IActionResult> AllOrders(string searchOrderId)
        {
            var orders = from o in _context.Orders.Include(o => o.Customer)
                         select o;

            if (!String.IsNullOrEmpty(searchOrderId))
            {
                orders = orders.Where(o => o.OrderId.ToString().Contains(searchOrderId));
            }

            ViewBag.SearchOrderId = searchOrderId;
            return View(await orders.ToListAsync());
        }


        // POST: Orders/UpdateOrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.OrderStatus = newStatus;
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(AllOrders));
        }



        // Method to get the count of orders
        public async Task<IActionResult> OrderCount()
        {
            var orderCount = await _context.Orders.CountAsync();
            ViewBag.OrderCount = orderCount;
            return View();
        }

        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            if (orderDetails == null)
            {
                return NotFound();
            }

            return PartialView("_OrderDetailsPartial", orderDetails);
        }
    }
}
