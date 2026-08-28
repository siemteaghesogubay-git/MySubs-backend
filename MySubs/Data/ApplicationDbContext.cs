namespace MySubs.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using MySubs.Models;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // viktigt - måste köras först för att Identity-tabellerna ska skapas korrekt

            // Subscription -> Category (many-to-one)
            builder.Entity<Subscription>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Subscriptions)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // hindrar att en kategori raderas om prenumerationer använder den

            // Subscription -> ApplicationUser (many-to-one)
            builder.Entity<Subscription>()
                .HasOne(s => s.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade); // raderar användarens prenumerationer om kontot tas bort

            // Explicit kolumntyp för pengar - undviker precisionsproblem med decimal/float
            builder.Entity<Subscription>()
                .Property(s => s.Cost)
                .HasColumnType("decimal(10,2)");
        }
    }
}