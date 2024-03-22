
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Data;
using QRMenuAPI.Models;

namespace QRMenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public FoodsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Foods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Food>>> GetFoods()
        {
            if (_context.Foods == null)
            {
                return NotFound();
            }
            return await _context.Foods.Where(f=>f.StateId == 1).ToListAsync();
        }

        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Food>>> GetAllFoods()
        {
            if (_context.Foods == null)
            {
                return NotFound();
            }
            return await _context.Foods.ToListAsync();
        }

        // GET: api/Foods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
            if (_context.Foods == null)
            {
                return NotFound();
            }
            var food = await _context.Foods.FindAsync(id);

            if (food == null)
            {
                return NotFound();
            }

            return food;
        }

        // PUT: api/Foods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "RestaurantAdministrator")]
        public ActionResult PutFood(int id, Food food)
        {
            var category = _context.Categories.Find(food.CategoryId);

            if (User.HasClaim("RestauranId", category.RestaurantId.ToString()) == false)
            {
                return NotFound();
            }
            _context.Entry(food).State = EntityState.Modified;
            _context.SaveChanges();
            return Content("Updated");
        }

        // POST: api/Foods
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "RestaurantAdministrator")]
        public string PostFood(Food food)
        {
            _context.Foods.Add(food);
            _context.SaveChanges();

            return "Food addeds";
        }

        // DELETE: api/Foods/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "RestaurantAdministrator")]
        public ActionResult DeleteFood(int id)
        {
            var food = _context.Foods.Find(id);
            var category = _context.Categories.Find(food.CategoryId);

            if (User.HasClaim("RestauranId", category.RestaurantId.ToString()) == false)
            {
                return NotFound();
            }
            if (_context.Foods == null)
            {
                return NotFound();
            }

            if (food == null)
            {
                return NotFound();
            }
            food.StateId = 0;
            _context.Foods.Update(food);
            _context.SaveChanges();

            return Content("Deleted");
        }

        private bool FoodExists(int id)
        {
            return (_context.Foods?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
