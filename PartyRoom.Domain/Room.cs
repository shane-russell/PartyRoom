using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyRoom.Domain
{
    public class Room
    {
        public int Id { get; set; }
        public int MaxOccupancy { get; set; }
        public int Attendees
        {
            get
            {
                return Attendees;
            }
            set
            {
                if (value > MaxOccupancy) throw new MaxOccupancyException("Sorry, the party room was already full");
                MaxOccupancy = value;
            }
        }
    }
}
