using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Model
{
    public class Trip
    {
        public Vehicle Vehicle { get; set; }
        public Ride Ride { get; set; }

        public Trip(Vehicle veh, Ride ride)
        {
            Vehicle = veh;
            Ride = ride;
        }
    }
}
