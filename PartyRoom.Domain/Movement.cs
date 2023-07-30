using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartyRoom.Domain;

namespace PartyRoom.Domain
{
    public class Movement
    {
        public Movement()
        {
        }
        public int Id { get; set; }
        public int PartyRoomId { get; set; }

        public string? NationalNumber { get; set; }

        public MovementType MovementType { get; set; } // IN or OUT

        public DateTime Time { get; set; }


        public class MovementFactory : IMovementFactory
        {
            public Movement CreateMovement(int Id, int PartyRoomId, string NationalNumber, MovementType movementType, DateTime Time)
            {
                if (string.IsNullOrEmpty(NationalNumber))
                {
                    throw new ArgumentNullException("NationalNumber cannot be empty");}

                return new Movement() { Id = Id, PartyRoomId = PartyRoomId, NationalNumber = NationalNumber, Time = Time };
            }
        }
    }
}