using Microsoft.AspNetCore.Identity;
using Shaghaf.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shaghaf.Repository.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    Email = "eslam.s.eltayar@gmail.com",
                    UserName = "eslam.eltayar",
                    PhoneNumber = "01065357827",

                };
                await userManager.CreateAsync(user, "Pa$$w0rd");
            }

        }
    }
}
