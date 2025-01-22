using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Domain.Entities.Identity;

namespace WhiteLagoon.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Villa> Villas { get; set; }
        public DbSet<VillaNumber> VillaNumbers { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Villa Data Seeds
            var Villa01 = new Villa()
            {
                Id = 1,
                Name = "Royal Villa",
                Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageURL = "Images/Villa/_villa01.jpg",
                Occupancy = 4,
                Price = 200,
                Sqft = 550,
            };

            var Villa02 = new Villa()
            {
                Id = 2,
                Name = "Premium Pool Villa",
                Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageURL = "Images/Villa/_villa02.jpg",
                Occupancy = 4,
                Price = 300,
                Sqft = 550,
            };

            var Villa03 = new Villa()
            {
                Id = 3,
                Name = "Luxury Pool Villa",
                Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageURL = "Images/Villa/_villa03.jpg",
                Occupancy = 4,
                Price = 400,
                Sqft = 750,
            };

            modelBuilder.Entity<Villa>().HasData(Villa01, Villa02,Villa03);

            //Villa Number Data Seeds
            var VN101 = new VillaNumber()
            {
                VillaId = 1,
                Villa_Number = 101,
            };
            var VN102 = new VillaNumber()
            {
                VillaId = 1,
                Villa_Number = 102,
            };
            var VN103 = new VillaNumber()
            {
                VillaId = 1,
                Villa_Number = 103,
            };
            var VN201 = new VillaNumber()
            {
                VillaId = 2,
                Villa_Number = 201,
            };
            var VN202 = new VillaNumber()
            {
                VillaId = 2,
                Villa_Number = 202,
            };
            var VN203 = new VillaNumber()
            {
                VillaId = 2,
                Villa_Number = 203,
            };
            var VN301 = new VillaNumber()
            {
                VillaId = 3,
                Villa_Number = 301,
            };
            var VN302 = new VillaNumber()
            {
                VillaId = 3,
                Villa_Number = 302,
            };
            var VN303 = new VillaNumber()
            {
                VillaId = 3,
                Villa_Number = 303,
            };
            modelBuilder.Entity<VillaNumber>().HasData(VN101, VN102, VN103, VN201, VN202, VN203, VN301, VN302, VN303);

           // Amenities DataSeeds
            modelBuilder.Entity<Amenity>().HasData(
              new Amenity
              {
                  Id = 1,
                  VillaId = 1,
                  Name = "Private Pool"
              }, new Amenity
              {
                  Id = 2,
                  VillaId = 1,
                  Name = "Microwave"
              }, new Amenity
              {
                  Id = 3,
                  VillaId = 1,
                  Name = "Private Balcony"
              }, new Amenity
              {
                  Id = 4,
                  VillaId = 1,
                  Name = "1 king bed and 1 sofa bed"
              },

              new Amenity
              {
                  Id = 5,
                  VillaId = 2,
                  Name = "Private Plunge Pool"
              }, new Amenity
              {
                  Id = 6,
                  VillaId = 2,
                  Name = "Microwave and Mini Refrigerator"
              }, new Amenity
              {
                  Id = 7,
                  VillaId = 2,
                  Name = "Private Balcony"
              }, new Amenity
              {
                  Id = 8,
                  VillaId = 2,
                  Name = "king bed or 2 double beds"
              },

              new Amenity
              {
                  Id = 9,
                  VillaId = 3,
                  Name = "Private Pool"
              }, new Amenity
              {
                  Id = 10,
                  VillaId = 3,
                  Name = "Jacuzzi"
              }, new Amenity
              {
                  Id = 11,
                  VillaId = 3,
                  Name = "Private Balcony"
              });
        }
    }
}
