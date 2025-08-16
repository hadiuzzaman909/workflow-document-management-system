using Microsoft.EntityFrameworkCore;
using WDMS.Domain.Entities;

namespace WDMS.Infrastructure
{
    public class WdmsDbContext : DbContext
    {
        public WdmsDbContext(DbContextOptions<WdmsDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<DocumentApproval> DocumentApprovals { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowAdmin> WorkflowAdmins { get; set; }
        public DbSet<DocumentReviewCycle> DocumentReviewCycles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentApproval>()
                .HasKey(da => da.ApprovalId);

            modelBuilder.Entity<DocumentApproval>()
                .HasOne(da => da.Admin)
                .WithMany()
                .HasForeignKey(da => da.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DocumentApproval>()
                .HasOne(da => da.DocumentReviewCycle)
                .WithMany()
                .HasForeignKey(da => da.CycleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Workflow)
                .WithMany()
                .HasForeignKey(d => d.WorkflowId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.CreatedByAdmin)
                .WithMany()
                .HasForeignKey(d => d.CreatedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DocumentReviewCycle>()
                .HasKey(drc => drc.CycleId);

            modelBuilder.Entity<DocumentReviewCycle>()
                .HasOne(drc => drc.Document)
                .WithMany()
                .HasForeignKey(drc => drc.DocumentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkflowAdmin>()
                .HasKey(wa => new { wa.WorkflowId, wa.AdminId });  

            modelBuilder.Entity<WorkflowAdmin>()
                .HasOne(wa => wa.Workflow)
                .WithMany()
                .HasForeignKey(wa => wa.WorkflowId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkflowAdmin>()
                .HasOne(wa => wa.Admin)
                .WithMany()
                .HasForeignKey(wa => wa.AdminId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}