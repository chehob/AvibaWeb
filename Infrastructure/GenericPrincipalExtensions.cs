using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace AvibaWeb.Infrastructure
{
    public static class GenericPrincipalExtensions
    {
        public static string Name(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                var name = claimsIdentity.Claims.SingleOrDefault(c => c.Type == "Name");
                return name == null ? "" : name.Value;
            }
            else
                return "";
        }
    }
}