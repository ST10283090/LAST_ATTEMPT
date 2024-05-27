using LAST_ATTEMPT.Data;
using LAST_ATTEMPT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LAST_ATTEMPT.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult HomeScreen()
        {
            return View();
        }

        public IActionResult ArtnCraft()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        public IActionResult EditProducts()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewAllOrders()
        {
            var allOrders = await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ToListAsync();
            return View(allOrders);
        }

        [Authorize]
        public async Task<IActionResult> ViewMyOrders()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var customerOrders = await _context.Orders.Where(o => o.UserId == userId).Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ToListAsync();
            return View(customerOrders);
        }

        public IActionResult Orders()
        {
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToList();

            return View(orders);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ClearOrders()
        {
            var orderItems = _context.OrderItems.ToList();
            var orders = _context.Orders.ToList();

            _context.OrderItems.RemoveRange(orderItems);
            _context.Orders.RemoveRange(orders);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ViewAllOrders)); 
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessNow(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = "Processed";
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ViewAllOrders));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessOrder(int id, string newStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = newStatus; 
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ViewAllOrders));
        }

        // Add the CreateOrder actions here
        public IActionResult CreateOrder()
        {
            var order = new Order
            {
                OrderDate = DateTime.Now,
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                Status = "Processing",
                OrderItems = new List<OrderItem>()
            };

            ViewBag.Products = _context.Products.ToList(); 
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order, int[] selectedProducts)
        {
            if (ModelState.IsValid)
            {
                foreach (var productId in selectedProducts)
                {
                    var product = await _context.Products.FindAsync(productId);
                    if (product != null)
                    {
                        order.OrderItems.Add(new OrderItem
                        {
                            ProductId = product.Id,
                            Quantity = 1, // i tried to make this like a yes or no, for availabilty but it wasnt working 
                            Price = product.Price
                        });
                    }
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return RedirectToAction("ViewMyOrders");
            }

            ViewBag.Products = _context.Products.ToList(); 
            return View(order);
        }
    }
}

