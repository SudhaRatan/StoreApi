using StoreApi.Models.IdentityModel;
using System.Security.Claims;

namespace StoreApi.Functions
{
    public class RoleId : IRoleId
    {
        public Iden GetIden(ClaimsIdentity identity)
        {
            var userClaims = identity.Claims;
            Iden iden = new Iden()
            {
                Id = int.Parse(userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name).Value),
                Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role).Value
            };
            return iden;
        }
    }
}
