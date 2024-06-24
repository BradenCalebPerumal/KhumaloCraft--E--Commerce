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
    public class ProductsController : Controller
    {
        private readonly CLDV6211_ST10287165_POE_P1Context _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(CLDV6211_ST10287165_POE_P1Context context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Products
        public IActionResult Index(string searchString, int? categoryId)
        {
            var products = from p in _context.Product.Include(p => p.Category)
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.SearchString = searchString;
            ViewBag.CategoryId = categoryId;
            return View(products.ToList());
        }

        // GET: Products/Delete/5


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var clientId = HttpContext.Session.GetInt32("ClientId");
            if (clientId == null)
            {
                return RedirectToAction("Loginn", "Clients");
            }

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            var clientId = HttpContext.Session.GetInt32("ClientId");
            if (clientId == null)
            {
                ModelState.AddModelError("", "You must be logged in to add products.");
                return RedirectToAction("AboutUs", "Home");
            }

            var client = _context.Client.FirstOrDefault(c => c.ClientId == clientId);
            if (client == null)
            {
                ModelState.AddModelError("", "Unable to find client information.");
                return RedirectToAction("TermsOfUse", "Home");
            }

            if (product.ImageType == "Upload")
            {
                if (product.ImageUpload != null && product.ImageUpload.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageUpload.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "/uploads/" + fileName;
                }
                else
                {
                    ModelState.AddModelError("ImageUpload", "Please upload an image file.");
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (product.ImageType == "Url")
            {
                if (string.IsNullOrWhiteSpace(product.ImageUrl))
                {
                    ModelState.AddModelError("ImageUrl", "Please enter a URL.");
                }
                else
                {
                    if (!Uri.TryCreate(product.ImageUrl, UriKind.Absolute, out Uri? uriResult) ||
                        (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                    {
                        ModelState.AddModelError("ImageUrl", "Invalid URL format. Please provide a valid URL.");
                    }
                }
            }

            product.ClientId = clientId.Value;
            product.Client = client;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
                return View(product);
            }

            try
            {
                _context.Product.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard", "Clients");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error saving product: {0}", ex.Message);
                ModelState.AddModelError("", "An error occurred while saving the product. Please try again.");
                ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
                return View(product);
            }
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? ImageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Product.FindAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Update the existing product fields
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.Quantity = product.Quantity;
                    existingProduct.ImageType = product.ImageType;
                    existingProduct.ImageUrl = product.ImageUrl;
                    existingProduct.ImageUpload = product.ImageUpload;
                    // Handle image update logic
                    if (product.ImageType == "Upload" && ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(ImageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        existingProduct.ImageUrl = "/images/" + fileName;
                    }
                    else if (product.ImageType == "Url" && !string.IsNullOrEmpty(product.ImageUrl))
                    {
                        existingProduct.ImageUrl = product.ImageUrl;
                    }

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(EditDisplay));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

        // GET: Products/EditDisplay
        public IActionResult EditDisplay()
        {
            var isClientLoggedIn = HttpContext.Session.GetString("IsClientLoggedIn");
            if (isClientLoggedIn != "true")
            {
                return RedirectToAction("Login", "Clients");
            }

            var clientId = HttpContext.Session.GetInt32("ClientId");
            var clientProducts = _context.Product
                .Where(p => p.ClientId == clientId)
                .Include(p => p.Category)
                .ToList();

            if (!clientProducts.Any())
            {
                ViewBag.Message = "No products listed.";
            }

            return View(clientProducts);
        }

        public IActionResult EditProduct(int id)
        {
            var product = _context.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, Product product, IFormFile? ImageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (product.ImageType == "Upload" && (ImageFile == null || ImageFile.Length == 0))
            {
                ModelState.AddModelError("ImageFile", "Please upload an image file.");
            }
            else if (product.ImageType == "Url" && string.IsNullOrEmpty(product.ImageUrl))
            {
                ModelState.AddModelError("ImageUrl", "Please enter a valid URL.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Product.FindAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Update the existing product fields
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.Quantity = product.Quantity;

                    // Handle image update logic
                    if (product.ImageType == "Upload" && ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(ImageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        existingProduct.ImageUrl = "/images/" + fileName;
                    }
                    else if (product.ImageType == "Url" && !string.IsNullOrEmpty(product.ImageUrl))
                    {
                        existingProduct.ImageUrl = product.ImageUrl;
                    }

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(EditDisplay));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        public async Task<IActionResult> GetProductDetails(int id)
        {
            var product = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return PartialView("_ProductDetailsPartial", product);
        }

    }
}