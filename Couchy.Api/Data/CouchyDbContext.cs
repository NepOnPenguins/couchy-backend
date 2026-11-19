using Couchy.Api.Common;
using Couchy.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Couchy.Api.Data;

/// <summary>
/// Replaces the Spring Data repositories. Table and column names are mapped to
/// the names Hibernate generated, so this app can run on the existing database.
/// </summary>
public class CouchyDbContext : DbContext
{
    private const string Timestamp = "timestamp without time zone";

    public CouchyDbContext(DbContextOptions<CouchyDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<StayRequest> StayRequests => Set<StayRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(u => u.UserId);
            e.Property(u => u.UserId).HasColumnName("user_id");
            e.Property(u => u.Username).HasColumnName("user_name").IsRequired();
            e.Property(u => u.Email).HasColumnName("email_id").IsRequired();
            e.Property(u => u.Password).HasColumnName("password").IsRequired();
            e.Property(u => u.Role).HasColumnName("role").HasConversion<string>();
            e.Property(u => u.Status).HasColumnName("status").HasConversion<string>();
            e.Property(u => u.CreatedAt).HasColumnName("created_at").HasColumnType(Timestamp);
            e.Property(u => u.UpdatedAt).HasColumnName("updated_at").HasColumnType(Timestamp);
            e.HasIndex(u => u.Username).IsUnique();
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Profile>(e =>
        {
            e.ToTable("profiles");
            e.HasKey(p => p.ProfileId);
            e.Property(p => p.ProfileId).HasColumnName("profile_id");
            e.Property(p => p.UserId).HasColumnName("user_id");
            e.Property(p => p.FirstName).HasColumnName("first_name");
            e.Property(p => p.LastName).HasColumnName("last_name");
            e.Property(p => p.Gender).HasColumnName("gender");
            e.Property(p => p.Pronouns).HasColumnName("pronouns");
            e.Property(p => p.Languages).HasColumnName("languages");
            e.Property(p => p.HostingStatus).HasColumnName("hosting_status").HasConversion<string>();
            e.Property(p => p.MeetingStatus).HasColumnName("meeting_status").HasConversion<string>();
            e.Property(p => p.About).HasColumnName("about").HasMaxLength(1000);

            // One user has at most one profile (this also creates the unique index on user_id).
            e.HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<Profile>(p => p.UserId)
                .IsRequired();

            e.OwnsOne(p => p.Location, loc =>
            {
                loc.Property(l => l.Country).HasColumnName("country");
                loc.Property(l => l.City).HasColumnName("city");
                loc.Property(l => l.Latitude).HasColumnName("latitude");
                loc.Property(l => l.Longitude).HasColumnName("longitude");
                loc.Property(l => l.District).HasColumnName("district");
            });

            e.OwnsOne(p => p.HomeDetails, home =>
            {
                home.Property(h => h.MaxGuests).HasColumnName("max_guests");
                home.Property(h => h.HasWifi).HasColumnName("has_wifi");
                home.Property(h => h.HasKitchen).HasColumnName("has_kitchen");
                home.Property(h => h.HasWashingMachine).HasColumnName("has_washing_machine");
                home.Property(h => h.HasHeating).HasColumnName("has_heating");
                home.Property(h => h.HasAirConditioning).HasColumnName("has_air_conditioning");
                home.Property(h => h.HasBathroom).HasColumnName("has_bathroom");
                home.Property(h => h.PetsAllowed).HasColumnName("pets_allowed");
                home.Property(h => h.SmokingAllowed).HasColumnName("smoking_allowed");
                home.Property(h => h.SleepingArrangement).HasColumnName("sleeping_arrangement");
                home.Property(h => h.HouseRules).HasColumnName("house_rules");
            });
        });

        modelBuilder.Entity<StayRequest>(e =>
        {
            e.ToTable("stay_requests");
            e.HasKey(s => s.Id);
            e.Property(s => s.Id).HasColumnName("id");
            e.Property(s => s.TravelerProfileId).HasColumnName("traveler_id");
            e.Property(s => s.HostProfileId).HasColumnName("host_id");
            e.Property(s => s.Status).HasColumnName("status").HasConversion<string>();
            e.Property(s => s.CreatedAt).HasColumnName("created_at").HasColumnType(Timestamp);
            e.Property(s => s.InitialMessage).HasColumnName("initial_message").HasColumnType("text").IsRequired();

            e.HasOne(s => s.TravelerProfile)
                .WithMany()
                .HasForeignKey(s => s.TravelerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(s => s.HostProfile)
                .WithMany()
                .HasForeignKey(s => s.HostProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // Same job as JPA auditing (@CreatedDate / @LastModifiedDate) on User.
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyTimestamps()
    {
        var now = Clock.Now();

        foreach (var entry in ChangeTracker.Entries<User>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}
