using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math.Optimization.Losses;
using CsvHelper;

namespace FlightDelayPredictor.AccordTree
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("parsing...");
            List<Flight> flights = Flight.ParseFlights();
            Console.WriteLine("done");

            int[,] fields = Flight.FlightsToArray(flights);


            Console.ReadKey();
        }
    }
}
