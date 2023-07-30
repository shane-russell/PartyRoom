using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyRoom.Domain
{
    public interface IMovementFactory
    {
        public Movement CreateMovement(int Id, int PartyRoomId, string NationalNumber, MovementType movementType,
            DateTime Time);
    }
}
