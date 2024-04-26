using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
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
    public class ProductsController : ControllerBase
    {
        private readonly StoreDbContext _context;
        private readonly IRoleId _roleId;

        public ProductsController(StoreDbContext context, IRoleId roleId)
        {
            _context = context;
            _roleId = roleId;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(int page)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            return await _context.Products.Skip(12 * page).Take(12).ToListAsync();
        }

        [HttpGet("ProductsNotInInventory")]
        [Authorize(Roles = "Shop,Admin")]
        public async Task<ActionResult> ProductsNotInventory(int page)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Iden iden = _roleId.GetIden(identity);
            if (iden == null)
            {
                return NotFound();
            }
            var products = _context.Products.Where(p => !_context.Inventories.Any(i => i.ShopId == iden.Id && i.ProductId == p.Id)).Skip(12 * page).Take(12).AsQueryable();
            int count = _context.Products.Where(p => !_context.Inventories.Any(i => i.ShopId == iden.Id && i.ProductId == p.Id)).AsQueryable().Count();
            ProdInvPagination prods = new ProdInvPagination();
            prods.products = products;
            prods.page = page;
            prods.count = count;

            return Ok(prods);
        }

        [HttpGet("image")]
        public async Task<ActionResult> GetImage(int id)
        {
            return Ok(await _context.ProductImages.FirstOrDefaultAsync(i => i.ProductId == id));
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProduct(int id, ProductDTO product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            ProductImage img1 = await _context.ProductImages.Where(i => i.Id == product.ImageId).FirstOrDefaultAsync();
            if (img1 == null)
            {
                return NotFound("Image id doesnot match");
            }
            _context.ProductImages.Remove(img1);

            Product product1 = await _context.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            product1.Name = product.Name;
            product1.Description = product.Description;
            product1.Brand = product.Brand;
            Console.WriteLine(product.Brand);
            product1.Price = product.Price;
            product1.Category = product.Category;
            product1.SubCategory = product.SubCategory;
            product1.Info = product.Info;

            ProductImage img = new ProductImage()
            {
                Image = product.Image
            };
            product1.ProductImages.Add(img);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin,Shop")]
        public async Task<ActionResult<Product>> PostProduct(ProductDTO product)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'StoreDbContext.Products'  is null.");
            }

            Product prod = new Product()
            {
                Name = product.Name,
                Description = product.Description,
                Brand = product.Brand,
                Price = product.Price,
                Category = product.Category,
                SubCategory = product.SubCategory,
                Info = product.Info
            };

            ProductImage prodImg = new ProductImage()
            {
                Image = product.Image
            };

            prod.ProductImages.Add(prodImg);
            try
            {
                await _context.Products.AddAsync(prod);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Added Product");
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Find")]
        public async Task<ActionResult<List<Product>>> FindProducts(ProductSearchParamsShop param)
        {
            List<Product> products = await _context.Products.Where(p => p.Name.ToLower().Contains(param.Name)).ToListAsync();
            if (products.Count == 0)
            {
                return NotFound();
            }
            return Ok(products);
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
