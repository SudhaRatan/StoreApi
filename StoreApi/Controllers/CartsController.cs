using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApi.DataTransferObjects;
using StoreApi.Functions;
using StoreApi.Models;
using StoreApi.Models.IdentityModel;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class CartsController : ControllerBase
    {
        private readonly StoreDbContext _context;
        private readonly IRoleId _roleId;

        public CartsController(StoreDbContext context, IRoleId roleId)
        {
            _context = context;
            _roleId = roleId;
        }

        // GET: api/Carts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCarts()
        {
            if (_context.Carts == null)
            {
                return NotFound();
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);
            return await _context.Carts.Where(c => c.UserId == iden.Id).ToListAsync();
        }

        // GET: api/Carts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            if (_context.Carts == null)
            {
                return NotFound();
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);

            var cart = await _context.Carts
                .Include(c => c.ProductCarts)
                    .ThenInclude(productCart => productCart.Product)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == iden.Id);

            if (cart == null)
            {
                return NotFound();
            }

            return cart;
        }

        // PUT: api/Carts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutCart(CartProductDTO cartProduct)
        {
            ProductCart productCart = await _context.ProductCarts.FindAsync(cartProduct.Id);
            productCart.Quantity = cartProduct.Quantity;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(cartProduct.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Updated");
        }

        // POST: api/Carts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{ShopId}")]
        public async Task<ActionResult<Cart>> PostCart(int ShopId, ProductCart cartProduct)
        {
            if (_context.Carts == null)
            {
                return Problem("Entity set 'StoreDbContext.Carts'  is null.");
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);

            Cart existingCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == iden.Id && c.ShopId == ShopId);
            bool exists = true;
            if (existingCart == null)
            {
                existingCart = new Cart();
                existingCart.UserId = iden.Id;
                existingCart.ShopId = ShopId;
                exists = false;
            }
            cartProduct.Quantity = 1;

            existingCart.ProductCarts.Add(cartProduct);

            if (exists)
                _context.Carts.Update(existingCart);
            else
                _context.Carts.Add(existingCart);

            await _context.SaveChangesAsync();


            return Ok("Added to cart");
        }

        // DELETE: api/Carts/5
        [HttpDelete("{id}")] // Clear cart
        public async Task<IActionResult> DeleteCart(int id)
        {
            if (_context.Carts == null)
            {
                return NotFound();
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.Id == id && c.UserId == iden.Id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return Ok("Cleared cart");
        }

        [HttpDelete("DeleteCartProduct/{id}")]
        public async Task<IActionResult> DeleteCartProduct(int id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);

            ProductCart cartProduct = await _context.ProductCarts.FindAsync(id);
            if (cartProduct == null)
            {
                return NotFound();
            }
            _context.ProductCarts.Remove(cartProduct);
            await _context.SaveChangesAsync();
            return Ok("Removed from cart");
        }

        [HttpDelete("DeleteAllCarts")]
        public async Task<IActionResult> DeleteAllCarts()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);

            _context.Carts.RemoveRange(_context.Carts.Where(c => c.UserId == iden.Id));
            await _context.SaveChangesAsync();

            return Ok("Removed all carts");
        }

        private bool CartExists(int id)
        {
            return (_context.Carts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
