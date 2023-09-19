using System;
using System.Collections.Generic;
using System.Linq;
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
    [Authorize(Roles = "Shop")]
    public class InventoriesController : ControllerBase
    {
        private readonly StoreDbContext _context;
        private readonly IRoleId _roleId;

        public InventoriesController(StoreDbContext context, IRoleId roleId)
        {
            _context = context;
            _roleId = roleId;
        }
        /*
                // GET: api/Inventories
                [HttpGet]
                public async Task<ActionResult<IEnumerable<Inventory>>> GetInventories()
                {
                  if (_context.Inventories == null)
                  {
                      return NotFound();
                  }
                    return await _context.Inventories.ToListAsync();
                }
        */
        // GET: api/Inventories/5
        [HttpGet]
        public async Task<ActionResult> GetInventory(int page)
        {
            /*if (_context.Inventories == null)
            {
                return NotFound();
            }*/
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);

            var inventory = await _context.Inventories.Where(i => i.ShopId == iden.Id).Include(i => i.Product).ToListAsync();

            /* if (inventory == null)
             {
                 return NotFound();
             }*/
            InvPagination invs = new InvPagination();
            invs.inventories = inventory;
            invs.page = page;
            return Ok(invs);
        }

        // PUT: api/Inventories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutInventory(Inventory inventory)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);
            if (inventory.ShopId != iden.Id)
            {
                return Unauthorized("Access denied");
            }

            Inventory existingInventory = _context.Inventories.FirstOrDefault(i => i.ShopId == inventory.ShopId && i.ProductId == inventory.ProductId);

            if (existingInventory == null)
            {
                return NotFound();
            }
            existingInventory.Quantity = inventory.Quantity;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryExists(inventory.Id))
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

        // POST: api/Inventories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Inventory>> PostInventory(Inventory inventory)
        {
            if (_context.Inventories == null)
            {
                return Problem("Entity set 'StoreDbContext.Inventories'  is null.");
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);

            Inventory existingInventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ShopId == iden.Id && i.ProductId == inventory.ProductId);

            if (existingInventory != null)
            {
                return BadRequest("Product already in inventory");
            }
            inventory.ShopId = iden.Id;
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();

            return Ok("Added product");
        }

        // DELETE: api/Inventories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            if (_context.Inventories == null)
            {
                return NotFound();
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);

            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }
            if(inventory.ShopId != iden.Id)
            {
                return Unauthorized("Access denied");
            }

            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }

        private bool InventoryExists(int id)
        {
            return (_context.Inventories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
