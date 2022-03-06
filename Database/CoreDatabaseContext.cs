using Microsoft.EntityFrameworkCore;
using PitaPairing.Domain.Application;
using PitaPairing.Domain.Index;
using PitaPairing.Domain.Module;
using PitaPairing.Domain.Post;
using PitaPairing.Domain.Semester;
using PitaPairing.User;

namespace PitaPairing.Database;

public class CoreDbContext : DbContext
{
    public DbSet<UserData> Users { get; set; } = null!;
    public DbSet<ModuleData> Modules { get; set; } = null!;
    public DbSet<IndexData> Indexes { get; set; } = null!;

    public DbSet<PostData> Posts { get; set; } = null!;

    public DbSet<ApplicationData> Applications { get; set; } = null!;

    public DbSet<SemesterData> Semester { get; set; } = null!;

    public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // configuring user
        var user = modelBuilder.Entity<UserData>();
        user.HasIndex(x => x.Name);
        user.HasIndex(x => x.Email)
            .IsUnique();
        user.HasIndex(x => x.Sub)
            .IsUnique();

        //  configuring module
        var module = modelBuilder.Entity<ModuleData>();
        module.HasIndex(x => new {x.Semester, x.CourseCode})
            .IsUnique();
        module.HasIndex(x => x.Name);

        // configuring index
        var index = modelBuilder.Entity<IndexData>();
        index.HasIndex(x => new {x.ModuleId, x.Code})
            .IsUnique();

        // configuring posts
        var post = modelBuilder.Entity<PostData>();
        post.HasIndex(x => new {x.UserId, x.IndexId}).IsUnique();

        modelBuilder.Entity<PostData>()
            .HasOne(x => x.Index)
            .WithMany()
            .HasForeignKey(p => p.IndexId);

        // Many to Many of Post and Indexes
        modelBuilder.Entity<PostData>()
            .HasMany(p => p.LookingFor)
            .WithMany(p => p.RelatedPosts)
            .UsingEntity(j => j.ToTable("PostsIndexes"));

        var application = modelBuilder.Entity<ApplicationData>();
        application.HasIndex(x => new {x.PostId, x.UserId})
            .IsUnique();
    }
}