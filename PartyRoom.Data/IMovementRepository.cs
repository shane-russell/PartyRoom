using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyRoom.Domain;

namespace movement.Data
{
    public interface IMovementRepository
    {
        IEnumerable<Movement> GetMovementsOfToday();
        IEnumerable<Movement> GetAll();
        Movement GetById(int id);
        void Add(Movement movement);
        void Update(Movement movement);
        void Delete(Movement movement);
    }
}
