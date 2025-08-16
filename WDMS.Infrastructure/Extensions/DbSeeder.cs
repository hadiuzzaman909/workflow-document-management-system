using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WDMS.Domain.Entities;
using WDMS.Domain.Enums;

namespace WDMS.Infrastructure.Extensions
{
    public static class DbSeeder
    {
        public static void Seed(WdmsDbContext context)
        {
            // Seed Admin table only if it doesn't already contain data
            if (!context.Admins.Any())
            {
                context.Admins.Add(new Admin
                {
                    // Do not set AdminId, let SQL Server handle it
                    Email = "admin@wdms.com",
                    PasswordHash = new byte[0], // Replace with actual hash
                    PasswordSalt = new byte[0], // Replace with actual salt
                    AccessLevel = AccessLevel.ReadWrite,
                    IsActive = true
                });
                context.SaveChanges();  // Save Admin
            }

            // Seed Workflow table only if it doesn't already contain data
            if (!context.Workflows.Any())
            {
                context.Workflows.Add(new Workflow
                {
                    Name = "Sample Workflow",
                    Type = WorkflowType.Order,
                    CreatedByAdminId = 1,  // Linking to the seeded Admin
                    CreatedAtUtc = DateTime.UtcNow
                });
                context.SaveChanges();  // Save Workflow
            }

            // Seed Document table
            if (!context.Documents.Any())
            {
                var document = new Document
                {

                    DocumentUid = Guid.NewGuid(),
                    Name = "Sample Document",
                    DocumentTypeId = 1,  // Assume this document type exists
                    FilePath = "path/to/document",
                    WorkflowId = 1,  // Linking to seeded Workflow
                    Status = DocumentStatus.Approved,
                    CreatedByAdminId = 1,  // Linking to seeded Admin
                    CreatedAtUtc = DateTime.UtcNow
                };
                context.Documents.Add(document);
                context.SaveChanges();  // Save document after workflows are seeded
            }

        }
    }
}
