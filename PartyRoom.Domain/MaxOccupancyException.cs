﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PartyRoom.Domain
{
    public class MaxOccupancyException : Exception
    {
        public MaxOccupancyException(string message) : base(message)
        {
            
        }
    }
}
