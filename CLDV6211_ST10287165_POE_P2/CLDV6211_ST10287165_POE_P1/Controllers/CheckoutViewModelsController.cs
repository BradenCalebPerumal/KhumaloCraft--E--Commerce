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
    public class CheckoutViewModelsController : Controller
    {
        private readonly CLDV6211_ST10287165_POE_P1Context _context;

        public CheckoutViewModelsController(CLDV6211_ST10287165_POE_P1Context context)
        {
            _context = context;
        }

        // GET: CheckoutViewModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.CheckoutViewModel.ToListAsync());
        }

        public IActionResult FormView()
        {
            var model = new CheckoutViewModel(); // Assuming you need to initialize it here
                           // Populate model as needed
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkoutViewModel = await _context.CheckoutViewModel
                .FirstOrDefaultAsync(m => m.CheckoutViewModelKey == id);
            if (checkoutViewModel == null)
            {
                return NotFound();
            }

            return View(checkoutViewModel);
        }

        // GET: CheckoutViewModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CheckoutViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CheckoutViewModelKey,ShippingAddress,City,PostalCode,Country,CreditCardNumber,ExpiryDate,CVV")] CheckoutViewModel checkoutViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(checkoutViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(checkoutViewModel);
        }

        // GET: CheckoutViewModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkoutViewModel = await _context.CheckoutViewModel.FindAsync(id);
            if (checkoutViewModel == null)
            {
                return NotFound();
            }
            return View(checkoutViewModel);
        }

        // POST: CheckoutViewModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CheckoutViewModelKey,ShippingAddress,City,PostalCode,Country,CreditCardNumber,ExpiryDate,CVV")] CheckoutViewModel checkoutViewModel)
        {
            if (id != checkoutViewModel.CheckoutViewModelKey)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(checkoutViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CheckoutViewModelExists(checkoutViewModel.CheckoutViewModelKey))
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
            return View(checkoutViewModel);
        }

        // GET: CheckoutViewModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkoutViewModel = await _context.CheckoutViewModel
                .FirstOrDefaultAsync(m => m.CheckoutViewModelKey == id);
            if (checkoutViewModel == null)
            {
                return NotFound();
            }

            return View(checkoutViewModel);
        }

        // POST: CheckoutViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var checkoutViewModel = await _context.CheckoutViewModel.FindAsync(id);
            if (checkoutViewModel != null)
            {
                _context.CheckoutViewModel.Remove(checkoutViewModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CheckoutViewModelExists(int id)
        {
            return _context.CheckoutViewModel.Any(e => e.CheckoutViewModelKey == id);
        }




        /*        [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> ConfirmOrder(CheckoutViewModel model)
                {
                    int custId = HttpContext.Session.GetInt32("CustId").GetValueOrDefault();
                    if (custId == 0)
                    {
                        // Handle unauthenticated session or redirect to login
                        TempData["Error"] = "Please log in to confirm your order.";
                        return RedirectToAction("Login");
                    }

                    if (!ModelState.IsValid)
                    {
                        return View("Checkout", model);
                    }

                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            var order = new Order
                            {
                                CustId = custId,
                                OrderDate = DateTime.Now,
                                OrderStatus = "Pending",
                                ShippingAddress = model.ShippingAddress,
                                City = model.City,
                                PostalCode = model.PostalCode,
                                Country = model.Country
                            };

                            _context.Orders.Add(order);
                            await _context.SaveChangesAsync();  // Save the order before adding details

                            foreach (var item in model.CartItems)
                            {
                                var orderDetail = new OrderDetail
                                {
                                    OrderId = order.OrderId,
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    Price = item.Product?.Price  // Handle nullable Price
                                };
                                _context.OrderDetails.Add(orderDetail);
                            }

                            await _context.SaveChangesAsync();  // Save all order details at once

                            // Clear the cart here
                            _context.CartItems.RemoveRange(model.CartItems);
                            await _context.SaveChangesAsync();

                            transaction.Commit();  // Commit the transaction

                            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();  // Roll back the transaction on error
                                                     // Log the exception
                            TempData["Error"] = "An error occurred while placing your order. Please try again.";
                            return View("Checkout", model);
                        }
                    }
                }
        */




       

        // This action handles the actual checkout form where users enter shipping and payment details
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(CheckoutViewModel model)
        {
            Console.WriteLine("ConfirmOrder called"); // Log entry
            int custId = HttpContext.Session.GetInt32("CustId").GetValueOrDefault();
            if (custId == 0)
            {
                TempData["Error"] = "Please log in to confirm your order.";
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid"); // Log entry
                return View("Checkout", model);
            }

            try
            {
                var order = new Order
                {
                    CustId = custId,
                    OrderDate = DateTime.Now,
                    OrderStatus = "Pending",
                    ShippingAddress = model.ShippingAddress,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    Country = model.Country
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in model.CartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Product?.Price
                    };
                    _context.OrderDetails.Add(orderDetail);
                }

                await _context.SaveChangesAsync();
                _context.CartItems.RemoveRange(model.CartItems);
                await _context.SaveChangesAsync();

                return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred: " + ex.Message); // Log exception
                TempData["Error"] = "An error occurred while placing your order. Please try again.";
                return View("Checkout", model);
            }
        }



        
        public IActionResult ViewOrders()
        {
            int custId = HttpContext.Session.GetInt32("CustId").GetValueOrDefault();
            var orders = _context.Orders
                .Where(o => o.CustId == custId && o.OrderStatus == "Processed")  // Remove the status condition if you want to show all statuses
                .ToList();

            return View(orders);
        }
        [HttpGet]
        public IActionResult SubmitOrder()
        {
            int custId = HttpContext.Session.GetInt32("CustId").GetValueOrDefault();
            var cartItems = _context.CartItems.Where(c => c.CustId == custId).Include(c => c.Product).ToList();

            if (!cartItems.Any())
            {
                // Optionally redirect to a different page if the cart is empty
                return RedirectToAction("CartEmpty"); // Define a "CartEmpty" view or similar
            }

            var model = new CheckoutViewModel
            {
                CartItems = cartItems,
                // Initialize other properties as needed for the checkout form
            };

            return View(model); // This assumes your view is named "SubmitOrder.cshtml"
        }

        [HttpPost]
        public IActionResult SubmitOrder(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                int custId = HttpContext.Session.GetInt32("CustId").GetValueOrDefault();

                // Creating the new order object
                var newOrder = new Order
                {
                    CustId = custId,
                    OrderDate = DateTime.Now,
                    OrderStatus = "Pending",
                    ShippingAddress = model.ShippingAddress,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    Country = model.Country,
                    PaymentMethod = model.PaymentMethod // Now this should work
                };

                _context.Orders.Add(newOrder);
                _context.SaveChanges(); // Save the order to generate the OrderId

                // Transfer items from cart to order details and remove them from the cart
                var cartItems = _context.CartItems.Where(c => c.CustId == custId).ToList();
                foreach (var item in cartItems)
                {
                    _context.OrderDetails.Add(new OrderDetail
                    {
                        OrderId = newOrder.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Product.Price
                    });

                    _context.CartItems.Remove(item);
                }

                _context.Orders.Add(newOrder);
                _context.SaveChanges();

                return RedirectToAction("OrderConfirmation", "Orders", new { id = newOrder.OrderId });

            }

            return View(model);
        }

 



    }
}
