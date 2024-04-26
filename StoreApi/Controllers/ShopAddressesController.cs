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
    [Authorize(Roles = "Shop")]
    public class ShopAddressesController : ControllerBase
    {
        private readonly StoreDbContext _context;
        private readonly IRoleId _roleId;

        public ShopAddressesController(StoreDbContext context, IRoleId roleId)
        {
            _context = context;
            _roleId = roleId;
        }

        // GET: api/ShopAddresses
        [HttpGet]
        public async Task<ActionResult<ShopAddress>> GetShopAddresses()
        {
            if (_context.ShopAddresses == null)
            {
                return NotFound();
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);
            return Ok(await _context.ShopAddresses.Where(sa => sa.ShopId == iden.Id).FirstOrDefaultAsync());
        }

        /*
                // GET: api/ShopAddresses/5
                [HttpGet("{id}")]
                [Authorize(Roles = "Shop")]
                public async Task<ActionResult<ShopAddress>> GetShopAddress(int id)
                {
                    if (_context.ShopAddresses == null)
                    {
                        return NotFound();
                    }
                    var shopAddress = await _context.ShopAddresses.FindAsync(id);

                    if (shopAddress == null)
                    {
                        return NotFound();
                    }

                    return shopAddress;
                }
        */
        // PUT: api/ShopAddresses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutShopAddress(ShopAddress shopAddress)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);

            ShopAddress existingShopAddress = await _context.ShopAddresses.Where(sa => sa.ShopId == iden.Id).FirstOrDefaultAsync();

            if (existingShopAddress == null)
            {
                return NotFound(shopAddress);
            }
            existingShopAddress.Address = shopAddress.Address;
            existingShopAddress.Building = shopAddress.Building;
            existingShopAddress.CoordinateX = shopAddress.CoordinateX;
            existingShopAddress.CoordinateY = shopAddress.CoordinateY;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopAddressExists(shopAddress.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(existingShopAddress);
        }

        // POST: api/ShopAddresses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostShopAddress(ShopAddress shopAddress)
        {
            if (_context.ShopAddresses == null)
            {
                return Problem("Entity set 'StoreDbContext.ShopAddresses'  is null.");
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);
            if (_context.ShopAddresses.Where(sa => sa.ShopId == iden.Id).Count() == 0)
            {
                shopAddress.ShopId = iden.Id;
                await _context.ShopAddresses.AddAsync(shopAddress);
                await _context.SaveChangesAsync();
                return Ok("Added address");
            }
            return BadRequest("Address already exists");

        }

        // DELETE: api/ShopAddresses/5
        [HttpDelete]
        public async Task<IActionResult> DeleteShopAddress([FromForm] IFormCollection form)
        {
            if (_context.ShopAddresses == null)
            {
                return NotFound();
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);
            var shopAddress = await _context.ShopAddresses.Where(sa => sa.Id == int.Parse(form["id"]) && sa.ShopId == iden.Id).FirstOrDefaultAsync();
            if (shopAddress == null)
            {
                return NotFound();
            }

            _context.ShopAddresses.Remove(shopAddress);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }

        private bool ShopAddressExists(int id)
        {
            return (_context.ShopAddresses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
