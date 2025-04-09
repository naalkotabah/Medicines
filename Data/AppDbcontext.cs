using Medicines.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Concurrent;
using System.Data;

namespace Medicines.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Users>? Users { get; set; } = default!;
        public DbSet<Pharmacics>? Pharmacies { get; set; } = default!;
        public DbSet<Medicine>? Medicines { get; set; } = default!;
        public DbSet<Order>? Orders { get; set; } = default!;
        public DbSet<OrderMedicine>? OrderMedicines { get; set; } = default!;
        public DbSet<Roles>? Roles { get; set; } = default!;
        public DbSet<Practitioner>? Practitioners { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.LastName)
                
                      .HasMaxLength(50);

                entity.Property(u => u.UserName)
                     
                      .HasMaxLength(50);

                entity.Property(u => u.Password)
                      .IsRequired()
                      .HasMaxLength(10);

              
                entity.HasCheckConstraint("CK_User_Password_ValidChars", "\"Password\" !~ '[^a-zA-Z0-9]'");


                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict); // يمنع حذف الدور إذا كان مرتبطًا بمستخدم
            });


            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
            });

        
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


        
                entity.HasOne(p => p.Practitioner)
                      .WithOne(pr => pr.Pharmacy)
                      .HasForeignKey<Pharmacics>(p => p.PractitionerId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict); 

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

                entity.Property(m => m.PharmacyId).IsRequired();
            });

        
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
             
                entity.Property(o => o.FinalPrice).HasPrecision(10, 2);

         
                entity.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict); 

         
                entity.HasOne(o => o.Pharmacy)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(o => o.PharmacyId)
                    .OnDelete(DeleteBehavior.Restrict); 
            });

     
            modelBuilder.Entity<OrderMedicine>()
                .HasKey(om => new { om.OrderId, om.MedicineId });

            modelBuilder.Entity<OrderMedicine>()
                .HasOne(om => om.Order)
                .WithMany(o => o.OrderMedicines)
                .HasForeignKey(om => om.OrderId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<OrderMedicine>()
                .HasOne(om => om.Medicine)
                .WithMany(m => m.OrderMedicines)
                .HasForeignKey(om => om.MedicineId)
                .OnDelete(DeleteBehavior.Restrict); 


            modelBuilder.Entity<Roles>().HasData(
                new Roles { Id = 1, Name = "User" },
                new Roles { Id = 2, Name = "Admin" },
                new Roles { Id = 3, Name = "Practitioner" }
            );


            modelBuilder.Entity<Users>().HasData(
                new Users { Id = 1, Name = "Admin", Password = "12345", UserName= "Admin", LastName = "Admin", IsDleted = false, RoleId = 2 }


            );

     


            base.OnModelCreating(modelBuilder);
        }

    }


}
