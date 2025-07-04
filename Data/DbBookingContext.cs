using Microsoft.EntityFrameworkCore;

namespace DoAn.Data
{
    public partial class DbBookingContext : DbContext
    {
        public DbBookingContext(DbContextOptions<DbBookingContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<ServiceDetail> ServiceDetail { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //user - customerbookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(u => u.CustomerBookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            //user - staffbookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Staff)
                .WithMany(u => u.StaffBookings)
                .HasForeignKey(b => b.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
            //Room - RoomImage cascade delete
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Images)
                .WithOne(i => i.Room)
                .HasForeignKey(i => i.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            // User-Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict); 

            // BookingDetail: composite key
            modelBuilder.Entity<BookingDetail>()
                .HasKey(bd => new { bd.BookingId, bd.RoomId });

            modelBuilder.Entity<BookingDetail>()
                .HasOne(bd => bd.Booking)
                .WithMany(b => b.BookingDetails)
                .HasForeignKey(bd => bd.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingDetail>()
                .HasOne(bd => bd.Room)
                .WithMany(r => r.BookingDetails)
                .HasForeignKey(bd => bd.RoomId);


            // Product - Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);
            modelBuilder.Entity<Category>()
                .HasMany(p => p.Products)
                .WithOne(c => c.Category)
                .OnDelete(DeleteBehavior.Cascade);

            // Service

            modelBuilder.Entity<ServiceDetail>()
                .HasOne(sd => sd.BookingDetail)
                .WithMany()
                .HasForeignKey(s => new {s.BookingId,s.RoomId})
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<ServiceDetail>()
                .HasOne(sd => sd.Customer)
                .WithMany()
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceDetail>()
                .HasOne(sd => sd.Product)
                .WithMany()
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            DbBookingContextSeeding.Seed(modelBuilder);
        }
        
    }
}
