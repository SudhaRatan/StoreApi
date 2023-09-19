using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoreApi.Functions;
using StoreApi.Models;
using StoreApi.Models.IdentityModel;
using StoreApi.Models.LoginModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private StoreDbContext _context;
        private IRoleId _roleId;
        public LoginController(IConfiguration configuration, StoreDbContext context, IRoleId roleId)
        {
            _config = configuration;
            _context = context;
            _roleId = roleId;
        }

        [HttpPost("ShopLogin")]
        public async Task<IActionResult> ShopLogin([FromBody] ShopLogin shopLogin)
        {
            var shop = await AuthenticateShop(shopLogin);

            if (shop != null)
            {
                var token = GenerateToken(shop.Id, "Shop");
                return Ok(token);
            }
            return NotFound("Shop not found or wrong password");
        }

        [HttpPost("UserLogin")]
        public async Task<IActionResult> UserLogin([FromBody] UserLogin userLogin)
        {
            var user = await AuthenticateUser(userLogin);

            if (user != null)
            {
                var token = GenerateToken(user.Id, "User");
                return Ok(token);
            }
            return NotFound("User not found or wrong password");
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(User user)
        {
            if (ModelState.IsValid)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            else return BadRequest(user);
        }

        [HttpPost("RegisterShop")]
        public async Task<IActionResult> RegisterShop(Shop shop)
        {
            if (ModelState.IsValid)
            {
                await _context.Shops.AddAsync(shop);
                await _context.SaveChangesAsync();
                return Ok(shop);
            }
            else return BadRequest(shop);
        }



        [HttpGet("GetUser")]
        [Authorize(Roles = "User")]
        public async Task<User> GetUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                Iden iden = _roleId.GetIden(identity);
                var User = await _context.Users.FirstOrDefaultAsync(x => x.Id == iden.Id);
                return User;
            }
            return null;
        }

        [HttpGet("GetShop")]
        [Authorize(Roles = "Shop")]
        public async Task<Shop> GetShop()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                Iden iden = _roleId.GetIden(identity);
                var Shop = await _context.Shops.Include(s => s.ShopAddresses).Where(s => s.Id == iden.Id).FirstOrDefaultAsync();
                return Shop;
            }
            return null;
        }

        [HttpPut("EditUser")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> EditUser([FromBody] User user)
        {
            if (user.Name != null && user.PhoneNumber != null)
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    Iden iden = _roleId.GetIden(identity);
                    User user1 = await _context.Users.FirstOrDefaultAsync(o => o.Id == iden.Id);
                    user1.Name = user.Name;
                    user1.PhoneNumber = user.PhoneNumber;
                    _context.SaveChangesAsync();
                    return Ok("Updated");
                }
                return BadRequest("Error");
            }
            return BadRequest("Model not valid");
        }

        [HttpPut("EditShop")]
        [Authorize(Roles = "Shop")]
        public async Task<IActionResult> EditShop([FromBody] Shop shop)
        {
            if (shop.ShopName != null && shop.PhoneNumber != null && shop.OwnerName != null)
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    Iden iden = _roleId.GetIden(identity);
                    Shop shop1 = await _context.Shops.FirstOrDefaultAsync(o => o.Id == iden.Id);
                    shop1.OwnerName = shop.OwnerName;
                    shop1.ShopName = shop.ShopName;
                    shop1.PhoneNumber = shop.PhoneNumber;
                    _context.SaveChangesAsync();
                    return Ok("Updated");
                }
                return BadRequest("Error");
            }
            return BadRequest("Model not valid");
        }

        [HttpPost("GetAdminToken")]
        public async Task<IActionResult> AdminLogin(Admin admin)
        {
            Admin ad = await _context.Admins.FirstOrDefaultAsync(a => a.UserName == admin.UserName && a.Password == admin.Password);
            if (ad != null)
            {
                var token = GenerateToken(ad.Id, "Admin");
                return Ok(token);
            }
            return NotFound("Invalid Credentials");
        }

        /*
                [HttpGet("GetShop")]
                [Authorize(Roles ="shop")]
                public IActionResult GetCurrentShop() {
                    var identity = HttpContext.User.Identity as ClaimsIdentity;
                    if (identity != null)
                    {
                        var shopClaims = identity.Claims;
                        return Ok(shopClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role).Value);
                    }
                    return NotFound("Error");
                }
        */

        private string GenerateToken(int id, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name,id.ToString()),
                new Claim(ClaimTypes.Role,role)
            };
            var token = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<Shop> AuthenticateShop(ShopLogin shopLogin)
        {
            var currentShop = await _context.Shops.FirstOrDefaultAsync(o => o.PhoneNumber == shopLogin.PhoneNumber && o.Password == shopLogin.Password);
            if (currentShop != null)
            {
                return currentShop;
            }
            return null;
        }

        private async Task<User> AuthenticateUser(UserLogin userLogin)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(o => o.PhoneNumber == userLogin.PhoneNumber && o.Password == userLogin.Password);
            if (currentUser != null)
            {
                return currentUser;
            }
            return null;
        }
    }
}
