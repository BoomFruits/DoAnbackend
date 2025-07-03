using Microsoft.EntityFrameworkCore;
using System;

namespace DoAn.Data
{
    public static class DbBookingContextSeeding
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
          //  var staffId = Guid.Parse("11111111-1111-1111-1111-111111111113");
          //  var customerId = Guid.Parse("11111111-1111-1111-1111-111111111112");

          //  // 3. Seed Categories
          //  modelBuilder.Entity<Category>().HasData(
          //      new Category { Id = 1, Name = "Đồ uống" },
          //      new Category { Id = 2, Name = "Đồ ăn" },
          //      new Category { Id = 3, Name = "Gym & Spa" },
          //      new Category { Id = 4, Name = "Party & Event"}
          //  );

          //  // 4. Seed Products
          //  modelBuilder.Entity<Product>().HasData( 
          //      new Product
          //      {
          //          Id = 3,
          //          CategoryId = 3,
          //          Title = "Xông hơi 60 phút",
          //          Description = "Dịch vụ thư giãn tại Spa",
          //          Price = 250000,
          //          StockQuantity = 10,
          //          Unit = "Buổi",
          //          Thumbnail = "",
          //          CreatedAt = new DateTime(2025, 5, 28),
          //          IsAvailable = true
          //      },
          //          new Product
          //          {
          //              Id = 4,
          //              CategoryId = 4,
          //              Title = "Trang trí sinh nhật",
          //              Description = "Dịch vụ setup tiệc",
          //              Price = 500000,
          //              StockQuantity = 50,
          //              Unit = "Lần",
          //              Thumbnail = "",
          //              CreatedAt = new DateTime(2025, 5, 28),
          //              IsAvailable = false
          //          }
          //  );

          //  // 5. Seed Booking
          //modelBuilder.Entity<Booking>().HasData(
          //      new Booking
          //      {
          //          Id = 5,
          //          StaffId = null,
          //          CustomerId = customerId,
          //          BookingDate = new DateTime(2024, 5, 10),
          //          status = 1,
          //          PaymentMethod = "Paypal",
          //          Note = "Đặt trước 2 ngày"
          //      }
          //  );

            // 6. Seed BookingDetai
            modelBuilder.Entity<BookingDetail>().HasData(
                new BookingDetail
                {
                    BookingId = 5,
                    RoomId = 3,
                    CheckinDate = new DateTime(2024, 5, 12),
                    CheckoutDate = new DateTime(2024, 5, 14),
                    IsCheckedIn = false,
                    IsCheckedOut = false,
                    RoomNote = "Test booking",
                    Price = 300000,
                }
            );
            // Seed ServiceDetail
            modelBuilder.Entity<ServiceDetail>().HasData(
                new ServiceDetail
                {
                    Id = 1,
                    BookingId = 5,
                    ProductId = 3,
                    CustomerId = Guid.Parse("3421EF41-6B25-4AC1-AC32-C3669513B3B4"),
                    Amount = 2,
                    Price = 160000
                },
                new ServiceDetail
                {
                    Id = 2,
                    BookingId = 5,
                    ProductId = 4,
                    CustomerId = Guid.Parse("3421EF41-6B25-4AC1-AC32-C3669513B3B4"),
                    Amount = 1,
                    Price = 210000
                }
            );
            // 8. Seed Rooms
            //modelBuilder.Entity<Room>().HasData(
            //    new Room
            //    {
            //        Id = 1,
            //        Room_No = "101",
            //        Room_Name = "Phòng đơn 1",
            //        Capacity = 1,
            //        Type = "Single",
            //        Price = 300000,
            //        CreatedAt = new DateTime(2024, 1, 1),
            //        IsAvailable = true,
            //        Bed = 1,
            //        Bath = 1,
            //        Area = 25,
            //        description = "Phòng đơn đầy đủ tiện nghi"
            //    },
            //    new Room
            //    {
            //        Id = 2,
            //        Room_No = "102",
            //        Room_Name = "Phòng đôi 2",
            //        Capacity = 2,
            //        Type = "Double",
            //        Price = 500000,
            //        CreatedAt = new DateTime(2024, 1, 2),
            //        IsAvailable = true,
            //        Bed = 2,
            //        Bath = 1,
            //        Area = 30,
            //        description = "Phòng đôi thoáng mát, view đẹp"
            //    }
            //);

            // 9. Seed RoomImages
            //modelBuilder.Entity<RoomImage>().HasData(
            //    new RoomImage { Id = 1, RoomId = 1, ImageUrl = "room1.jpg" },
            //    new RoomImage { Id = 2, RoomId = 2, ImageUrl = "room2.jpg" }
            //);
        }

    }
}
