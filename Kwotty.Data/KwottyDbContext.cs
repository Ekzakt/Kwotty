using Kwotty.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Kwotty.Data;

public class KwottyDbContext : DbContext // Or your preferred DbContext base class
{
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Medium> Media { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Quote> Quotes { get; set; } // Maps to 'Quotes' table now
    public DbSet<Rating> QuoteRatings { get; set; }
    public DbSet<QuoteCategory> QuoteCategories { get; set; }
    public DbSet<QuoteItem> QuoteItems { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<UserCategory> UserCategories { get; set; }


    public KwottyDbContext(DbContextOptions<KwottyDbContext> options) : base(options) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Composite Primary Keys for Join Tables

        modelBuilder.Entity<UserCategory>()
            .HasKey(uc => new { uc.UserId, uc.CategoryId });

        modelBuilder.Entity<UserCategory>()
            .HasOne(uc => uc.UserSettings)
            .WithMany(us => us.UserCategories)
            .HasForeignKey(uc => uc.UserId);

        modelBuilder.Entity<UserCategory>()
            .HasOne(uc => uc.Category)
            .WithMany(c => c.UserCategories)
            .HasForeignKey(uc => uc.CategoryId);


        modelBuilder.Entity<QuoteCategory>()
            .HasKey(qc => new { qc.QuoteId, qc.CategoryId });

        modelBuilder.Entity<QuoteCategory>()
            .HasOne(qc => qc.Quote)
            .WithMany(q => q.QuoteCategories)
            .HasForeignKey(qc => qc.QuoteId);

        modelBuilder.Entity<QuoteCategory>()
            .HasOne(qc => qc.Category)
            .WithMany(c => c.QuoteCategories)
            .HasForeignKey(qc => qc.CategoryId);


        modelBuilder.Entity<Rating>()
            .HasKey(qr => new { qr.QuoteId, qr.UserId });

        modelBuilder.Entity<Rating>()
            .HasOne(qr => qr.Quote)
            .WithMany(q => q.QuoteRatings)
            .HasForeignKey(qr => qr.QuoteId);

        modelBuilder.Entity<Rating>()
            .HasOne(qr => qr.User)
            .WithMany(u => u.QuoteRatings)
            .HasForeignKey(qr => qr.UserId);


        // Configure One-to-One relationship between User and UserSettings

        modelBuilder.Entity<User>()
            .HasOne(u => u.UserSettings)
            .WithOne(us => us.User)
            .HasForeignKey<UserSettings>(us => us.UserId);

    }
}