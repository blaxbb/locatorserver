using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using LocatorServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace LocatorServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<LocatorUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<LocatorServer.Models.LocationEntry> LocationEntry { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var admin = new IdentityRole("admin")
            {
                NormalizedName = "ADMIN",
                Id = "5c3e7b18-6942-440c-b1a1-1f9fb6e92730",
                ConcurrencyStamp = "230790d1-1678-4cb7-b62c-0b15103f8235"
            };
            var auth = new IdentityRole("authorized")
            {
                NormalizedName = "AUTHORIZED",
                Id = "2i3e7218-4232-420c-z4q2-1f9q65492768",
                ConcurrencyStamp = "d4b9f87f-abff-4cfb-ba8b-4e547bd9b3bb"
            };
            builder.Entity<IdentityRole>().HasData(admin, auth);
        }
    }
}
