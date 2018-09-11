using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorldOfTravels.Models;

namespace WorldOfTravels.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<WorldOfTravels.Models.Country> Country { get; set; }
        public DbSet<WorldOfTravels.Models.Post> Post { get; set; }
    }
}
