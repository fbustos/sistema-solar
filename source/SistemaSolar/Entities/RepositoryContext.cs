using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Entities
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Pronostico> Pronosticos { get; set; }

        public DbSet<Planeta> Planetas { get; set; }
    }
}
