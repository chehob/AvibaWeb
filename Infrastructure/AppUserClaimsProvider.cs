using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using AvibaWeb.Models;

namespace AvibaWeb.Infrastructure
{
    public class AppUserClaimsProvider
    {
        public static IEnumerable<Claim> GetClaims(AppUser user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Name", user.Name ?? user.UserName));
            if (user.Position != null)
            {
                claims.Add(new Claim("Position", user.Position));
            }
            return claims;
        }

        private static Claim CreateClaim(string type, string value)
        {
            return new Claim(type, value, ClaimValueTypes.String, "RemoteClaims");
        }
    }
}