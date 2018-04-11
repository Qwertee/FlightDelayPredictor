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

        public static int[][] FieldsArray;
        public static int[] OutputArray;

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
            List<Flight> flights = new List<Flight>();
            var csv = new CsvReader(File.OpenText("../../../../flights.csv"));
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
                    AirlineInts.Add(flight.Airline, AirlineInts.Count);
                }

                flight.OriginAirport = csv.GetField<string>("ORIGIN_AIRPORT");
                if (!AirportInts.ContainsKey(flight.OriginAirport))
                {
                    AirportInts.Add(flight.OriginAirport, AirportInts.Count);
                }

                flight.DestinationAirport = csv.GetField<string>("DESTINATION_AIRPORT");
                if (!AirportInts.ContainsKey(flight.DestinationAirport))
                {
                    AirportInts.Add(flight.DestinationAirport, AirportInts.Count);
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
        public static void FlightsToArrays(List<Flight> flights, int sampleSize = -1)
        {
            Debug.WriteLine("Converting Flights list to arrays...");

            if (sampleSize == -1)
            {
                FieldsArray = new int[flights.Count][];
                OutputArray = new int[flights.Count];

                for (int i = 0; i < flights.Count; i++)
                {
                    FieldsArray[i] = new int[NUM_FIELDS];

                    FieldsArray[i][0] = flights[i].Month;
                    FieldsArray[i][1] = flights[i].Day;
                    FieldsArray[i][2] = flights[i].DayOfWeek;
                    FieldsArray[i][3] = AirlineInts[flights[i].Airline];
                    FieldsArray[i][4] = AirportInts[flights[i].OriginAirport];
                    FieldsArray[i][5] = AirportInts[flights[i].DestinationAirport];

                    // 1 if there is a delay
                    OutputArray[i] = flights[i].DepartureDelay > 0 ? 1 : 0;
                }
            } else
            {
                Random random = new Random();

                FieldsArray = new int[sampleSize][];
                OutputArray = new int[sampleSize];

                for (int i = 0; i < sampleSize; i++)
                {
                    FieldsArray[i] = new int[NUM_FIELDS];

                    // get random flight from list
                    Flight flight = flights[random.Next(flights.Count)];

                    FieldsArray[i][0] = flight.Month;
                    FieldsArray[i][1] = flight.Day;
                    FieldsArray[i][2] = flight.DayOfWeek;
                    FieldsArray[i][3] = AirlineInts[flight.Airline];
                    FieldsArray[i][4] = AirportInts[flight.OriginAirport];
                    FieldsArray[i][5] = AirportInts[flight.DestinationAirport];

                    // 1 if there is a delay
                    OutputArray[i] = flight.DepartureDelay > 0 ? 1 : 0;

                }
            }

            Debug.WriteLine("done.");
        }

    }
}
