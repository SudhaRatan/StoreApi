using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApi.Functions;
using StoreApi.Models;
using StoreApi.Models.IdentityModel;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class UserAddressesController : ControllerBase
    {
        private readonly StoreDbContext _context;
        private readonly IRoleId _roleId;

        public UserAddressesController(StoreDbContext context, IRoleId roleId)
        {
            _context = context;
            _roleId = roleId;
        }

        // GET: api/UserAddresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAddress>>> GetUserAddresses()
        {
            if (_context.UserAddresses == null)
            {
                return NotFound();
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);

            List<UserAddress> addresses = await _context.UserAddresses.Where(ua => ua.UserId == iden.Id).ToListAsync();
            if (addresses.Count != 0)
            {
                return addresses;
            }
            return NotFound();

        }

        // GET: api/UserAddresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserAddress>> GetUserAddress(int id)
        {
            if (_context.UserAddresses == null)
            {
                return NotFound();
            }
            var userAddress = await _context.UserAddresses.FindAsync(id);

            if (userAddress == null)
            {
                return NotFound();
            }

            return userAddress;
        }

        // PUT: api/UserAddresses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutUserAddress(UserAddress userAddress)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);

            UserAddress existingUserAddress = await _context.UserAddresses.Where(ua => ua.UserId == iden.Id && ua.Id == userAddress.Id).FirstOrDefaultAsync();

            if (existingUserAddress == null)
            {
                return NotFound(userAddress);
            }
            existingUserAddress.Name = userAddress.Name;
            existingUserAddress.PhoneNumber = userAddress.PhoneNumber;
            existingUserAddress.City = userAddress.City;
            existingUserAddress.Pincode = userAddress.Pincode;
            existingUserAddress.StreetName = userAddress.StreetName;
            existingUserAddress.Building = userAddress.Building;
            existingUserAddress.CoordinateX = userAddress.CoordinateX;
            existingUserAddress.CoordinateY = userAddress.CoordinateY;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAddressExists(userAddress.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserAddresses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserAddress>> PostUserAddress(UserAddress userAddress)
        {
            if (_context.UserAddresses == null)
            {
                return Problem("Entity set 'StoreDbContext.UserAddresses'  is null.");
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);
            userAddress.UserId = iden.Id;
            await _context.UserAddresses.AddAsync(userAddress);
            await _context.SaveChangesAsync();

            return Ok("Added address");
        }

        // DELETE: api/UserAddresses/5
        [HttpDelete]
        public async Task<IActionResult> DeleteUserAddress([FromForm] IFormCollection form)
        {
            if (_context.UserAddresses == null)
            {
                return NotFound();
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);

            var userAddress = await _context.UserAddresses.Where(ua => ua.Id == int.Parse(form["id"]) && ua.UserId == iden.Id).FirstOrDefaultAsync();
            if (userAddress == null)
            {
                return NotFound();
            }

            _context.UserAddresses.Remove(userAddress);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }

        private bool UserAddressExists(int id)
        {
            return (_context.UserAddresses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
