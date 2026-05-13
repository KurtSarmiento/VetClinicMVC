using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VetClinicMVC.Models
{
    public partial class VetClinicContext : IdentityDbContext<IdentityUser>
    {
        public VetClinicContext()
        {
        }

        public VetClinicContext(DbContextOptions<VetClinicContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<Breed> Breeds { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Medication> Medications { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Pet> Pets { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }
        public virtual DbSet<Species> Species { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<SystemLog> SystemLogs { get; set; }
        public virtual DbSet<Visit> Visits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=VetClinicDB;Trusted_Connection=True;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC2C8CF7714");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
                entity.Property(e => e.DurationMinutes).HasDefaultValue(30);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Scheduled");
                entity.HasOne(d => d.Client).WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.ClientId).OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.Pet).WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.PetId).OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.Staff).WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.StaffId).OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.LogId).HasName("PK__AuditLog__5E54864826CD81B1");
                entity.HasIndex(e => e.Timestamp, "IX_AuditLogs_Timestamp").IsDescending();
                entity.HasIndex(e => e.UserId, "IX_AuditLogs_UserId");
                entity.HasIndex(e => new { e.EntityName, e.EntityId }, "IX_AuditLogs_Entity");
            });

            modelBuilder.Entity<Breed>(entity =>
            {
                entity.HasKey(e => e.BreedId).HasName("PK__Breeds__D1E9AE9D489704D8");
                entity.HasOne(d => d.Species).WithMany(p => p.Breeds)
                    .HasForeignKey(d => d.SpeciesId).OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.ClientId).HasName("PK__Clients__E67E1A24B9E1E4CE");
                entity.HasIndex(e => e.Email, "UQ__Clients__A9D10534BD002ED2").IsUnique();
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAB5655C76F6");
                entity.HasIndex(e => e.InvoiceNumber, "UQ__Invoices__D776E98133DF6D98").IsUnique();
            });

            modelBuilder.Entity<Medication>(entity =>
            {
                entity.HasKey(e => e.MedicationId);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
            });

            modelBuilder.Entity<Pet>(entity =>
            {
                entity.HasKey(e => e.PetId).HasName("PK__Pets__48E53862C763279D");
                entity.HasIndex(e => e.MicrochipId, "UQ__Pets__4637B1CC91F45455").IsUnique();
                entity.HasOne(d => d.Client).WithMany(p => p.Pets)
                    .HasForeignKey(d => d.ClientId);
                entity.HasOne(d => d.Species).WithMany(p => p.Pets)
                    .HasForeignKey(d => d.SpeciesId).OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB00A770D7615");
                entity.HasOne(d => d.Category).WithMany(p => p.Services)
                    .HasForeignKey(d => d.CategoryId).OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ServiceCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId).HasName("PK__ServiceC__19093A0BDBFA0B76");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(e => e.Description)
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Species>(entity =>
            {
                entity.HasKey(e => e.SpeciesId);
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.StaffId).HasName("PK__Staff__96D4AB17EFBF3109");
                entity.HasIndex(e => e.UserId, "UQ__Staff__1788CC4DF80130E1").IsUnique();
            });

            modelBuilder.Entity<SystemLog>(entity =>
            {
                entity.HasKey(e => e.LogId);
            });

            modelBuilder.Entity<Visit>(entity =>
            {
                entity.HasKey(e => e.VisitId).HasName("PK__Visits__4D3AA1DEA49F82AA");
                entity.HasOne(d => d.Pet).WithMany(p => p.Visits)
                    .HasForeignKey(d => d.PetId).OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.Staff).WithMany(p => p.Visits)
                    .HasForeignKey(d => d.StaffId).OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}