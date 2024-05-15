﻿using Microsoft.EntityFrameworkCore;
using Pronia.Models;

namespace Pronia.DataAccesLayer
{
    public class ProniaContext : DbContext
    {
        public ProniaContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Slider> Sliders { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }


        public DbSet<Product> Products { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries()) 
            {
                switch(entry.State)
                {
                    case EntityState.Added:
                        ((BaseEntity)entry.Entity).CreatedTime= DateTime.UtcNow;
                        ((BaseEntity)entry.Entity).IsDeleted = false;
                        break;
                    case EntityState.Modified:
                        ((BaseEntity)entry.Entity).UpdatedTime= DateTime.UtcNow;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Server=DESKTOP-GNV6364\SQLEXPRESS;Database=Pronia;Trusted_Connection=True;TrustServerCertificate=True");
            base.OnConfiguring(options);
        }
    }
}
