using Medicines.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Medicines.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Pharmacics> Pharmacies { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderMedicine> OrderMedicines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔹 تعريف جدول `Users`
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PhoneNumber).HasMaxLength(15);
            });

            // 🔹 تعريف جدول `Pharmacies`
            modelBuilder.Entity<Pharmacics>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
                entity.Property(p => p.Address).IsRequired().HasMaxLength(250);
                entity.Property(p => p.City).HasMaxLength(100);
                entity.Property(p => p.Latitude).HasPrecision(10, 7);
                entity.Property(p => p.Longitude).HasPrecision(10, 7);
                entity.Property(p => p.LicenseNumber).IsRequired().HasMaxLength(50);
                entity.Property(p => p.PharmacistName).HasMaxLength(100);
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
            });

            // 🔹 تعريف جدول `Orders`
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.dateTime).HasColumnType("datetime2(7)");
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

            base.OnModelCreating(modelBuilder);
        }
    }


}
