using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Model
{
    public class Ride
    {
        public long Id { get; set; }

        public Position InitialPosition { get; set; }
        public Position EndPosition { get; set; }

        public long Distance { get; set; }

        public long EarlyStart { get; set; }
        public long LatestEnd { get; set; }

        public bool Done { get; set; }

        public Ride(long id, Position init, Position endpos, long start, long end)
        {
            Id = id;
            InitialPosition = init;
            EndPosition = endpos;

            EarlyStart = start;
            LatestEnd = end;

            Distance = Math.Abs(InitialPosition.X - EndPosition.X) + Math.Abs(InitialPosition.Y - EndPosition.Y);
            Done = false;
        }
    }
}
