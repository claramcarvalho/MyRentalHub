using Microsoft.EntityFrameworkCore;
using RentalProperties.Models;

namespace RentalProperties.DATA
{
    public class RentalPropertiesDBContext : DbContext
    {
        public RentalPropertiesDBContext(DbContextOptions<RentalPropertiesDBContext> options) : base(options)
        {
           
        }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<MessageFromTenant> MessagesFromTenants { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<EventInProperty> EventsInProperties { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<ManagerSlot> ManagerSlots { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<UserAccount>()
                .Property(x => x.UserType)
                .HasConversion<int>();

            modelBuilder.Entity<UserAccount>()
                .Property(x => x.UserStatus)
                .HasConversion<int>();*/

            modelBuilder.Entity<Property>( entity =>
            {
                entity.HasOne(p => p.Manager).WithMany()
                    .HasForeignKey(u => u.ManagerId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            
            modelBuilder.Entity<Apartment>( entity =>
            {
                entity.Property(e => e.ApartmentNumber)
                .HasMaxLength(10)
                .IsUnicode(false);

                entity.Property(e => e.PriceAnnounced).HasColumnType("numeric(8, 2)");

                entity.HasOne(d => d.Property).WithMany(p => p.Apartments)
                    .HasForeignKey(d => d.PropertyId);
            });

            modelBuilder.Entity<MessageFromTenant>(entity =>
            {
                entity.Property(e => e.AnswerFromManager).HasColumnType("text");
                entity.Property(e => e.MessageSent).HasColumnType("text");

                entity.HasOne(d => d.Apartment).WithMany(m => m.Messages)
                    .HasForeignKey(d => d.ApartmentId);

                entity.HasOne(d => d.Tenant).WithMany(m => m.Messages)
                    .HasForeignKey(d => d.TenantId);
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasOne(d => d.Apartment).WithMany(m => m.Appointments)
                    .HasForeignKey(d => d.ApartmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Tenant).WithMany(a => a.Appointments)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<EventInProperty>(entity =>
            {
                entity.Property(e => e.EventDescription).HasColumnType("text");
                
                entity.Property(e => e.EventTitle)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.Apartment).WithMany(p => p.EventsInApartment)
                    .HasForeignKey(d => d.ApartmentId);

                entity.HasOne(d => d.Property).WithMany(p => p.EventsInProperty)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Rental>(entity =>
            {
                entity.Property(e => e.PriceRent).HasColumnType("numeric(8, 2)");

                entity.HasOne(d => d.Apartment).WithMany(p => p.Rentals)
                    .HasForeignKey(d => d.ApartmentId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Tenant).WithMany(p => p.Rentals)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ManagerSlot>(entity =>
            {
                entity.HasKey(d => d.SlotId);

                entity.Property(d=>d.SlotId).UseIdentityColumn();

                entity.HasOne(d => d.Manager).WithMany(p => p.ManagerSlots)
                    .HasForeignKey(d => d.ManagerId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
