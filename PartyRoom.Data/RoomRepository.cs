using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PartyRoom.Domain;

namespace PartyRoom.Data
{
    public class RoomRepository : IRoomRepository
    {
        private readonly PartyRoomDbContext _context;

        public RoomRepository(PartyRoomDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            var rooms = await _context.Rooms.ToListAsync();
            _context.Rooms.Find();
            return rooms;
        }

        public async Task<Room> GetByIdAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            return room;
        }

        public async Task<Room> AddAsync(Room room)
        {
            _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Room> UpdateAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Room> DeleteAsync(Room room)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return room;
        }
    }
}

