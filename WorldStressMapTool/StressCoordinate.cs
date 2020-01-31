using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using UTM_LL;


namespace WorldStressMapTool
{
    public class StressCoordinate : BaseCoordinate
    {
        public override string ToString()
        {
            return ("Lat:" + Latitude + " Long:" + Longitude + " Az:" + Azimuth);
        }
        public StressCoordinate(double lat, double lon) : base(lat, lon)
        {

        }

        public string[] Columns { get; internal set; }
        public double Azimuth { get; internal set; }
        public StressTable Table { private get; set; }
        public bool Draw { get; internal set; }
        public double Distance { get; set; }

        internal string Summary(string v)
        {
            string[] idList = v.Split(" ".ToCharArray(), StringSplitOptions.None);
            string summ = "";
            foreach (var id in idList)
            {
                int idx = Table.GetIDX(id);
                if (idx == -1) continue;
                summ += (id + ":" + Columns[idx] + " ");
            }
            return (summ.Trim());
        }
        // headerid [!]filt, comma separated
        internal bool Filter(string filter)
        {
            if (filter == null) return (true);
            if (filter.StartsWith("#")) return (true);

            string[] part = filter.Split(",".ToCharArray());

            for (int i = 0; i < part.Length; i++)
            {
                string p = part[i].Trim();
                int idx = p.IndexOf(" ");
                if (idx < 0) continue;
                string id = p.Substring(0, idx).Trim();
                string filt = p.Substring(idx + 1).Trim();
                var not = filt.StartsWith("!");
                if (not) filt = filt.Substring(1).Trim();
                idx = Table.GetIDX(id);
                if (idx < 0) continue;
                string setting = Columns[idx];
                bool eq = String.Equals(setting, filt);
                if (not == eq) return (false);
            }

            return (true);
        }


    }

    public class GlobalZone
    {
        List<StressCoordinate> _collection = new List<StressCoordinate>();
        List<GlobalZone> _neighborCollection = new List<GlobalZone>();

        public GlobalZone(int lat0, int lat1, int lon0, int lon1)
        {
            Empty = true;
            MinLatitude = lat0;
            MaxLatitude = lat1;
            Corners = new List<BaseCoordinate>
            {
                new BaseCoordinate(lat0, lon0),
                new BaseCoordinate(lat0, lon1),
                new BaseCoordinate(lat1, lon1),
                new BaseCoordinate(lat1, lon0)
            };
        }

        private double MinLatitude { get; set; }
        private double MaxLatitude { get; }
        public List<StressCoordinate> Collection { get => _collection; set => _collection = value; }
        public List<GlobalZone> NeighborCollection { get => _neighborCollection; set => _neighborCollection = value; }
        public List<BaseCoordinate> Corners { get; internal set; }
        public bool Draw { get; set; }
        public bool Empty { get; private set; }
        public double MinProjectedX { get; internal set; }
        public double MinProjectedY { get; internal set; }
        public double MaxProjectedX { get; internal set; }
        public double MaxProjectedY { get; internal set; }

        internal void Add(StressCoordinate item)
        {
            _collection.Add(item);
            Empty = false;
        }

        internal void AddNeighbor(GlobalZone gZ)
        {
            if (gZ.Empty == false)
                _neighborCollection.Add(gZ);
        }
    }

    public class StressTable
    {
        List<StressCoordinate> _collection = new List<StressCoordinate>();
        string[] _headers;
        int latInc = 10;
        int lonInc = 10;
        int nLonDivs;
        int nLatDivs;

        public List<List<GlobalZone>> Zone { get; set; } = new List<List<GlobalZone>>();

        public GlobalZone GetZone(double lat, double lon)
        {
            int layerIndex = (int)((lat + 90.0) / latInc);
            int lonIdx = (int)((lon + 180) / lonInc);
            if (layerIndex >= nLatDivs) layerIndex = nLatDivs - 1;
            if (lonIdx < 0 || lonIdx >= nLonDivs) lonIdx = lonIdx % nLonDivs;
            return (Zone[layerIndex][lonIdx]);
        }
        public string TableText {  get { return (Properties.Resources.wsm2016);  } }
        public StressTable()
        {

            for (int lat = -90; lat < 90; lat += latInc)
            {
                List<GlobalZone> _layer = new List<GlobalZone>();
                Zone.Add(_layer);
                for (int lon = -180; lon < 180; lon += lonInc)
                    _layer.Add(new GlobalZone(lat, lat + latInc, lon, lon + lonInc));
                nLonDivs = _layer.Count;
            }
            nLatDivs = Zone.Count;


            _collection = new List<StressCoordinate>();

            System.IO.StringReader stream = new System.IO.StringReader(Properties.Resources.wsm2016);
            using (TextFieldParser parser = new TextFieldParser(stream))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                int latIDX = 0;
                int lonIDX = 0;
                int aziIDX = 0;
                int cntyIDX = 0;
                while (!parser.EndOfData)
                {
                    // Processing row
                    string[] fields = parser.ReadFields();
                    if (fields == null) continue;
                    if (_headers == null)
                    {
                        _headers = fields;
                        latIDX = GetIDX("LAT");
                        lonIDX = GetIDX("LON");
                        aziIDX = GetIDX("AZI");
                        cntyIDX = GetIDX("COUNTRY");
                        continue;
                    }

                    if (fields.Length <= cntyIDX) continue;

                    double lat = Double.Parse(fields[latIDX]);
                    double lon = Double.Parse(fields[lonIDX]);
                    double azi = Double.Parse(fields[aziIDX]);
                    StressCoordinate item = new StressCoordinate(lat, lon);
                    item.Columns = fields;
                    item.Azimuth = azi;
                    item.Table = this;
                    if (item == null) continue;
                    _collection.Add(item);
                    GlobalZone zone = GetZone(lat, lon);
                    zone.Add(item);
                }
            }



            // neighbors. don't add a neighbor if theres nothing in it!
            for (int i = 0; i < Zone.Count; i++)
            {
                // by layer...
                List<GlobalZone> _layer = Zone[i];
                List<GlobalZone> _layerA = i > 0 ? Zone[i - 1] : null;
                List<GlobalZone> _layerB = i < Zone.Count - 1 ? Zone[i + 1] : null;

                for (int j = 0; j < nLonDivs; j++)
                {
                    GlobalZone gz0 = _layer[j];
                    int jA = j == 0 ? (nLonDivs - 1) : j - 1;
                    int jB = j == (nLonDivs - 1) ? 0 : j + 1;

                    if (i == 0 || i == Zone.Count - 1)
                    {
                        // polar layer (shared N or south pole)
                        foreach (GlobalZone gzNN in _layer)
                        {
                            if (gzNN != gz0)
                                gz0.AddNeighbor(gzNN);
                        }
                    }
                    else
                    {
                        // normal case (nn to sides)
                        gz0.AddNeighbor(_layer[jA]);
                        gz0.AddNeighbor(_layer[jB]);
                    }

                    if (_layerA != null)
                    {
                        gz0.AddNeighbor(_layerA[j]);
                        gz0.AddNeighbor(_layerA[jA]);
                        gz0.AddNeighbor(_layerA[jB]);
                    }

                    if (_layerB != null)
                    {
                        gz0.AddNeighbor(_layerB[j]);
                        gz0.AddNeighbor(_layerB[jA]);
                        gz0.AddNeighbor(_layerB[jB]);
                    }
                }

            }

        }



        public int GetIDX(string v)
        {
            for (int i = 0; i < _headers.Length; i++)
                if (_headers[i] == v) return (i);
            return (-1);
        }

        public StressCoordinate GetNearest(double lat, double lon)
        {
            Double minDist = Double.MaxValue;
            StressCoordinate near = null;
            foreach (var item in _collection)
            {
                double dist = item.GetDistanceTo(lat, lon);
                if (dist < minDist)
                {
                    minDist = dist;
                    near = item;
                }
            }
            return (near);
        }

        internal static void ProjectToTangentPlane(OrthographicProjection ortho, List<StressCoordinate> list)
        {
            if (list == null) return;
            foreach (var sC in list)
            {
                sC.Draw = false;
                if (ortho.ToTangentPlane(sC.Latitude, sC.Longitude, out double x, out double y))
                {
                    sC.ProjectedX = x;
                    sC.ProjectedY = y;
                    sC.Draw = true;
                }
            }
        }

        public void ProjectToTangentPlane(OrthographicProjection ortho, string filter)
        {
            // all zones
            double x, y;
            if (String.IsNullOrWhiteSpace(filter)) filter = null;

            // all vectors default don't draw
            foreach (StressCoordinate sC in _collection) sC.Draw = false;

            int nempty = 0;
            int nvisible = 0;
            int ntotal = 0;
            // by zones
            foreach (var zLayer in this.Zone)
            {
                foreach (var zone in zLayer)
                {
                    zone.Draw = false;
                    ntotal++;
                    if (zone.Empty == false)
                    {
                        zone.MinProjectedX = zone.MinProjectedY = double.MaxValue;
                        zone.MaxProjectedX = zone.MaxProjectedY = double.MinValue;
                        foreach (var coord in zone.Corners)
                        {
                            if (ortho.ToTangentPlane(coord.Latitude, coord.Longitude, out x, out y))
                            {
                                coord.ProjectedX = x;
                                coord.ProjectedY = y;
                                zone.MinProjectedX = Math.Min(zone.MinProjectedX, x);
                                zone.MaxProjectedX = Math.Max(zone.MaxProjectedX, x);
                                zone.MinProjectedY = Math.Min(zone.MinProjectedY, y);
                                zone.MaxProjectedY = Math.Max(zone.MaxProjectedY, y);
                                zone.Draw = true;
                            }
                        }

                        if (zone.Draw)
                        {
                            nvisible++;
                            foreach (var sC in zone.Collection)
                            {
                                if (sC.Filter(filter) &&
                                    ortho.ToTangentPlane(sC.Latitude, sC.Longitude, out x, out y))
                                {
                                    sC.ProjectedX = x;
                                    sC.ProjectedY = y;
                                    sC.Draw = true;
                                }
                            }
                        }
                    }
                    else nempty++;
                }
            }

        }

        internal List<StressCoordinate> GetFocusedStressCoordinates(double lat, double lon, double radius, string filter)
        {
            if (radius <= 0.0 && String.IsNullOrWhiteSpace(filter)) return (_collection);

            List<StressCoordinate> ret = new List<StressCoordinate>();
            foreach (StressCoordinate sC in _collection)
            {
                if (!sC.Filter(filter)) continue;

                if (radius > 0.0)
                {
                    double dist = sC.GetDistanceTo(lat, lon) / 1000.0; //km
                    if (dist > radius) continue;
                }
                ret.Add(sC);
            }
            return (ret);
        }


        internal double GetInterpolatedAzimuth(double lat, double lon, double maxRad, int nn, string filter, out List<StressCoordinate> retCloseList)
        {
            List<StressCoordinate> closeList = new List<StressCoordinate>();
            retCloseList = closeList;

            // get zones we want 
            GlobalZone zone = GetZone(lat, lon);
            if (zone == null) return (Double.NaN);

            // neighboring zones (if any)
            List<GlobalZone> allZone = new List<GlobalZone> { zone };
            allZone.AddRange(zone.NeighborCollection);

            foreach (GlobalZone gZ in allZone)
            {
                foreach (StressCoordinate sC in gZ.Collection)
                {
                    if (!sC.Filter(filter)) continue;
                    if (sC.Azimuth >= 999.0) continue; // dont use NULLS!!
                    double dist = sC.GetDistanceTo(lat, lon) / 1000.0; //km
                    if (dist > maxRad) continue;
                    _appendToCapacity(sC, closeList, dist, nn);
                }
            }

            // now figure out weighted mean 
            double numX = 0.0;
            double numY = 0.0;
            double den = 0.0;
            foreach (StressCoordinate t in closeList)
            {
                double weight = (1.0 / t.Distance);
                double azRad = GeoCoordinateUtilities.ToRadians(t.Azimuth);
                double xc = Math.Sin(azRad);
                double yc = Math.Cos(azRad);
                double numpartX = xc * weight; // closer = bigger weight
                double numpartY = yc * weight; // closer = bigger weight
                numX += numpartX;
                numY += numpartY;
                den += weight;
            }

            numX = numX / den;
            numY = numY / den;
            double az = GeoCoordinateUtilities.ToDegrees(Math.Atan2(numX, numY));

            while (az < 0.0) az += 180.0;
            while (az > 180.0) az -= 180.0;

            return az;
        }
        private void _appendToCapacity(StressCoordinate item, List<StressCoordinate> ret, double dist, int nn)
        {
            item.Distance = dist;

            // is this closer than any?
            int insert = -1;
            for (int i = 0; i < ret.Count; i++)
            {
                if (item.Distance < ret[i].Distance)
                {
                    insert = i;
                    break;
                }
            }

            if (insert != -1)
                ret.Insert(insert, item);
            else
                ret.Add(item);

            // now remove the last one if it got too big
            if (ret.Count > nn)
                ret.RemoveAt(nn);
        }







    }

    public class MapLine
    {
        public MapLine(string name)
        {
            Name = name;
        }
        public List<BaseCoordinate> Collection { get; set; } = new List<BaseCoordinate>();
        public string Name { get; }

        internal void Add(double lat, double lon)
        {
            Collection.Add(new BaseCoordinate(lat, lon));
        }
    }
}

