using System;
using System.Collections.Generic;

namespace lab1
{
    public class RoutingTable
    {
        public int Vertex { get; set; }
        public Dictionary<int, RouteEntry> Entries { get; set; }

        public RoutingTable(int vertex)
        {
            Vertex = vertex;
            Entries = new Dictionary<int, RouteEntry>();
        }

        public void UpdateEntry(int destination, int hopCount, int nextVertex)
        {
            if (!Entries.ContainsKey(destination) || Entries[destination].HopCount > hopCount)
            {
                Entries[destination] = new RouteEntry
                {
                    Destination = destination,
                    HopCount = hopCount,
                    NextVertex = nextVertex
                };
            }
        }

        public RouteEntry GetEntry(int destination)
        {
            return Entries.ContainsKey(destination) ? Entries[destination] : null;
        }
    }

    public class RouteEntry
    {
        public int Destination { get; set; }
        public int HopCount { get; set; }
        public int NextVertex { get; set; }
    }
}