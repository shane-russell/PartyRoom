using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using movement.Data;
using PartyRoom.Domain;

namespace PartyRoom.Data
{
    public class MovementRepository : IMovementRepository
    {
        private readonly PartyRoomDbContext _context;

        public MovementRepository(PartyRoomDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Movement> GetMovementsOfToday()
        {
            var today = DateTime.Today;

            var movements = _context.Movements.Where(m => m.Time >= today && m.Time < today.AddDays(1)).OrderBy(m => m.Time).ToList();
            return movements;

            // (00:00incl->11:59incl) sorted chronologically
        }

        public IEnumerable<Movement> GetAll()
        {
            var movements = _context.Movements.ToList();
            return movements;
        }

        public Movement GetById(int id)
        {
            var movement = _context.Movements.Find(id);
            return movement;
        }

        public void Add(Movement movement)
        {
            _context.Movements.Add(movement);
            _context.SaveChanges();
        }

        public void Update(Movement movement)
        {
            _context.Movements.Update(movement);
            _context.SaveChangesAsync();
        }

        public void Delete(Movement movement)
        {
            _context.Movements.Remove(movement);
            _context.SaveChangesAsync();
        }
    }
}
