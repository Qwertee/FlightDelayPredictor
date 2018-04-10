using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightDelayPredictor.AccordTree
{
    public class Flight
    {
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
                flight.OriginAirport = csv.GetField<string>("ORIGIN_AIRPORT");
                flight.DestinationAirport = csv.GetField<string>("DESTINATION_AIRPORT");

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

    }
}
