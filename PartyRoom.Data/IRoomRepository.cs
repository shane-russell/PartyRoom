using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyRoom.Domain;

namespace PartyRoom.Data
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllAsync();
        Task<Room> GetByIdAsync(int id);
        Task<Room> AddAsync(Room room);
        Task<Room> UpdateAsync(Room room);
        Task<Room> DeleteAsync(Room room);
    }
}
