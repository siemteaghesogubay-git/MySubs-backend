namespace MySubs.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using MySubs.Models;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
        IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<FamilyGroup> FamilyGroups { get; set; }
        public DbSet<FamilyGroupMember> FamilyGroupMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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

            builder.Entity<Subscription>()
                .Property(s => s.Cost)
                .HasColumnType("decimal(10,2)");

            // RefreshToken -> ApplicationUser (many-to-one)
            builder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade); // raderar refresh tokens om användaren tas bort

            // FamilyGroupMember -> FamilyGroup (many-to-one)
            builder.Entity<FamilyGroupMember>()
                .HasOne(fgm => fgm.FamilyGroup)
                .WithMany(fg => fg.Members)
                .HasForeignKey(fgm => fgm.FamilyGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // FamilyGroupMember -> ApplicationUser (many-to-one)
            builder.Entity<FamilyGroupMember>()
                .HasOne(fgm => fgm.User)
                .WithMany()
                .HasForeignKey(fgm => fgm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // FamilyGroup -> ApplicationUser (skaparen)
            builder.Entity<FamilyGroup>()
                .HasOne(fg => fg.CreatedByUser)
                .WithMany()
                .HasForeignKey(fg => fg.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict); // hindrar att skaparen raderas om gruppen finns kvar

            // En användare kan bara vara medlem i en grupp en gång
            builder.Entity<FamilyGroupMember>()
                .HasIndex(fgm => new { fgm.FamilyGroupId, fgm.UserId })
                .IsUnique();
        }
    }
}