using Medicines.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Data;

namespace Medicines.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Users>? Users { get; set; }
        public DbSet<Pharmacics>? Pharmacies { get; set; }
        public DbSet<Medicine>? Medicines { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<OrderMedicine>? OrderMedicines { get; set; }
        public DbSet<Roles>? Roles { get; set; }
        public DbSet<Practitioner>? Practitioners { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔹 تعريف جدول `Users`
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(10);

                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict); // لمنع حذف الأدوار المرتبطة بمستخدمين
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
            });

            // 🔹 تعريف جدول `Practitioner`
            modelBuilder.Entity<Practitioner>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.NamePractitioner).IsRequired().HasMaxLength(75);
                entity.Property(p => p.Studies).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Address).HasMaxLength(100);
                entity.Property(p => p.PhonNumber).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Pharmacics>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
                entity.Property(p => p.Address).IsRequired().HasMaxLength(250);
                entity.Property(p => p.City).HasMaxLength(100);
                entity.Property(p => p.Latitude).HasPrecision(10, 7);
                entity.Property(p => p.Longitude).HasPrecision(10, 7);
                entity.Property(p => p.LicenseNumber).IsRequired().HasMaxLength(50);


                // 🔹 تعريف العلاقة One-to-One
                entity.HasOne(p => p.Practitioner)
                      .WithOne(pr => pr.Pharmacy)
                      .HasForeignKey<Pharmacics>(p => p.PractitionerId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict); // منع حذف الطبيب عند حذف الصيدلية

                entity.HasMany(p => p.Medicines)
                      .WithOne(m => m.Pharmacy)
                      .HasForeignKey(m => m.PharmacyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 🔹 تعريف جدول `Medicines`
            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.TradeName).IsRequired().HasMaxLength(150);
                entity.Property(m => m.ScientificName).IsRequired().HasMaxLength(150);
                entity.Property(m => m.Dosage).HasPrecision(6, 2);
                entity.Property(m => m.DrugTiming).HasMaxLength(100);
                entity.Property(m => m.SideEffects).HasMaxLength(500);
                entity.Property(m => m.ContraindicatedDrugs).HasMaxLength(500);
                entity.Property(m => m.ManufacturerName).HasMaxLength(150);
                entity.Property(m => m.ProducingCompany).HasMaxLength(150);
                entity.Property(m => m.Price).HasPrecision(10, 2);

                entity.Property(m => m.PharmacyId).IsRequired();
            });

            // 🔹 تعريف جدول `Orders`
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
             
                entity.Property(o => o.FinalPrice).HasPrecision(10, 2);

                // العلاقة بين `Order` و `User`
                entity.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // ✅ تجنب `Multiple Cascade Paths`

                // العلاقة بين `Order` و `Pharmacy`
                entity.HasOne(o => o.Pharmacy)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(o => o.PharmacyId)
                    .OnDelete(DeleteBehavior.Restrict); // ✅ تجنب `Multiple Cascade Paths`
            });

            // 🔹 تعريف العلاقة `Many-to-Many` بين `Order` و `Medicine`
            modelBuilder.Entity<OrderMedicine>()
                .HasKey(om => new { om.OrderId, om.MedicineId });

            modelBuilder.Entity<OrderMedicine>()
                .HasOne(om => om.Order)
                .WithMany(o => o.OrderMedicines)
                .HasForeignKey(om => om.OrderId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ تجنب `Multiple Cascade Paths`

            modelBuilder.Entity<OrderMedicine>()
                .HasOne(om => om.Medicine)
                .WithMany(m => m.OrderMedicines)
                .HasForeignKey(om => om.MedicineId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ تجنب `Multiple Cascade Paths`


            modelBuilder.Entity<Roles>().HasData(
                new Roles { Id = 1, Name = "User" },
                new Roles { Id = 2, Name = "Admin" },
                new Roles { Id = 3, Name = "Practitioner" }
            );


            modelBuilder.Entity<Users>().HasData(
                new Users { Id = 1, Name = "Admin", Password = "12345", IsDleted = false, RoleId = 2 }


            );

     


            base.OnModelCreating(modelBuilder);
        }

    }


}
