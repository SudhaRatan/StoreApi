using StoreApi.Models.IdentityModel;
using System.Security.Claims;

namespace StoreApi.Functions
{
    public interface IRoleId
    {
        public Iden GetIden(ClaimsIdentity identity);
    }
}
