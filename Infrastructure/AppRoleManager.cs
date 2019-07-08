using AvibaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace AvibaWeb.Infrastructure
{
    public class AppRoleManager : RoleManager<AppRole>, IDisposable
    {
        public AppRoleManager(RoleStore<AppRole> store)
           : base(store)
        {
        }

        public static AppRoleManager Create(IdentityFactoryOptions<AppRoleManager> options, IOwinContext context)
        {
            return new AppRoleManager(new RoleStore<AppRole>(context.Get<AppIdentityDbContext>()));
        }
    }
}