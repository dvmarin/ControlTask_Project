using ControlTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlTask.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Developer> Developers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Esquema
            modelBuilder.HasDefaultSchema("app");

            // Configurar tablas
            modelBuilder.Entity<Developer>(entity =>
            {
                entity.ToTable("Developers");
                entity.HasKey(e => e.DeveloperId);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Projects");
                entity.HasKey(e => e.ProjectId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ClientName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Status)
                      .HasDefaultValue("Planned")
                      .HasConversion<string>();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("Tasks");
                entity.HasKey(e => e.TaskId);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Status)
                      .HasDefaultValue("ToDo")
                      .HasConversion<string>();
                entity.Property(e => e.Priority)
                      .HasDefaultValue("Medium")
                      .HasConversion<string>();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

                // Validaciones
                entity.HasCheckConstraint("CK_Tasks_EstimatedComplexity",
                    "[EstimatedComplexity] BETWEEN 1 AND 5 OR [EstimatedComplexity] IS NULL");

                // Relaciones
                entity.HasOne(t => t.Project)
                      .WithMany(p => p.Tasks)
                      .HasForeignKey(t => t.ProjectId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Assignee)
                      .WithMany(d => d.Tasks)
                      .HasForeignKey(t => t.AssigneeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
