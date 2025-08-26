using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Dal.Entities
{
    public class AppUsr : IdentityUser { /* champs supplémentaires éventuels */ }

    public class AuthDbContext : IdentityDbContext<AppUsr>
    {
        public AuthDbContext(DbContextOptions<MinshpDatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
