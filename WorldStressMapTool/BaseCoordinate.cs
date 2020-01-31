using System.Device.Location;

namespace WorldStressMapTool
{
    public class BaseCoordinate : GeoCoordinate
    {
        public BaseCoordinate(double latitude, double longitude) : base(latitude, longitude)
        {
            ProjectedX = 0.0;
            ProjectedY = 0.0;
        }
        public BaseCoordinate(GeoCoordinate gC) : base(gC.Latitude, gC.Longitude)
        {
            ProjectedX = 0.0;
            ProjectedY = 0.0;
        }
        public double ProjectedX { get; set; }
        public double ProjectedY { get; set; }


        internal double GetDistanceTo(double lat, double lon)
        {
            return (GetDistanceTo(new GeoCoordinate(lat, lon)));
        }
        public override string ToString()
        {
            return (Latitude + ", " + Longitude);
        }
    }


}