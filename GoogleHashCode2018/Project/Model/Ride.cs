using System;

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

        public long RealStart { get; set; }

        public bool Done { get; set; }

        public bool DoneInEarlyStart { get; set; }

        public long Bonus { get; set; }

        public long TotalSimulationSteps { get; set; }


        public Ride(long id, long bonus, Position init, Position endpos, long start, long end, long totalsteps)
        {
            Id = id;
            Bonus = bonus;
            InitialPosition = init;
            EndPosition = endpos;
            TotalSimulationSteps = totalsteps;

            EarlyStart = start;
            LatestEnd = end;

            Distance = Math.Abs(InitialPosition.X - EndPosition.X) + Math.Abs(InitialPosition.Y - EndPosition.Y);

            if (Distance <= 1)
                throw new Exception();

            Done = false;
            DoneInEarlyStart = false;
        }

        public long CalculateScore()
        {
            long realBonus = DoneInEarlyStart
                ? Bonus
                : 0;

            return Done
                ? Distance + realBonus
                : 0;
        }


        public bool IsOnTimeOrAfterEarlyStart(long startStep)
        {
            bool result = true;

            if (startStep + this.Distance > this.TotalSimulationSteps)
                result = false;

            if (startStep + this.Distance > this.LatestEnd)
                result = false;

            if (startStep < this.EarlyStart)
                result = false;

            return result;
        }
    }
}
