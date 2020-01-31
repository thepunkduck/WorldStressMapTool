using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;

namespace UTM_LL
{
    public class OrthographicProjection
    {
        private double _cosφ0, _sinφ0;
        private double _λ0Rad;

        // angle in degrees
        public OrthographicProjection(double φ, double λ)
        {
            _setup(φ, λ);
        }

        void _setup(double φ, double λ)
        {
            φ0 = φ;
            λ0 = λ;
            _λ0Rad = ToRadians(λ);
            _cosφ0 = _cosDeg(φ0);
            _sinφ0 = _sinDeg(φ0);
            Radius = EarthRadiusKm;
        }

        public double φ0 { get; private set; }
        public double λ0 { get; private set; }
        public double Radius { get; private set; }

        double _cosDeg(double angle) { return Math.Cos(angle * 0.01745329252); }
        double _sinDeg(double angle) { return Math.Sin(angle * 0.01745329252); }
        public static double ToRadians(double deg) { return (deg * 0.01745329252); }
        public static double ToDegrees(double rad) { return (rad * 57.295779513); }
        public static double EarthRadiusKm { get { return 6371; } }

        public static double SanitizeLongitude(double λ)
        {
            while (λ >= 180) λ -= 360;
            while (λ < -180) λ += 360;
            return (λ);
        }

        public bool ToTangentPlane(double φ, double λ, out double x, out double y)
        {
            double cosC = _sinφ0 * _sinDeg(φ) + _cosφ0 * _cosDeg(φ) * _cosDeg(λ - λ0);
            if (cosC < 0.0) { x = 0; y = 0; return (false); } // clip if around other side
            x = Radius * _cosDeg(φ) * _sinDeg(λ - λ0);
            y = Radius * (_cosφ0 * _sinDeg(φ) - _sinφ0 * _cosDeg(φ) * _cosDeg(λ - λ0));
            return (true);
        }

        public bool ToSphere(double x, double y, out double φ, out double λ)
        {
            if (x == 0 && y == 0) { φ = φ0; λ = λ0; }
            double ρ = Math.Sqrt(x * x + y * y);
            double c = Math.Asin(ρ / Radius);
            double a = Math.Cos(c);
            double sinC = Math.Sin(c);
            φ = ToDegrees(Math.Asin(a * _sinφ0 + ((y * sinC * _cosφ0) / ρ)));
            λ = ToDegrees(_λ0Rad + Math.Atan2(x * sinC, ρ * a * _cosφ0 - y * sinC * _sinφ0));
            λ = SanitizeLongitude(λ);
            return (true);
        }

    }


    public static class GeoCoordinateUtilities
    {
        public static GeoCoordinate GetCentralGeoCoordinate(IList<GeoCoordinate> geoCoordinates)
        {
            if (geoCoordinates.Count == 1)
            {
                return geoCoordinates.Single();
            }

            double x = 0;
            double y = 0;
            double z = 0;

            foreach (var geoCoordinate in geoCoordinates)
            {
                var latitude = ToRadians(geoCoordinate.Latitude);
                var longitude = ToRadians(geoCoordinate.Longitude);

                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            var total = geoCoordinates.Count;

            x = x / total;
            y = y / total;
            z = z / total;

            var centralLongitude = Math.Atan2(y, x);
            var centralSquareRoot = Math.Sqrt(x * x + y * y);
            var centralLatitude = Math.Atan2(z, centralSquareRoot);

            return new GeoCoordinate(ToDegrees(centralLatitude), ToDegrees(centralLongitude));
        }

        public static double ToRadians(double deg) { return (deg * 0.01745329252); }
        public static double ToDegrees(double rad) { return (rad * 57.295779513); }
        public static double ParseDegrees(string input)
        {
            if (String.IsNullOrWhiteSpace(input)) return (Double.NaN);

            // letter at end N, S E, W??
            input = input.Trim();
            int sign = 1;
            if (input.EndsWith("N") || input.EndsWith("n")) sign = 1;
            if (input.EndsWith("S") || input.EndsWith("s")) sign = -1;
            if (input.EndsWith("E") || input.EndsWith("E")) sign = 1;
            if (input.EndsWith("W") || input.EndsWith("w")) sign = -1;
            if (input.StartsWith("+")) sign = 1;
            if (input.StartsWith("-")) sign = -1;

            input = input.TrimEnd("NWEWnsew".ToCharArray());
            input = input.TrimStart("+-".ToCharArray());

            string[] part = input.Split(" \t,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            double deg = 0, m = 0, s = 0;
            double res;
            if (part.Length > 0 && double.TryParse(part[0], out res)) deg = res;
            if (part.Length > 1 && double.TryParse(part[1], out res)) m = res;
            if (part.Length > 2 && double.TryParse(part[2], out res)) s = res;

            double decDegrees = sign * (deg + (m / 60.0) + (s / 3600.0));
            return (decDegrees);
        }

        public static string FormatLatitude(double lat)
        {
            return (FormatCoord(lat, " N", " S"));
        }
        public static string FormatLongitude(double lon)
        {
            return (FormatCoord(lon, " E", " W"));
        }

        static string FormatCoord(double ang, string pos, string neg)
        {
            string dir;
            if (ang > 0.0) dir = pos;
            else if (ang < 0.0) dir = neg;
            else dir = "";
            ang = Math.Abs(ang);
            int deg = (int)ang;
            double dmin = (ang - deg) * 60;
            int min = (int)dmin;
            double dsec = (dmin - min) * 60;
            return ($"{deg}° {min}' {dsec:0.0}\"{dir}");
        }


        public static GeoCoordinate GetEndPoint(GeoCoordinate start, double azimuth, double distance)
        {
            double b = distance / OrthographicProjection.EarthRadiusKm;
            double sLat = ToRadians(90 - start.Latitude);
            double radAz = ToRadians(azimuth);
            double a = Math.Acos(Math.Cos(b) * Math.Cos(sLat) + Math.Sin(sLat) * Math.Sin(b) * Math.Cos(radAz));
            double B = Math.Asin(Math.Sin(b) * Math.Sin(radAz) / Math.Sin(a));
            double lat2 = ToDegrees(a);
            lat2 = 90 - lat2;
            double lon2 = ToDegrees(B) + start.Longitude;
            lon2 =  OrthographicProjection.SanitizeLongitude(lon2);
            return new GeoCoordinate(lat2, lon2);
        }

    }
}
