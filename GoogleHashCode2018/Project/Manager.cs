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
        // Files data
        private readonly string _inputPath = "./Samples/";
        private readonly string _outputPath = "./Outputs/";
        private readonly string _inputFileName = "e_high_bonus";

        static Random _rnd = new Random();

        // Problem data
        List<Vehicle> VehicleList { get; set; } = new List<Vehicle>();
        List<Ride> RideList { get; set; } = new List<Ride>();
        long TotalSimulationSteps { get; set; }

        public Manager(string inputFileName = null)
        {
            if (inputFileName != null)
                _inputFileName = inputFileName;

            string _outputFileName = "output_" + _inputFileName + ".out";
            _inputFileName += ".in";
            _inputPath += _inputFileName;
            _outputPath += _outputFileName;
        }

        public void ExampleReproduction()
        {
            LoadData();

            VehicleList.First().SuccessfullRides.Add(RideList.SingleOrDefault(ride => ride.Id == 0));
            VehicleList.First().SuccessfullRides.First().Done = true;
            VehicleList.First().SuccessfullRides.First().DoneInEarlyStart = true;

            VehicleList.Last().SuccessfullRides.Add(RideList.SingleOrDefault(ride => ride.Id == 2));
            VehicleList.Last().SuccessfullRides.First().Done = true;
            VehicleList.Last().SuccessfullRides.Add(RideList.SingleOrDefault(ride => ride.Id == 1));
            VehicleList.Last().SuccessfullRides.Last().Done = true;

            PrintData();
        }
        public void Run()
        {
            LoadData();

            ProcessData();

            PrintData();
        }

        private void LoadData()
        {
            ParsedFile _file = new ParsedFile(_inputPath);

            IParsedLine firstLine = _file.NextLine();

            /*long gridRow = */
            firstLine.NextElement<long>();
            /*long gridCol = */
            firstLine.NextElement<long>();

            //Grid = new Grid(gridRow, gridCol);

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
        }

        private void PrintData()
        {
            Console.WriteLine(_outputPath + " is ready!");
            var file = _outputPath;
            Writer.Clear(file);

            for (int i = 0; i < VehicleList.Count; ++i)
            {
                string line = string.Empty;

                line += VehicleList[i].SuccessfullRides.Count.ToString() + " ";

                VehicleList[i].SuccessfullRides = VehicleList[i].SuccessfullRides.OrderBy(r => r.RealStart).ToList();

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


        private void ProcessData()
        {
            RideList = RideList.OrderBy(ride => ride.Distance).ToList();    // Room for improvement

            long currentStep = 0;

            while (currentStep < TotalSimulationSteps)
            {
                // Update Vehicle State
                UpdateVehicleState(currentStep);

                Ride optimalRideForCurrentStep = GetOptimalRide(currentStep);

                if (optimalRideForCurrentStep != null)
                    VehicleList = VehicleList.OrderBy(v => v.CalculateDistanceToAPoint(optimalRideForCurrentStep.InitialPosition)).ToList();
                // Possible improvement: always provide an optimalRideAlternative and a Vehicle Ordered list according to previous one

                foreach (Vehicle v in VehicleList.Where(v => v.Free).ToList())
                {
                    if (!RideList.Any())
                        break;

                    /// Next ride choice for each vehicle
                    Ride ride = (optimalRideForCurrentStep != null && optimalRideForCurrentStep.Done == false)     // Since optimalRide is declared before entering foreach, null comprobation is not enough and .Done needs to checked too here
                        ? optimalRideForCurrentStep                         // Current behavior: chose only according to bonus give for early start
                        : null;

                    // Possible improvement : ride selection taking into account distance vehicle-ride init, etc. (has to do with ordering vehicleList)
                    // Alternatives to main ride choice (already done or impossible to do)

                    if (ride == null)   // Alternatives to optimal ride
                    {
                        switch (_inputFileName)
                        {
                            case ("b_should_be_easy.in"):
                                ride = GetRandomRemainingRide();
                                break;
                            case ("c_no_hurry.in"):     // Deterministic output
                                {
                                    ride = MoreLinq.MoreEnumerable.MinBy(RideList, r => v.CalculateDistanceToAPoint(r.InitialPosition));
                                }
                                break;
                            case ("d_metropolis.in"):
                                {
                                    ride = currentStep <= 0.75 * TotalSimulationSteps
                                        ? GetRandomRemainingRide()
                                        : MoreLinq.MoreEnumerable.MinBy(RideList, r => v.CalculateDistanceToAPoint(r.InitialPosition));
                                }
                                break;
                            case ("e_high_bonus.in"):
                                {
                                    ride = currentStep <= 0.75 * TotalSimulationSteps
                                        ? ride ?? GetRandomRemainingRide()
                                        : ride ?? MoreLinq.MoreEnumerable.MinBy(RideList, r => v.CalculateDistanceToAPoint(r.InitialPosition));
                                }
                                break;
                            default:
                                throw new Exception("File behavior needs to be defined");
                        }
                    }

                    /// Step simulation actions
                    if (ride.IsOnTimeOrAfterEarlyStart(currentStep + v.CalculateDistanceToAPoint(ride.InitialPosition)))
                    {
                        RideList.Remove(ride);
                        ride.Distance += v.CalculateDistanceToAPoint(ride.InitialPosition);
                        ride.Done = true;
                        ride.DoneInEarlyStart = currentStep == ride.EarlyStart;
                        v.Free = false;
                        v.StepWhenWillBeFee = currentStep + ride.Distance;  // Including trip to position

                        v.SuccessfullRides.Add(ride);
                    }
                }
                currentStep++;
            }
        }

        private void UpdateVehicleState(long currentStep)
        {
#pragma warning disable S2201 // Return values from functions without side effects should not be ignored
            VehicleList.Where(v => v.StepWhenWillBeFee == currentStep)
                .Select(veh =>
                {
                    if (veh.Free == true)
                        throw new Exception();

                    veh.Free = true;
                    veh.StepWhenWillBeFee = -1;
                    veh.RealPosition = veh.SuccessfullRides.Last().EndPosition;
                    return veh;
                }).ToList();    // ToList is needed in order to evaluate the select immediately due to lazy evaluation.
#pragma warning restore S2201 // Return values from functions without side effects should not be ignored
        }

        private Ride GetRandomRemainingRide()
        {
            return RideList[_rnd.Next(0, RideList.Count - 1)];
        }

        private Ride GetOptimalRide(long currentStep)
        {
            return RideList.FirstOrDefault(r => r.EarlyStart == currentStep);
        }
    }
}
