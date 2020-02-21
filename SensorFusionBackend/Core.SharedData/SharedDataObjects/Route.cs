using System.Collections.Generic;

namespace Core.Common.SharedDataObjects
{
    public class Route
    {
        public List<GpsPoint> points { get; set; }

        public double StartLat { get; set; }

        public double StartLng { get; set; }

        public double EndLat { get; set; }

        public double EndLng { get; set; }

        public float RouteLength { get; set; }
    }
}