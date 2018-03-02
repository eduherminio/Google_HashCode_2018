using System;
using System.Linq;
using System.Collections.Generic;

using FileParser;
using FileScriber;

using Project.Model;

namespace Project
{
    public class Manager
    {
        private readonly string _inputPath = "./Samples/";
        private readonly string _outputPath = "./Outputs/";

        private readonly string _inputFileName = "e_high_bonus";

        public Manager()
        {
            string _outputFileName = "output_" + _inputFileName + ".out";
            _inputFileName += ".in";
            _inputPath += _inputFileName;
            _outputPath += _outputFileName;
        }

        public void Run()
        {
            LoadData();

            //ExampleReproduction();
            ProcessData();

            PrintData();
        }

        private void LoadData()
        {
            ParsedFile _file = new ParsedFile(_inputPath);

            IParsedLine firstLine = _file.NextLine();

            long gridRow = firstLine.NextElement<long>();
            long gridCol = firstLine.NextElement<long>();

            Grid = new Grid(gridRow, gridCol);

            long n_vehicles = firstLine.NextElement<long>();

            for (int i = 0; i < n_vehicles; ++i)
                VehicleList.Add(new Vehicle(i));

            long n_rides = firstLine.NextElement<long>();
            long bonus = firstLine.NextElement<long>();
            TotalSimulationSteps = firstLine.NextElement<long>();

            long rideId = 0;
            while (!_file.Empty)
            {
                IParsedLine line = _file.NextLine();

                long row = line.NextElement<long>();
                long col = line.NextElement<long>();


                long endrow = line.NextElement<long>();
                long endcol = line.NextElement<long>();

                long start = line.NextElement<long>();
                long end = line.NextElement<long>();

                RideList.Add(new Ride(
                    rideId,
                    bonus,
                    new Position(row, col),
                    new Position(endrow, endcol),
                    start,
                    end,
                    TotalSimulationSteps));

                ++rideId;
            }

            if (n_rides != RideList.Count)
                throw new ParsingException();

            RideList = RideList.OrderBy(ride => ride.Distance).ToList();
        }

        private void PrintData()
        {
            var file = _outputPath;
            Writer.Clear(file);

            for (int i = 0; i < VehicleList.Count; ++i)
            {
                string line = string.Empty;

                line += VehicleList[i].SuccessfullRides.Count.ToString() + " ";

                VehicleList[i].SuccessfullRides.OrderBy(r => r.RealStart);

                foreach (var ride in VehicleList[i].SuccessfullRides)
                {
                    line += ride.Id;

                    if (ride.Id != VehicleList[i].SuccessfullRides.Last().Id)
                        line += " ";
                }

                if (i != (VehicleList.Count - 1))
                    Writer.WriteLine(file, line);
                else
                    Writer.Write(file, line);
            }
        }

        List<Vehicle> VehicleList { get; set; } = new List<Vehicle>();

        List<Ride> RideList { get; set; } = new List<Ride>();


        long TotalSimulationSteps { get; set; }

        private long CalculateScore()
        {
            long score = 0;
            foreach (var vehicle in VehicleList)
                foreach (var ride in vehicle.SuccessfullRides)
                    score += ride.CalculateScore();

            return score;
        }

        private void ProcessData()
        {
            long currentStep = 0;

            while (currentStep < TotalSimulationSteps)
            {
                // UpdateVehicles
                foreach (var veh in VehicleList.Where(v => v.StepWhenWillBeFee == currentStep).ToList())
                {
                    if (veh.Free == true)
                        throw new Exception();

                    veh.Free = true;
                    veh.StepWhenWillBeFee = -1;
                    veh.RealPosition = veh.SuccessfullRides.Last().EndPosition;
                }

                //VehicleList.OrderBy(v => v.CalculateDistanceToAPoint(RideList.First().InitialPosition));
                Random rnd = new Random();
                foreach (Vehicle v in VehicleList.Where(v => v.Free).ToList())
                {
                    long remainingRides = RideList.Count;
                    if (remainingRides == 0)
                        break;
                    Ride ride = RideList[rnd.Next(0, RideList.Count - 1)];  // Room for improvement
                    if(ride != null && true == ride.IsOnTimeOrAfterEarlyStart(currentStep + v.CalculateDistanceToAPoint(ride.InitialPosition)))
                    {
                        ride.Distance += v.CalculateDistanceToAPoint(ride.InitialPosition);
                        ride.Done = true;
                        ride.DoneInEarlyStart = currentStep == ride.EarlyStart;     // Room for improvement
                        v.Free = false;
                        v.StepWhenWillBeFee = currentStep + ride.Distance;  // Including trip to position

                        RideList.Remove(ride);
                        v.SuccessfullRides.Add(ride);
                    }
                }
                currentStep++;
            }
        }
    }
}
