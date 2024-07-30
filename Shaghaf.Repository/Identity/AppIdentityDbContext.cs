using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shaghaf.Core.Entities.Identity;
using Shaghaf.Repository.Identity.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shaghaf.Repository.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions options)
            :base(options)
        {
            
        }

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    }
}
