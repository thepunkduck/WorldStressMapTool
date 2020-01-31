using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace WorldStressMapTool
{
    public class WSMMap
    {
        public WSMMap()
        {
            Read();
            Color(17);
        }

        public List<WSMCountry> Collection { get; } = new List<WSMCountry>();


        public void Read()
        {
            Collection.Clear();
            System.IO.StringReader stream = new System.IO.StringReader(Properties.Resources.countryBoundaries);
            using (TextFieldParser parser = new TextFieldParser(stream))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    // Processing row
                    string[] fields = parser.ReadFields();

                    if (fields != null)
                    {
                        string country = fields[6];
                        WSMCountry cDef = new WSMCountry(country) { Brush = null };

                        _initPolyInfo(fields[0]);
                        while (true)
                        {
                            WSMPolygon poly = _getNextPoly();
                            if (poly == null) break;
                            cDef.AddPolygon(poly);
                            if (poly.Polytype == 1) cDef.HasInternalBorders = true;
                        }


                        if (cDef.Collection.Count > 0)
                        {
                            if (cDef.Collection.Count > 1)
                                cDef.Collection.Sort((p1, p2) => (p1.Polytype - p2.Polytype));


                            cDef.Finish();
                            Collection.Add(cDef);
                        }
                    }
                }
            }




        }
        /*
        The four-color mapping algorithm is very complex, with 1476 special cases that you have to handle in your code.
        If you can spare one more colour, the five colour mapping algorithm will meet your requirements, is much simpler, 
        and there is a nice writeup on it at devx.com

        For the special case of a United States map, there are many states with less than five neighbours (e.g., Florida), 
        so you only have to address the first case of the algorithm, which is this:

        1.	Convert the map to a graph (looks like you've done this or are close with your adjacency list)
        2.	Choose one node (state) on the graph with less than five neighbours and remove it from the graph.
        This will reduce the complexity of your graph and may cause some nodes that previously had five or 
        more neighbours to now have less than five.
        3.	Choose another node from the updated graph with less than five neighbours and remove it.
        4.	Continue until you've removed all the nodes from the graph.
        5.	Add the nodes back the graph in reverse order from which you removed them (think stack here).
        6.	Colour the added node with a colour that is not used by any of its current neighbours.
        7.	Continue until you've coloured in the entire graph.

        */

        Random _rand = new Random();

        public void Color(int seed = 0)
        {
            if (seed == 0)
                _rand = new Random();
            else
                _rand = new Random(seed);
            _color();
        }

        void _color()
        {
            //if (!madeNN)
            //{
            //    for (int i = 0; i < _collection.Count; i++)
            //        for (int j = i + 1; j < _collection.Count; j++)
            //        {
            //            if (_collection[i].IsNeighbour(_collection[j]))
            //            {
            //                _collection[i].NeighborList.Add(_collection[j]);
            //                _collection[j].NeighborList.Add(_collection[i]);
            //            }
            //        }
            //    madeNN = true;
            //}

            List<Point> list = new List<Point>();
            for (int i = 0; i < Collection.Count; i++)
            {
                Collection[i].NeighborList.Clear();
                for (int j = i + 1; j < Collection.Count; j++)
                    list.Add(new Point(i, j));
            }
            var sync = new object();
            Parallel.For(0, list.Count, index =>
            {
                Point pt = list[index];
                if (Collection[pt.X].IsNeighbour(Collection[pt.Y]))
                {
                    // 
                    lock (sync)
                    {
                        Collection[pt.X].NeighborList.Add(Collection[pt.Y]);
                        Collection[pt.Y].NeighborList.Add(Collection[pt.X]);
                    }
                }
            });



            // safe copy to retain neighbours
            List<WSMCountry> copyCollection = new List<WSMCountry>();
            foreach (var cDef0 in Collection)
            {
                cDef0.Tag = null;
                cDef0.Brush = null;
                WSMCountry copy = cDef0.ShallowCopy();
                // point to each other
                cDef0.Tag = copy;
                copy.Tag = cDef0;
                copyCollection.Add(copy);
            }

            List<WSMCountry> stack = new List<WSMCountry>();


            // Choose one node (state) on the graph with less than five neighbours and remove it from the graph
            bool inAction = true;
            while (inAction)
            {
                inAction = false;

                for (int nnMin = 4; nnMin >= 0 && !inAction; nnMin--)
                {
                    foreach (var cDef0 in Collection)
                    {
                        if (cDef0.NeighborList.Count == nnMin)
                        {
                            _removeFromCollection(Collection, cDef0);
                            stack.Add(cDef0);
                            inAction = true;
                            break;
                        }
                    }
                }
            }

            // all nodes gone!
            // 5.Add the nodes back the graph in reverse order from which you removed them(think stack here).
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                WSMCountry cDef0 = stack[i];
                Collection.Add(cDef0);
                // resore neighbour list as before
                WSMCountry copy = cDef0.Tag as WSMCountry;
                cDef0.Tag = null;
                if (copy == null)
                { System.Diagnostics.Debug.WriteLine("FAILED!!"); return; }
                cDef0.NeighborList = copy.NeighborList;

                //  6.Colour the added node with a colour that is not used by any of its current neighbours.
                List<Brush> bList = _initRandomBrush();
                int ic = 0;
                while (cDef0.Brush == null && ic < bList.Count)
                {
                    Brush brush = bList[ic++];

                    bool brushOk = true;
                    foreach (var cDef in cDef0.NeighborList)
                    {
                        if (cDef.Brush == null) continue; // not colored yet
                        if (brush == cDef.Brush) brushOk = false; // this one is used
                    }
                    if (brushOk) cDef0.Brush = brush;
                }

                if (cDef0.Brush == null)
                {
                    cDef0.Brush = Brushes.DarkSalmon;
                    System.Diagnostics.Debug.WriteLine("FAILED!!" + cDef0.Name);
                }


            }


            // islands.. find best color (next furthest away from anything else)
            foreach (var cDef0 in Collection)
            {
                if (cDef0.NeighborList.Count == 0)
                {
                    GeoCoordinate gc0 = cDef0.CentralCoordinate;
                    List<WSMCountry> distanceList = new List<WSMCountry>();
                    // get list of nearest countries
                    foreach (var cDef1 in Collection)
                    {
                        if (cDef0 == cDef1) continue;
                        GeoCoordinate gc1 = cDef1.CentralCoordinate;
                        double distance = gc0.GetDistanceTo(gc1);
                        distanceList.Add(cDef1);
                        cDef1.Distance = distance;

                    }

                    // order near to far
                    distanceList.Sort(delegate (WSMCountry p1, WSMCountry p2)
                           {
                               if (p1.Distance > p2.Distance) return (1);
                               if (p2.Distance > p1.Distance) return (-1);
                               return (0);
                           });

                    // full colour list 
                    List<Brush> bList = _initRandomBrush();

                    // remove colours until only one remains
                    // that color is furthest
                    foreach (WSMCountry cDef1 in distanceList)
                    {
                        bList.Remove(cDef1.Brush);
                        if (bList.Count == 1) break;
                    }

                    cDef0.Brush = bList[0];
                }


            }
        }

        internal void Color(string countryName, Brush brush)
        {
            foreach (var cDef in Collection)
            {
                if (String.Equals(cDef.Name, countryName, StringComparison.InvariantCultureIgnoreCase))
                {
                    cDef.Brush = brush;
                    break;
                }
            }
        }

        private void _removeFromCollection(List<WSMCountry> collection, WSMCountry cDef0)
        {
            collection.Remove(cDef0);
            // also remove all occurances in neightbour lists
            foreach (var cDef1 in collection)
            {
                cDef1.NeighborList.RemoveAll(cd => object.Equals(cd, cDef0));
            }
        }


        private List<Brush> _initRandomBrush()
        {
            List<Brush> brushList = new List<Brush>
            {
                Brushes.DarkOliveGreen,
                Brushes.DarkKhaki,
                Brushes.Olive,
                Brushes.LightGreen,
                Brushes.YellowGreen
            };
            brushList.Shuffle();
            return (brushList);
        }




        string _polyInfo;
        int _polyPos;
        private void _initPolyInfo(string polyInfo)
        {
            _polyInfo = polyInfo;
            _polyPos = 0;
        }

        readonly string _outer = "<outerBoundaryIs>";
        readonly string _inner = "<innerBoundaryIs>";
        readonly string _head = @"<LinearRing><coordinates>";
        readonly string _tailI = @"</coordinates></LinearRing></innerBoundaryIs>";
        readonly string _tailO = @"</coordinates></LinearRing></outerBoundaryIs>";
        private WSMPolygon _getNextPoly()
        {
            WSMPolygon poly = new WSMPolygon();
            int idx0 = _polyInfo.IndexOf(_head, _polyPos, StringComparison.Ordinal);
            if (idx0 == -1) return (null);
            // which head?
            string tail;
            if (_polyInfo.Substring(idx0 - _outer.Length, _outer.Length) == _outer)
            { tail = _tailO; poly.Polytype = 0; }
            else if (_polyInfo.Substring(idx0 - _inner.Length, _inner.Length) == _inner)
            { tail = _tailI; poly.Polytype = 1; }
            else
                return (null);

            int idx1 = _polyInfo.IndexOf(tail, _polyPos + _head.Length, StringComparison.Ordinal);
            if (idx1 == -1) return (null);

            idx0 = idx0 + _head.Length;
            _polyPos = idx1 + tail.Length;

            string coords = _polyInfo.Substring(idx0, idx1 - idx0);

            string[] part = coords.Split(", ".ToArray());



            for (int i = 0; i < part.Length; i += 3)
            {
                try
                {
                    poly.AddCoordinate(double.Parse(part[i + 1]), double.Parse(part[i]));
                }
                catch
                {
                    // ignored
                }
            }



            if (poly.Collection.Count > 2) return (poly);
            else return (null);
        }


    }


    static class MyExtensions
    {
        private static readonly Random Rng = new Random(17);
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}