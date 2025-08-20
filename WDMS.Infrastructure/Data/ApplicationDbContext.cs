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

        public DbSet<Document> Documents { get; set; } = null!;
        public DbSet<TaskAssignment> TaskAssignments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


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

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasFilter("[IsDeleted] = 0");


                entity.HasIndex(e => e.IsDeleted);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => new { e.Email, e.IsDeleted, e.IsActive });
            });


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

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasFilter("[IsDeleted] = 0");
            });


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

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasFilter("[IsDeleted] = 0");
            });

            modelBuilder.Entity<Workflow>()
                        .HasOne(w => w.CreatedByAdmin)
                        .WithMany()
                        .HasForeignKey(w => w.CreatedByAdminId)
                        .IsRequired(false)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Workflow>().Property(w => w.Name).HasMaxLength(255);
            modelBuilder.Entity<Workflow>().HasQueryFilter(w => !w.IsDeleted);

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

            // Document
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(d => d.DocumentId);

                entity.HasIndex(d => d.DocumentUid).IsUnique();

                entity.Property(d => d.Name).IsRequired().HasMaxLength(255);
                entity.Property(d => d.FilePath).IsRequired().HasMaxLength(1024);
                entity.Property(d => d.OriginalFileName).HasMaxLength(255);
                entity.Property(d => d.ContentType).HasMaxLength(127);

                entity.Property(d => d.CreatedAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(d => d.IsDeleted)
                      .IsRequired()
                      .HasDefaultValue(false);

                entity.HasOne(d => d.DocumentType)
                      .WithMany(dt => dt.Documents)
                      .HasForeignKey(d => d.DocumentTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Workflow)
                      .WithMany(w => w.Documents)
                      .HasForeignKey(d => d.WorkflowId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.CreatedByAdmin)
                      .WithMany()
                      .HasForeignKey(d => d.CreatedByAdminId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}