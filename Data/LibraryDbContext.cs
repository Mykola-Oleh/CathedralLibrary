using CathedralLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibrary.Data;

public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
    public DbSet<Publication> Publications => Set<Publication>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<PublicationAuthor> PublicationAuthors => Set<PublicationAuthor>();
    public DbSet<PublicationType> PublicationTypes => Set<PublicationType>();
    public DbSet<Reader> Readers => Set<Reader>();
    public DbSet<Copy> Copies => Set<Copy>();
    public DbSet<CopyStatus> CopyStatuses => Set<CopyStatus>();
    public DbSet<LibraryRequest> Requests => Set<LibraryRequest>();
    public DbSet<RequestStatus> RequestStatuses => Set<RequestStatus>();
    public DbSet<FundChange> FundChanges => Set<FundChange>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PublicationAuthor>()
            .HasKey(x => new { x.PublicationId, x.AuthorId });

        modelBuilder.Entity<PublicationAuthor>()
            .HasOne(x => x.Publication).WithMany(x => x.PublicationAuthors)
            .HasForeignKey(x => x.PublicationId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PublicationAuthor>()
            .HasOne(x => x.Author).WithMany(x => x.PublicationAuthors)
            .HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Publication>()
            .HasOne(x => x.PublicationType).WithMany(x => x.Publications)
            .HasForeignKey(x => x.PublicationTypeId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Copy>()
            .HasIndex(x => new { x.PublicationId, x.CopyNumber }).IsUnique();
        modelBuilder.Entity<Copy>()
            .HasOne(x => x.Publication).WithMany(x => x.Copies)
            .HasForeignKey(x => x.PublicationId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Copy>()
            .HasOne(x => x.CopyStatus).WithMany(x => x.Copies)
            .HasForeignKey(x => x.CopyStatusId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LibraryRequest>()
            .HasOne(x => x.Reader).WithMany(x => x.Requests)
            .HasForeignKey(x => x.ReaderId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryRequest>()
            .HasOne(x => x.Copy).WithMany(x => x.Requests)
            .HasForeignKey(x => x.CopyId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LibraryRequest>()
            .HasOne(x => x.RequestStatus).WithMany(x => x.Requests)
            .HasForeignKey(x => x.RequestStatusId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FundChange>()
            .HasOne(x => x.Publication).WithMany(x => x.FundChanges)
            .HasForeignKey(x => x.PublicationId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PublicationType>().HasData(
            new PublicationType { Id = 1, Name = "Книга" },
            new PublicationType { Id = 2, Name = "Журнал" },
            new PublicationType { Id = 3, Name = "Навчальний посібник" });
        modelBuilder.Entity<CopyStatus>().HasData(
            new CopyStatus { Id = 1, Name = "Доступний" },
            new CopyStatus { Id = 2, Name = "Виданий" },
            new CopyStatus { Id = 3, Name = "На ремонті" });
        modelBuilder.Entity<RequestStatus>().HasData(
            new RequestStatus { Id = 1, Name = "Нова" },
            new RequestStatus { Id = 2, Name = "Видано" },
            new RequestStatus { Id = 3, Name = "Повернено" },
            new RequestStatus { Id = 4, Name = "Відхилено" });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.CreatedAt).IsModified = false;
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}
