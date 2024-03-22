
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Data;
using QRMenuAPI.Models;
using System.Security.Claims;

namespace QRMenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RestaurantsController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Restaurants
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurants()
        {
            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            return await _context.Restaurants.Where(c => c.StateId == 1).ToListAsync();
        }

        [HttpGet("All")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetAllRestaurants()
        {
            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            return await _context.Restaurants.ToListAsync();
        }

        // GET: api/Restaurants/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Restaurant>> GetRestaurant(int id)
        {
            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return restaurant;
        }

        [HttpGet("Menu/{id}")]
        public ActionResult<Restaurant> GetMenu(int id)
        {

            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            var restaurant = _context.Restaurants.Include(r => r.Categories).ThenInclude(c => c.Foods).FirstOrDefault(r => r.Id == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return restaurant;
        }

        // PUT: api/Restaurants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "RestaurantAdministrator")]
        public ActionResult PutRestaurant(int id, Restaurant restaurant)
        {

            if (User.HasClaim("RestaurantId", id.ToString()) == false)
            {
                return Content("Wrong User");
            }
            _context.Entry(restaurant).State = EntityState.Modified;
            _context.SaveChanges();
            return Content("Updated");
        }
        [HttpPut("StateUpdate/{id}")]
        [Authorize(Roles = "RestaurantAdministrator")]
        public ActionResult UpdateState(int id, byte stateId)
        {

            if (User.HasClaim("RestaurantId", id.ToString()) == false)
            {
                return Content("Wrong User");
            }
            var restaurant = _context.Restaurants.Find(id);
            if (restaurant != null)
            {
                restaurant.StateId = stateId;
                _context.Restaurants.Update(restaurant);
                IQueryable<Category> categories = _context.Categories.Where(r => r.RestaurantId == id);
                foreach (Category category in categories)
                {
                    category.StateId = stateId;
                    _context.Categories.Update(category);
                    IQueryable<Food> foods = _context.Foods.Where(f => f.CategoryId == category.Id);
                    foreach (Food food in foods)
                    {
                        food.StateId = stateId;
                        _context.Foods.Update(food);
                    }
                }

                IQueryable<ApplicationUser> users = _context.Users.Where(u => u.CompanyId == id);
                foreach (ApplicationUser user in users)
                {

                    _context.Users.Update(user);
                }
            }

            _context.SaveChanges();
            return Content("State Updated");
        }

        // POST: api/Restaurants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "CompanyAdministrator")]
        public int PostRestaurant(Restaurant restaurant)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            Claim claim;

            _context.Restaurants.Add(restaurant);
            _context.SaveChanges();
            applicationUser.CompanyId = restaurant.CompanyId;
            applicationUser.Email = restaurant.Email;
            applicationUser.Name = restaurant.Name;
            applicationUser.PhoneNumber = restaurant.Phone;
            applicationUser.RegisterDate = DateTime.Today;
            applicationUser.StateId = restaurant.StateId;
            applicationUser.UserName = restaurant.Name + restaurant.Id.ToString();
            _userManager.CreateAsync(applicationUser, "Admin1234!").Wait();
            claim = new Claim("RestaurantId", restaurant.Id.ToString());
            _userManager.AddClaimAsync(applicationUser, claim).Wait();
            _userManager.AddToRoleAsync(applicationUser, "RestaurantAdministrator").Wait();
            return restaurant.Id;
        }

        // DELETE: api/Restaurants/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "CompanyAdministrator,RestaurantAdministrator")]
        public ActionResult DeleteRestaurant(int id)
        {
            if (User.HasClaim("RestaurantId", id.ToString()) == false)
            {
                return Unauthorized();
            }
            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            var restaurant = _context.Restaurants.Find(id);
            if (restaurant != null)
            {
                restaurant.StateId = 0;
                _context.Restaurants.Update(restaurant);
                IQueryable<Category> categories = _context.Categories.Where(r => r.RestaurantId == id);
                foreach (Category category in categories)
                {
                    category.StateId = 0;
                    _context.Categories.Update(category);
                    IQueryable<Food> foods = _context.Foods.Where(f => f.CategoryId == category.Id);
                    foreach (Food food in foods)
                    {
                        food.StateId = 0;
                        _context.Foods.Update(food);
                    }
                }
               
                IQueryable<RestaurantUser> users = _context.RestaurantUsers.Where(u => u.RestaurantId == id);
                foreach (RestaurantUser user in users)
                {
                   _context.RestaurantUsers.Remove(user);
                }
            }
            
            _context.SaveChanges();
            return Content("Deleted");
        }

        private bool RestaurantExists(int id)
        {
            return (_context.Restaurants?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        
    }
}
