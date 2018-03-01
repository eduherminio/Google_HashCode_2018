using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Model
{
    public class Vehicle
    {
        public Vehicle(long id, Position pos = null)
        {
            Id = id;
            RealPosition = pos ?? new Position(0, 0);
            Free = true;
        }
        public long Id { get; set; }
        public Position RealPosition { get; set; }

        public bool Free { get; set; }
        public List<Ride> SuccessfullRides { get; set; } = new List<Ride>();

        public long CalculateDistanceToAPoint(Position position)
        {
            return Math.Abs(RealPosition.X - position.X) + Math.Abs(RealPosition.Y - position.Y);
        }
    }
}
