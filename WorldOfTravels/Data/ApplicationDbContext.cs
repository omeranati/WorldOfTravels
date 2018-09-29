using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorldOfTravels.Models;

namespace WorldOfTravels.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<WorldOfTravels.Models.Country> Country { get; set; }
        public DbSet<WorldOfTravels.Models.Post> Post { get; set; }
        public DbSet<WorldOfTravels.Models.Comment> Comment { get; set; }
        public DbSet<WorldOfTravels.Models.User> User { get; set; }
    }
}
