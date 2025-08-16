using Microsoft.EntityFrameworkCore;
using WDMS.Domain.Entities;

namespace WDMS.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowAdmin> WorkflowAdmins { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Admin entity
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.AdminId);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PasswordHash)
                    .IsRequired();

                entity.Property(e => e.AccessLevel)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                // Create unique index on email for non-deleted records
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasFilter("[IsDeleted] = 0");

                // Add indexes for performance
                entity.HasIndex(e => e.IsDeleted);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => new { e.Email, e.IsDeleted, e.IsActive });
            });


            // Configure DocumentType entity
            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.HasKey(e => e.DocumentTypeId);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                // Create unique index on name for non-deleted records
                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasFilter("[IsDeleted] = 0");
            });

            // Configure Admin entity
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.AdminId);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PasswordHash)
                    .IsRequired();

                entity.Property(e => e.AccessLevel)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                // Create unique index on email for non-deleted records
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasFilter("[IsDeleted] = 0");
            });

            // Configure Workflow entity
            modelBuilder.Entity<Workflow>(entity =>
            {
                entity.HasKey(e => e.WorkflowId);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                // Add foreign key for CreatedByAdminId
                entity.HasOne(e => e.CreatedByAdmin)  // Navigating to Admin entity
                    .WithMany()  // One admin can create multiple workflows
                    .HasForeignKey(e => e.CreatedByAdminId)
                    .OnDelete(DeleteBehavior.Restrict); // Don't allow cascading delete
            });

            // Configure WorkflowAdmin entity (many-to-many relationship between Workflow and Admin)
            modelBuilder.Entity<WorkflowAdmin>()
                .HasKey(e => new { e.WorkflowId, e.AdminId });

            modelBuilder.Entity<WorkflowAdmin>()
                .HasOne(e => e.Workflow)
                .WithMany(w => w.WorkflowAdmins)
                .HasForeignKey(e => e.WorkflowId);

            modelBuilder.Entity<WorkflowAdmin>()
                .HasOne(e => e.Admin)
                .WithMany(a => a.WorkflowAdmins)
                .HasForeignKey(e => e.AdminId);
        }
    }
}