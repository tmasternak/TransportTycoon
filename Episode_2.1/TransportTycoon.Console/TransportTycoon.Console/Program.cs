using System;
using System.Collections.Generic;
using System.Linq;

namespace TransportTycoon.Console
{
    internal class Program
    {
        static string data =
            @"A           	B           	km
            Cogburg     	Copperhold  	1047
            Leverstorm  	Irondale    	673
            Cogburg     	Steamdrift  	1269
            Copperhold  	Irondale    	345
            Copperhold  	Leverstorm  	569
            Leverstorm  	Gizbourne   	866
            Rustport    	Cogburg     	1421
            Rustport    	Steamdrift  	1947
            Rustport    	Gizbourne   	1220
            Irondale    	Gizbourne   	526
            Cogburg     	Irondale    	1034
            Rustport        Irondale    	1302";

        static void Main(string[] args)
        {
            var cities = new Dictionary<string, City>();

            var lines = data.Split(Environment.NewLine).Skip(1);
            foreach (var line in lines)
            {
                var lineParts = line.Split(new []{' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);

                var sourceName = lineParts[0];
                var destinationName = lineParts[1];
                var distance = int.Parse(lineParts[2]);

                if (cities.ContainsKey(sourceName) == false)
                {
                    cities.Add(sourceName, new City { Name = sourceName });
                }

                if (cities.ContainsKey(destinationName) == false)
                {
                    cities.Add(destinationName, new City { Name = destinationName });
                }

                var source = cities[sourceName];
                var destination = cities[destinationName];

                source.NeighborCities.Add((destination, distance));
                destination.NeighborCities.Add((source, distance));
            }

            var routeStartName = args[0];
            var routeEndName = args[1];

            var routeStart = cities[routeStartName];
            var routeEnd = cities[routeEndName];

            routeStart.TotalDistance = 0;

            var nextCities = new SortedSet<City>(new ByTotalDistance()) {routeStart};

            while (true)
            {
                var currentCity = nextCities.Min;
                nextCities.Remove(currentCity);

                if (currentCity == routeEnd)
                {
                    break;
                }

                foreach (var (neighborCity, neighborDistance) in currentCity.NeighborCities)
                {
                    if (neighborCity.TotalDistance.HasValue == false)
                    {
                        neighborCity.TotalDistance = currentCity.TotalDistance + neighborDistance;
                        neighborCity.Previous = currentCity;

                        nextCities.Add(neighborCity);
                    }
                }
            }

            var path = new List<string>();
            var nextOnPath = routeEnd;

            while (nextOnPath != null)
            {
                path.Insert(0, nextOnPath.Name);
                nextOnPath = nextOnPath.Previous;
            }

            System.Console.WriteLine(string.Join(",", path.ToArray()));
        }

        class City
        {
            public string Name { get; set; }
            public List<(City, int)> NeighborCities { get; } = new List<(City, int)>();
            public int? TotalDistance { get; set; }
            public City Previous { get; set; }
        }

        class ByTotalDistance : IComparer<City>
        {
            public int Compare(City x, City y)
            {
                return x.TotalDistance.GetValueOrDefault() - y.TotalDistance.GetValueOrDefault();
            }
        }
    }
}
