using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightDelayPredictor.AccordTree
{
    public class Flight
    {
        static readonly int NUM_FIELDS = 6;

        static Dictionary<string, int> AirlineInts = new Dictionary<string, int>();
        static Dictionary<string, int> AirportInts = new Dictionary<string, int>();

        public int Month { get; set; }
        public int Day { get; set; }
        public int DayOfWeek { get; set; }
        public string Airline { get; set; }
        public string OriginAirport { get; set; }
        public string DestinationAirport { get; set; }
        public int DepartureDelay { get; set; }

        public static List<Flight> ParseFlights()
        {
            AirlineInts.Add("COUNT", 0);
            AirportInts.Add("COUNT", 0);

            List<Flight> flights = new List<Flight>();
            var csv = new CsvReader(File.OpenText("../../../flights.csv"));
            csv.Read();
            csv.ReadHeader();


            // takes about a minute on my laptop
            int line_count = 0;
            int skipped_lines = 0;

            while (csv.Read())
            {
                line_count++;
                // Console.WriteLine("Line count: " + line_count++);
                Flight flight = new Flight();
                flight.Month = csv.GetField<int>("MONTH");
                flight.Day = csv.GetField<int>("DAY");
                flight.DayOfWeek = csv.GetField<int>("DAY_OF_WEEK");

                flight.Airline = csv.GetField<string>("AIRLINE");
                if (!AirlineInts.ContainsKey(flight.Airline))
                {
                    AirlineInts.Add(flight.Airline, AirlineInts["COUNT"]);
                    AirlineInts["COUNT"]++;
                }

                flight.OriginAirport = csv.GetField<string>("ORIGIN_AIRPORT");
                if (!AirportInts.ContainsKey(flight.OriginAirport))
                {
                    AirportInts.Add(flight.OriginAirport, AirportInts["COUNT"]);
                    AirportInts["COUNT"]++;
                }

                flight.DestinationAirport = csv.GetField<string>("DESTINATION_AIRPORT");
                if (!AirportInts.ContainsKey(flight.DestinationAirport))
                {
                    AirportInts.Add(flight.DestinationAirport, AirportInts["COUNT"]);
                    AirportInts["COUNT"]++;
                }

                // departure delay may not exist
                string delay = csv.GetField("DEPARTURE_DELAY");
                if (delay.Equals(""))
                {
                    skipped_lines++;
                    continue;
                }
                flight.DepartureDelay = int.Parse(delay);

                flights.Add(flight);
            }
            Console.WriteLine("total lines:   " + line_count);
            Console.WriteLine("skipped lines: " + skipped_lines);
            return flights;
        }

        // Convert list of flights to int[,] array to be used by the Accord decision tree
        public static int[,] FlightsToArray(List<Flight> flights)
        {
            Debug.WriteLine("Converting Flights list to array...");

            int[,] fields = new int[flights.Count, NUM_FIELDS];

            for (int i = 0; i < flights.Count; i++)
            {
                fields[i, 0] = flights[i].Month;
                fields[i, 1] = flights[i].Day;
                fields[i, 2] = flights[i].DayOfWeek;
                fields[i, 3] = AirlineInts[flights[i].Airline];
                fields[i, 4] = AirportInts[flights[i].OriginAirport];
                fields[i, 5] = AirportInts[flights[i].DestinationAirport];
            }

            Debug.WriteLine("done.");
            return fields;
        }

    }
}
