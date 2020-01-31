using System;

namespace UTM_LL
{

    class Ellipsoid
    {
        public Ellipsoid()
        {

        }

        public Ellipsoid(int Id, string name, double radius, double ecc)
        {
            id = Id;
            ellipsoidName = name;
            EquatorialRadius = radius; eccentricitySquared = ecc;
        }

        public int id;
        public string ellipsoidName;
        public double EquatorialRadius;
        public double eccentricitySquared;
    }

    public class UTM_LL_Tool
    {
        static Ellipsoid[] ellipsoid =
  {//  id, Ellipsoid name, Equatorial Radius, square of eccentricity	
	    new Ellipsoid( -1, "Placeholder", 6377563, 0),//placeholder only, To allow array indices to match id numbers
        new Ellipsoid( 1, "Airy", 6377563, 0.00667054),
        new  Ellipsoid( 2, "Australian National", 6378160, 0.006694542),
        new  Ellipsoid( 3, "Bessel 1841", 6377397, 0.006674372),
        new  Ellipsoid( 4, "Bessel 1841 (Nambia) ", 6377484, 0.006674372),
        new  Ellipsoid( 5, "Clarke 1866", 6378206, 0.006768658),
        new  Ellipsoid( 6, "Clarke 1880", 6378249, 0.006803511),
        new  Ellipsoid( 7, "Everest", 6377276, 0.006637847),
        new  Ellipsoid( 8, "Fischer 1960 (Mercury) ", 6378166, 0.006693422),
        new  Ellipsoid( 9, "Fischer 1968", 6378150, 0.006693422),
        new  Ellipsoid( 10, "GRS 1967", 6378160, 0.006694605),
        new  Ellipsoid( 11, "GRS 1980", 6378137, 0.00669438),
        new  Ellipsoid( 12, "Helmert 1906", 6378200, 0.006693422),
        new  Ellipsoid( 13, "Hough", 6378270, 0.00672267),
        new  Ellipsoid( 14, "International", 6378388, 0.00672267),
        new  Ellipsoid( 15, "Krassovsky", 6378245, 0.006693422),
        new  Ellipsoid( 16, "Modified Airy", 6377340, 0.00667054),
        new  Ellipsoid( 17, "Modified Everest", 6377304, 0.006637847),
        new  Ellipsoid( 18, "Modified Fischer 1960", 6378155, 0.006693422),
        new  Ellipsoid( 19, "South American 1969", 6378160, 0.006694542),
        new  Ellipsoid( 20, "WGS 60", 6378165, 0.006693422),
        new  Ellipsoid( 21, "WGS 66", 6378145, 0.006694542),
        new  Ellipsoid( 22, "WGS-72", 6378135, 0.006694318),
        new  Ellipsoid( 23, "WGS-84", 6378137, 0.00669438)
};

        public static void LLtoUTM(int referenceEllipsoid, double lat, double Long,
                        out double utmNorthing, out double utmEasting,
                        out string utmZone)
        {
            //converts lat/long to UTM coords.  Equations from USGS Bulletin 1532 
            //East Longitudes are positive, West longitudes are negative. 
            //North latitudes are positive, South latitudes are negative
            //Lat and Long are in decimal degrees
            //Written by Chuck Gantz- chuck.gantz@globalstar.com

            double a = ellipsoid[referenceEllipsoid].EquatorialRadius;
            double eccSquared = ellipsoid[referenceEllipsoid].eccentricitySquared;
            double k0 = 0.9996;

            double LongOrigin;
            double eccPrimeSquared;
            double N, T, C, A, M;

            //Make sure the longitude is between -180.00 .. 179.9
            double LongTemp = OrthographicProjection.SanitizeLongitude(Long);

            double LatRad = OrthographicProjection.ToRadians(lat);
            double LongRad = OrthographicProjection.ToRadians(LongTemp);
            double LongOriginRad;
            int ZoneNumber;

            ZoneNumber = (int)((LongTemp + 180) / 6) + 1;


            if (lat >= 56.0 && lat < 64.0 && LongTemp >= 3.0 && LongTemp < 12.0)
                ZoneNumber = 32;

            // Special zones for Svalbard
            if (lat >= 72.0 && lat < 84.0)
            {
                if (LongTemp >= 0.0 && LongTemp < 9.0) ZoneNumber = 31;
                else if (LongTemp >= 9.0 && LongTemp < 21.0) ZoneNumber = 33;
                else if (LongTemp >= 21.0 && LongTemp < 33.0) ZoneNumber = 35;
                else if (LongTemp >= 33.0 && LongTemp < 42.0) ZoneNumber = 37;
            }
            LongOrigin = (ZoneNumber - 1) * 6 - 180 + 3;  //+3 puts origin in middle of zone
            LongOriginRad = OrthographicProjection.ToRadians(LongOrigin);

            //compute the UTM Zone from the latitude and longitude
            utmZone = String.Format("{0}{1}", ZoneNumber, UTMLetterDesignator(lat));

            eccPrimeSquared = (eccSquared) / (1 - eccSquared);

            N = a / Math.Sqrt(1 - eccSquared * Math.Sin(LatRad) * Math.Sin(LatRad));
            T = Math.Tan(LatRad) * Math.Tan(LatRad);
            C = eccPrimeSquared * Math.Cos(LatRad) * Math.Cos(LatRad);
            A = Math.Cos(LatRad) * (LongRad - LongOriginRad);

            M = a * ((1 - eccSquared / 4 - 3 * eccSquared * eccSquared / 64 - 5 * eccSquared * eccSquared * eccSquared / 256) * LatRad
                        - (3 * eccSquared / 8 + 3 * eccSquared * eccSquared / 32 + 45 * eccSquared * eccSquared * eccSquared / 1024) * Math.Sin(2 * LatRad)
                                            + (15 * eccSquared * eccSquared / 256 + 45 * eccSquared * eccSquared * eccSquared / 1024) * Math.Sin(4 * LatRad)
                                            - (35 * eccSquared * eccSquared * eccSquared / 3072) * Math.Sin(6 * LatRad));

            utmEasting = k0 * N * (A + (1 - T + C) * A * A * A / 6
                            + (5 - 18 * T + T * T + 72 * C - 58 * eccPrimeSquared) * A * A * A * A * A / 120)
                            + 500000.0;

            utmNorthing = k0 * (M + N * Math.Tan(LatRad) * (A * A / 2 + (5 - T + 9 * C + 4 * C * C) * A * A * A * A / 24
                         + (61 - 58 * T + T * T + 600 * C - 330 * eccPrimeSquared) * A * A * A * A * A * A / 720));
            if (lat < 0)
                utmNorthing += 10000000.0; //10000000 meter offset for southern hemisphere
        }

        static char UTMLetterDesignator(double lat)
        {
            //This routine determines the correct UTM letter designator for the given latitude
            //returns 'Z' if latitude is outside the UTM limits of 84N to 80S
            //Written by Chuck Gantz- chuck.gantz@globalstar.com
            char LetterDesignator;

            if ((84 >= lat) && (lat >= 72)) LetterDesignator = 'X';
            else if ((72 > lat) && (lat >= 64)) LetterDesignator = 'W';
            else if ((64 > lat) && (lat >= 56)) LetterDesignator = 'V';
            else if ((56 > lat) && (lat >= 48)) LetterDesignator = 'U';
            else if ((48 > lat) && (lat >= 40)) LetterDesignator = 'T';
            else if ((40 > lat) && (lat >= 32)) LetterDesignator = 'S';
            else if ((32 > lat) && (lat >= 24)) LetterDesignator = 'R';
            else if ((24 > lat) && (lat >= 16)) LetterDesignator = 'Q';
            else if ((16 > lat) && (lat >= 8)) LetterDesignator = 'P';
            else if ((8 > lat) && (lat >= 0)) LetterDesignator = 'N';
            else if ((0 > lat) && (lat >= -8)) LetterDesignator = 'M';
            else if ((-8 > lat) && (lat >= -16)) LetterDesignator = 'L';
            else if ((-16 > lat) && (lat >= -24)) LetterDesignator = 'K';
            else if ((-24 > lat) && (lat >= -32)) LetterDesignator = 'J';
            else if ((-32 > lat) && (lat >= -40)) LetterDesignator = 'H';
            else if ((-40 > lat) && (lat >= -48)) LetterDesignator = 'G';
            else if ((-48 > lat) && (lat >= -56)) LetterDesignator = 'F';
            else if ((-56 > lat) && (lat >= -64)) LetterDesignator = 'E';
            else if ((-64 > lat) && (lat >= -72)) LetterDesignator = 'D';
            else if ((-72 > lat) && (lat >= -80)) LetterDesignator = 'C';
            else LetterDesignator = 'Z'; //This is here as an error flag to show that the Latitude is outside the UTM limits

            return LetterDesignator;
        }


        static public void UTMtoLL(int referenceEllipsoid, double utmNorthing, double utmEasting, string utmZone,
              out double lat, out double Long)
        {
            //converts UTM coords to lat/long.  Equations from USGS Bulletin 1532 
            //East Longitudes are positive, West longitudes are negative. 
            //North latitudes are positive, South latitudes are negative
            //Lat and Long are in decimal degrees. 
            //Written by Chuck Gantz- chuck.gantz@globalstar.com

            double k0 = 0.9996;
            double a = ellipsoid[referenceEllipsoid].EquatorialRadius;
            double eccSquared = ellipsoid[referenceEllipsoid].eccentricitySquared;
            double eccPrimeSquared;
            double e1 = (1 - Math.Sqrt(1 - eccSquared)) / (1 + Math.Sqrt(1 - eccSquared));
            double N1, T1, C1, R1, D, M;
            double LongOrigin;
            double mu, phi1, phi1Rad;
            double x, y;
            int ZoneNumber;
            string ZoneLetter;
            int NorthernHemisphere; //1 for northern hemispher, 0 for southern

            x = utmEasting - 500000.0; //remove 500,000 meter offset for longitude
            y = utmNorthing;

            // UTM zone is number and letter
            utmZone = utmZone.Trim();
            int idx = utmZone.Length - 1;
            ZoneLetter = utmZone.Substring(idx); // last character
            ZoneNumber = int.Parse(utmZone.Substring(0, idx));
            if (String.Compare(ZoneLetter, "N") >= 0)  //	if((* ZoneLetter - 'N') >= 0)
                NorthernHemisphere = 1;//point is in northern hemisphere
            else
            {
                NorthernHemisphere = 0;//point is in southern hemisphere
                y -= 10000000.0;//remove 10,000,000 meter offset used for southern hemisphere
            }

            LongOrigin = (ZoneNumber - 1) * 6 - 180 + 3;  //+3 puts origin in middle of zone

            eccPrimeSquared = (eccSquared) / (1 - eccSquared);

            M = y / k0;
            mu = M / (a * (1 - eccSquared / 4 - 3 * eccSquared * eccSquared / 64 - 5 * eccSquared * eccSquared * eccSquared / 256));

            phi1Rad = mu + (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32) * Math.Sin(2 * mu)
                        + (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32) * Math.Sin(4 * mu)
                        + (151 * e1 * e1 * e1 / 96) * Math.Sin(6 * mu);
            phi1 = OrthographicProjection.ToDegrees(phi1Rad);

            N1 = a / Math.Sqrt(1 - eccSquared * Math.Sin(phi1Rad) * Math.Sin(phi1Rad));
            T1 = Math.Tan(phi1Rad) * Math.Tan(phi1Rad);
            C1 = eccPrimeSquared * Math.Cos(phi1Rad) * Math.Cos(phi1Rad);
            R1 = a * (1 - eccSquared) / Math.Pow(1 - eccSquared * Math.Sin(phi1Rad) * Math.Sin(phi1Rad), 1.5);
            D = x / (N1 * k0);

            lat = phi1Rad - (N1 * Math.Tan(phi1Rad) / R1) * (D * D / 2 - (5 + 3 * T1 + 10 * C1 - 4 * C1 * C1 - 9 * eccPrimeSquared) * D * D * D * D / 24
                            + (61 + 90 * T1 + 298 * C1 + 45 * T1 * T1 - 252 * eccPrimeSquared - 3 * C1 * C1) * D * D * D * D * D * D / 720);
            lat = OrthographicProjection.ToDegrees(lat);

            Long = (D - (1 + 2 * T1 + C1) * D * D * D / 6 + (5 - 2 * C1 + 28 * T1 - 3 * C1 * C1 + 8 * eccPrimeSquared + 24 * T1 * T1)
                            * D * D * D * D * D / 120) / Math.Cos(phi1Rad);
            Long = LongOrigin + OrthographicProjection.ToDegrees(Long);
        }




    }
}
