using Armoire.Interfaces;
using Armoire.Models;
using Microsoft.EntityFrameworkCore;

namespace Armoire.Utils;

// https://medium.com/@PoulLorca/integrating-entity-framework-with-avalonia-ui-using-sqlite-a-gentle-introduction-8fdf4772a2c9
public class AppDbContext : DbContext
{
    public DbSet<Drawer> Drawers { get; set; }
    public DbSet<Item> Items { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ArmoireData.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Drawer>()
            .HasOne(d => d.ParentDrawer)
            .WithMany(d => d.Drawers)
            .HasForeignKey(d => d.ParentDrawerId);

        modelBuilder
            .Entity<Item>()
            .HasOne(d => d.ParentDrawer)
            .WithMany(d => d.Items)
            .HasForeignKey(d => d.ParentDrawerId);
    }
}
