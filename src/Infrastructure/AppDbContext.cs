// Infrastructure/AppDbContext.cs
using Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();
    public DbSet<GroupPermission> GroupPermissions => Set<GroupPermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Users
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.Id);

             e.Property(x => x.Email)
                .HasMaxLength(256)
                .IsRequired();

                e.HasIndex(x => x.Email).IsUnique();

            e.Property(x => x.DisplayName)
                .HasMaxLength(200)
                .IsRequired();

            e.Property(x => x.CreatedAt).HasPrecision(3);
        });

        // Groups
        modelBuilder.Entity<Group>(e =>
        {
            e.ToTable("Groups");
            e.HasKey(x => x.Id);

            e.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            e.HasIndex(x => x.Name).IsUnique();

            e.Property(x => x.Description).HasMaxLength(400);
            e.Property(x => x.CreatedAt).HasPrecision(3);
        });

        // Permissions
        modelBuilder.Entity<Permission>(e =>
        {
            e.ToTable("Permissions");
            e.HasKey(x => x.Id);

            e.Property(x => x.Key)
                .HasMaxLength(150)
                .IsRequired();

            e.HasIndex(x => x.Key).IsUnique();

            e.Property(x => x.Description).HasMaxLength(400);
        });

        // UserGroups (M:N)
        modelBuilder.Entity<UserGroup>(e =>
        {
            e.ToTable("UserGroups");
            e.HasKey(x => new { x.UserId, x.GroupId });

            e.Property(x => x.AddedUtc).HasPrecision(3);

            e.HasOne(x => x.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Group)
                .WithMany(g => g.UserGroups)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => x.GroupId); // common query: users by group
        });

        // GroupPermissions (M:N)
        modelBuilder.Entity<GroupPermission>(e =>
        {
            e.ToTable("GroupPermissions");
            e.HasKey(x => new { x.GroupId, x.PermissionId });

            e.HasOne(x => x.Group)
                .WithMany(g => g.GroupPermissions)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Permission)
                .WithMany(p => p.GroupPermissions)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => x.PermissionId);
        });

        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        // Fixed IDs for determining seeding
        var seededTimestamp = DateTime.SpecifyKind(new DateTime(2024, 1, 1, 0, 0, 0), DateTimeKind.Utc);

        var adminGroup = new Group { Id = 1, Name = "Admin",  Description = "Full access", CreatedAt = seededTimestamp };
        var level1     = new Group { Id = 2, Name = "Level1", Description = "Operational access", CreatedAt = seededTimestamp };
        var level2     = new Group { Id = 3, Name = "Level2", Description = "Read-only access", CreatedAt = seededTimestamp };

        modelBuilder.Entity<Group>().HasData(adminGroup, level1, level2);

        var permissions = new[]
        {
            new Permission { Id = 1, Key = "users.read",        Description = "View users" },
            new Permission { Id = 2, Key = "users.manage",      Description = "Create/update/deactivate users" },
            new Permission { Id = 3, Key = "groups.read",       Description = "View groups" },
            new Permission { Id = 4, Key = "groups.manage",     Description = "Create/update/delete groups" },
            new Permission { Id = 5, Key = "permissions.read",  Description = "View permissions" },
            new Permission { Id = 6, Key = "billing.read",      Description = "View billing" },
            new Permission { Id = 7, Key = "billing.manage",    Description = "Manage billing" },
            new Permission { Id = 8, Key = "reports.read",      Description = "View reports" },
            new Permission { Id = 9, Key = "reports.export",    Description = "Export reports" }
        };

        modelBuilder.Entity<Permission>().HasData(permissions);

        // Admin gets all permissions
        var adminLinks = permissions.Select(p => new GroupPermission
        {
            GroupId = 1,
            PermissionId = p.Id
        });

        // Level1 gets a subset
        var level1PermIds = new[] { 1, 3, 5, 6, 8, 9 };
        var level1Links = level1PermIds.Select(pid => new GroupPermission
        {
            GroupId = 2,
            PermissionId = pid
        });
        
        // Level2 read-only subset
        var level2PermIds = new[] { 6, 8 };
        var level2Links = level2PermIds.Select(pid => new GroupPermission
        {
            GroupId = 3,
            PermissionId = pid
        });

        modelBuilder.Entity<GroupPermission>().HasData(
            adminLinks.Concat(level1Links).Concat(level2Links).ToArray()
        );
    
}}