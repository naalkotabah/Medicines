using Medicines.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Medicines.Data
{
   

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Pharmacics> Pharmacies { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         
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

                // 🔹 علاقة الصيدلية بالأدوية (صيدلية تحتوي على عدة أدوية)
                entity.HasMany(p => p.Medicines)
                      .WithOne(m => m.Pharmacy)
                      .HasForeignKey(m => m.PharmacyId)
                      .OnDelete(DeleteBehavior.Cascade); 
            });

    
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

            base.OnModelCreating(modelBuilder);
        }

    }


}
