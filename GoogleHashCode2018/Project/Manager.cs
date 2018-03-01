using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using FileParser;
using FileScriber;
using Project.Model;

namespace Project
{
    public class Manager
    {
        private readonly string _inputPath = "./Samples/";
        private readonly string _outputPath = "./Outputs/";

        private readonly string _inputFileName = "a_example";
        private readonly string _outputFileName = null;

        private ParsedFile _file;

        public Manager()
        {
            _outputFileName = "output_" + _inputFileName + ".out";
            _inputFileName += ".in";
            _inputPath += _inputFileName;
            _outputPath += _outputFileName;
        }

        public void Run()
        {
            LoadData();

            ProcessData();

            PrintData();
        }

        private void LoadData()
        {
            _file = new ParsedFile(_inputPath);

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
                    end));

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
        Grid Grid { get; set; }
        long TotalSimulationSteps { get; set; }

        List<Trip> DoneTrips = new List<Trip>();


        private void ExampleReproduction()
        {
            VehicleList.First().SuccessfullRides.Add(RideList.SingleOrDefault(ride => ride.Id == 0));
            VehicleList.First().SuccessfullRides.First().Done = true;
            VehicleList.First().SuccessfullRides.First().DoneInEarlyStart = true;

            VehicleList.Last().SuccessfullRides.Add(RideList.SingleOrDefault(ride => ride.Id == 2));
            VehicleList.Last().SuccessfullRides.First().Done = true;
            VehicleList.Last().SuccessfullRides.Add(RideList.SingleOrDefault(ride => ride.Id == 1));
            VehicleList.Last().SuccessfullRides.Last().Done = true;

            long bonus = CalculateScore();
        }

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
            //ExampleReproduction();



            Simulation();


            // Update vehicle pos after each ride
        }

        private void Simulation()
        {
            int currenStep = 0;

            while (currenStep < TotalSimulationSteps)
            {




                currenStep++;
            }
        }

        private void Algorithm()
        {
            foreach (var vechile in VehicleList.Where(veh => veh.Free))
            {
                RideList.First().
            }
        }
    }
}
