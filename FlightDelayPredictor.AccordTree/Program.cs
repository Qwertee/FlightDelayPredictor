using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
            Stopwatch sw = new Stopwatch();

            sw.Start();
            Console.WriteLine("parsing...");
            List<Flight> flights = Flight.ParseFlights();
            sw.Stop();
            Console.WriteLine("done, time taken: " + sw.Elapsed.Seconds + " seconds");

            Flight.FlightsToArrays(flights, 100000);

            ID3Learning teacher = new ID3Learning();
            Console.WriteLine("training...");
            sw.Restart();
            var tree = teacher.Learn(Flight.FieldsArray, Flight.OutputArray);
            sw.Stop();
            Console.WriteLine("done, time taken: " + sw.Elapsed.Seconds + " seconds");

            double error = new ZeroOneLoss(Flight.OutputArray).Loss(tree.Decide(Flight.FieldsArray));
            Console.WriteLine("accuracy on training set: " + ((1 - error) * 100) + "%");

            Console.ReadKey();
        }
    }
}
