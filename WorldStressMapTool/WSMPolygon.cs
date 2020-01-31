using System.Collections.Generic;

namespace WorldStressMapTool
{
    public class WSMPolygon
    {
        public bool Draw { get; set; }
        public int Polytype { get; set; }
        public double MinLatitude { get; internal set; }
        public double MaxLatitude { get; internal set; }

        public double MinX { get; internal set; }
        public double MaxX { get; internal set; }
        public double MinY { get; internal set; }
        public double MaxY { get; internal set; }

        public List<BaseCoordinate> Collection { get; set; } = new List<BaseCoordinate>();

        public void AddCoordinate(double lat, double lon)
        {
            Collection.Add(new BaseCoordinate(lat, lon));
        }

        internal bool IsInView(double radius)
        {
            if (MinX > radius) return (false);
            if (MaxX < -radius) return (false);
            if (MinY > radius) return (false);
            if (MaxY < -radius) return (false);
            return (true);
        }
    }


}