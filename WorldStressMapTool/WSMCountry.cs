using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Drawing;
using UTM_LL;

namespace WorldStressMapTool
{
    public class WSMCountry
    {
        public WSMCountry(string name)
        {
            Name = name;
            Collection = new List<WSMPolygon>();
        }

        public string Name { get; set; }
        public List<WSMPolygon> Collection { get; set; }
        public bool HasInternalBorders { get; internal set; }
        public Brush Brush { get; internal set; }
        public List<WSMCountry> NeighborList = new List<WSMCountry>();
        public object Tag { get; set; }
        public double MinLatitude { get; set; }
        public double MaxLatitude { get; set; }
        public double MaxLongitude { get; set; }
        public double MinLongitude { get; set; }
        public double Distance { get; internal set; }
        public BaseCoordinate CentralCoordinate { set; get; }
        public BaseCoordinate EasternCentralCoordinate { set; get; }

        public override string ToString()
        {
            return (Name + " : " + CentralCoordinate.ToString() + " " + Collection.Count + " polygons");
        }
        internal void AddPolygon(WSMPolygon poly)
        {
            Collection.Add(poly);
        }

        internal WSMCountry ShallowCopy()
        {
            WSMCountry copy = new WSMCountry(Name);
            copy.Collection = Collection;
            copy.HasInternalBorders = HasInternalBorders;
            copy.Brush = Brush;
            copy.NeighborList = new List<WSMCountry>(NeighborList);
            return (copy);
        }

        internal void Finish()
        {
            MinLatitude = Double.MaxValue;
            MaxLatitude = Double.MinValue;
            MinLongitude = Double.MaxValue;
            MaxLongitude = Double.MinValue;
            foreach (WSMPolygon fP in Collection)
            {
                fP.MinLatitude = Double.MaxValue;
                fP.MaxLatitude = Double.MinValue;
                foreach (var co in fP.Collection)
                {
                    MinLatitude = Math.Min(MinLatitude, co.Latitude);
                    MaxLatitude = Math.Max(MaxLatitude, co.Latitude);
                    MinLongitude = Math.Min(MinLongitude, co.Longitude);
                    MaxLongitude = Math.Max(MaxLongitude, co.Longitude);
                    fP.MinLatitude = Math.Min(fP.MinLatitude, co.Latitude);
                    fP.MaxLatitude = Math.Max(fP.MaxLatitude, co.Latitude);
                }
            }

            CentralCoordinate = GetCentralGeoCoordinate();
            EasternCentralCoordinate = new BaseCoordinate((MinLatitude + MaxLatitude) / 2.0, MaxLongitude);
        }



        public bool IsNeighbour(WSMCountry countryDefinition2)
        {
            if (MinLatitude > countryDefinition2.MaxLatitude) return (false);
            if (MaxLatitude < countryDefinition2.MinLatitude) return (false);

            foreach (WSMPolygon poly1 in Collection)
            {
                foreach (WSMPolygon poly2 in countryDefinition2.Collection)
                {
                    if (poly1.MinLatitude > poly2.MaxLatitude) continue;
                    if (poly1.MaxLatitude < poly2.MinLatitude) continue;

                    foreach (var pt1 in poly1.Collection)
                    {
                        foreach (var pt2 in poly2.Collection)
                        {
                            if (pt1.Latitude == pt2.Latitude && pt1.Longitude == pt2.Longitude)
                                return (true);
                        }
                    }
                }
            }

            return (false);
        }

        private BaseCoordinate GetCentralGeoCoordinate()
        {
            List<GeoCoordinate> geoCoords = new List<GeoCoordinate>();
            foreach (WSMPolygon poly1 in Collection)
                foreach (var pt1 in poly1.Collection)
                    geoCoords.Add(new GeoCoordinate(pt1.Latitude, pt1.Longitude));

            GeoCoordinate gc = GeoCoordinateUtilities.GetCentralGeoCoordinate(geoCoords);
            BaseCoordinate fC = new BaseCoordinate(gc);
            return (fC);
        }
    }


}