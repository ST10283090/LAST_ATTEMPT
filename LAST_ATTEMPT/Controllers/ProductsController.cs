using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LAST_ATTEMPT.Data;
using LAST_ATTEMPT.Models;

namespace LAST_ATTEMPT.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
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
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Prod_Name,Prod_Description,Price,Prod_Availability,Category,CreatedAt,UpdatedAt")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(HomeController.EditProducts), "Home");

            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Prod_Name,Prod_Description,Price,Prod_Availability,Category,CreatedAt,UpdatedAt")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            
            var originalProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            if (ModelState.IsValid)
            {
                try
                {
                    
                    product.CreatedAt = originalProduct.CreatedAt; 
                    product.UpdatedAt = DateTime.Now; //added this cuase the updated and created time was just outputing as the same time
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(HomeController.EditProducts), "Home");

            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
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
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(HomeController.EditProducts), "Home");//that generic index page will find me in my sleep i swear

        }

        // POST: Products/AddToCart/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Retrieve the user ID of the current user
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Check if the item already exists in the user's shopping cart
            var existingCartItem = await _context.AddToCarts
                .Where(item => item.ProductId == id && item.UserId == userId)
                .FirstOrDefaultAsync();

            if (existingCartItem != null)
            {
                
                existingCartItem.Quantity += 1;
            }
            else
            {
               
                var newCartItem = new AddToCart
                {
                    ProductId = id,
                    UserId = userId,
                    Quantity = 1
                };
                _context.AddToCarts.Add(newCartItem);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect back to the same page
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            // Find the item in the cart
            var cartItem = await _context.AddToCarts.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            // Remove the item from the cart
            _context.AddToCarts.Remove(cartItem);
            await _context.SaveChangesAsync();

            // Redirect back to the shopping cart page
            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var cartItems = await _context.AddToCarts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (cartItems.Count == 0)
            {
                return RedirectToAction(nameof(Cart));
            }

            var order = new Order
            {
                UserId = userId,
                OrderItems = cartItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.AddToCarts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderHistory));
        }

        public async Task<IActionResult> OrderHistory()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            return View(orders);
        }


        public async Task<IActionResult> Cart()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var cartItems = await _context.AddToCarts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PutInRequest()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }

            // Create a new order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Processing",
                OrderItems = await _context.AddToCarts
                    .Where(c => c.UserId == userId)
                    .Select(c => new OrderItem
                    {
                        ProductId = c.ProductId,
                        Quantity = c.Quantity,
                        Price = c.Product.Price
                    })
                    .ToListAsync()
            };

            _context.Orders.Add(order);

            // Clear the shopping cart
            var cartItems = await _context.AddToCarts.Where(c => c.UserId == userId).ToListAsync();
            _context.AddToCarts.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderProcessing), "Products");
        }

        public IActionResult OrderProcessing()
        {
            return View();
        }
    }
}



