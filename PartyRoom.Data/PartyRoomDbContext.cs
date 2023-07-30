using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PartyRoom.Domain;

namespace PartyRoom.Data
{
    public class PartyRoomDbContext : DbContext
    {
        public PartyRoomDbContext(DbContextOptions<PartyRoomDbContext> options) : base(options) {}
        public DbSet<Movement>? Movements { get; set; }
        public DbSet<Room>? Rooms { get; set; }

        private Movement.MovementFactory _factory = new Movement.MovementFactory();

        public PartyRoomDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Movement seedMovementOne = _factory.CreateMovement(1, 1, "12345678901", MovementType.In, DateTime.Now);
            Movement seedMovementTwo = _factory.CreateMovement(2, 1, "12345678902", MovementType.In, DateTime.Now);

            modelBuilder.Entity<Movement>().HasData(seedMovementOne, seedMovementTwo);

            modelBuilder.Entity<Room>().HasData(
                new Room()
                    {
                        Id = 1,
                        Attendees = 2,
                        MaxOccupancy = 100
                    },
                new Room
                {
                        Id = 2,
                        Attendees = 7,
                        MaxOccupancy = 200
                    }
                );
                
            base.OnModelCreating(modelBuilder);
        }
    }
}
