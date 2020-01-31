using ChartLib;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using UTM_LL;
using Excel = Microsoft.Office.Interop.Excel;

namespace WorldStressMapTool
{
    public partial class Form1 : Form, IMessageFilter
    {
        readonly StressTable _stressTable;
        readonly List<StressCoordinate> _myList = new List<StressCoordinate>();
        double _zoomRadius;
        readonly List<Image> _azimImage;
        List<Image> _azimImageUser;
        readonly List<Image> _azimImageClose;
        readonly int _azimInc = 2;
        int _azimCount;
        int _screenDim;
        int _scX, _scY;
        double _cFactor;
        float _worldScreenRadius;
        private double _screenRad;
        double _xviz0, _xviz1, _yviz0, _yviz1;
        int _mdsx, _mdsy;
        OrthographicProjection _ortho;
        OrthographicProjection _mdOrtho;
        readonly Brush _seaBrush = Brushes.Navy;
        private Brush _radBrush;
        readonly WSMMap _rwmap;
        private List<MapLine> mapLines;
        readonly Pen _mapLinePen;
        readonly Pen _mapThickPen;
        List<StressCoordinate> _closeList;
        private WSMCountry _mouseOverCountry;
        readonly Brush _activeTransBrush;
        private readonly TooltipContainer _ttC;
        private readonly Layer _layerMap;
        private readonly Layer _layerStress;
        private readonly Layer _layerDynamic1;
        private readonly Layer _layerDynamic2;
        private double _pickRadius = 200;
        private int _nVect = 5;
        private Point _countryAnnotPos;
        private WSMCities _citiesMAJOR;
        private WSMCities _citiesALL;

        public Form1()
        {
            InitializeComponent();
            Application.AddMessageFilter(this);
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;
            textBoxPickRadius.Text = _pickRadius.ToString();
            textBoxMaxVect.Text = _nVect.ToString();
            _stressTable = new StressTable();
            _azimImage = _buildAZIM(Color.FromArgb(100, Color.White));
            _azimImageUser = _buildAZIM(Color.HotPink);
            _azimImageClose = _buildAZIM(Color.Orange);
            pictureBox1.BackColor = Color.Black;
            _layerMap = new Layer(_paintMAP);
            _layerStress = new Layer(_paintSTRESS);
            _layerDynamic1 = new Layer(_paintDYNAMIC1);
            _layerDynamic2 = new Layer(_paintDYNAMIC2);
            _activeTransBrush = new SolidBrush(Color.FromArgb(100, Color.Red));
            _radBrush = new SolidBrush(Color.FromArgb(50, Color.White));
            _mapLinePen = new Pen(Color.FromArgb(50, Color.LightSteelBlue));
            _mapThickPen = new Pen(Color.FromArgb(200, Color.LightSteelBlue));
            _rwmap = new WSMMap();
            _rwmap.Color(RegionInfo.CurrentRegion.EnglishName, Brushes.Orchid);
            _citiesMAJOR = new WSMCities(Properties.Resources.citiesMAJOR);
            _citiesALL = new WSMCities(Properties.Resources.citiesALL);
            _ttC = new TooltipContainer(pictureBox1);
            Width = 1000;
            Height = 1000;

            _mapLines();

            _updateProjection();
            toolStripDropDownButton1.SelectedIndex = 0;
        }

        // setup
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x20a)
            {
                // WM_MOUSEWHEEL, find the control at screen position m.LParam
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                IntPtr hWnd = WindowFromPoint(pos);
                if (hWnd != IntPtr.Zero && hWnd != m.HWnd && Control.FromHandle(hWnd) != null)
                {
                    SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }
            return false;
        }

        // P/Invoke declarations
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        private List<Image> _buildAZIM(Color color)
        {
            Pen pen = new Pen(color);
            List<Image> ret = new List<Image>();
            Bitmap bm;
            Graphics g;
            for (int azim = 0; azim <= 180; azim += _azimInc)
            {
                bm = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                g = Graphics.FromImage(bm);
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TranslateTransform(8, 8);
                g.RotateTransform(azim);
                g.DrawLine(pen, -0, 7, -0, -7);
                g.DrawRectangle(pen, -2f, -2f, 4f, 4f);
                g.ResetTransform();
                ret.Add(bm);
            }
            _azimCount = ret.Count;

            // null azimuth special case (999)
            Brush brush = new SolidBrush(Utilities.ColorUtilities.GetContrastColor(color));
            bm = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            g = Graphics.FromImage(bm);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TranslateTransform(8, 8);
            g.FillRectangle(brush, -2f, -2f, 4f, 4f);
            g.DrawRectangle(pen, -2f, -2f, 4f, 4f);
            ret.Add(bm);
            return (ret);
        }

        private void _mapLines()
        {
            mapLines = new List<MapLine>();

            // lat
            int n = 36;
            double inc = 360 / n;
            for (int lat = -90; lat <= 90; lat += 10)
            {
                MapLine mp = new MapLine("Lat:" + lat);
                mapLines.Add(mp);
                for (int i = 0; i < n + 1; i++)
                    mp.Add(lat, -180 + i * inc);
            }

            // lon
            n = 36;
            inc = 180 / 36;
            for (int lon = -180; lon < 180; lon += 10)
            {
                MapLine mp = new MapLine("Lon:" + lon);
                mapLines.Add(mp);
                for (int i = 0; i < n + 1; i++)
                    mp.Add(-90 + i * inc, lon);
            }
        }



        private void _updateProjection()
        {
            double x, y;

            double lat = trackBar1.Value / 100.0;
            labelLAT.Text = lat.ToString(CultureInfo.InvariantCulture);
            labelLAT.Refresh();
            double lon = trackBar2.Value / 100.0;
            labelLONG.Text = lon.ToString(CultureInfo.InvariantCulture);
            labelLONG.Refresh();
            _zoomRadius = trackBar3.Value;
            labelRAD.Text = _zoomRadius.ToString(CultureInfo.InvariantCulture);
            labelRAD.Refresh();


            _ortho = new OrthographicProjection(lat, lon);

            _stressTable.ProjectToTangentPlane(_ortho, textBoxFILTER.Text);

            // extras
            StressTable.ProjectToTangentPlane(_ortho, _myList);
            StressTable.ProjectToTangentPlane(_ortho, _closeList);

            foreach (var mLine in mapLines)
            {
                foreach (var item in mLine.Collection)
                {
                    if (_ortho.ToTangentPlane(item.Latitude, item.Longitude, out x, out y))
                    {
                        item.ProjectedX = x;
                        item.ProjectedY = y;
                    }
                    else
                    {
                        item.ProjectedX = double.NaN;
                        item.ProjectedY = double.NaN;
                    }
                }
            }

            if (cities != null)
                foreach (NamedCoordinate nC in cities.Collection)
                {
                    if (_ortho.ToTangentPlane(nC.Latitude, nC.Longitude, out x, out y))
                    {
                        nC.ProjectedX = x;
                        nC.ProjectedY = y;
                    }
                    else
                    {
                        nC.ProjectedX = double.NaN;
                        nC.ProjectedY = double.NaN;
                    }
                }

            foreach (WSMCountry cDef in _rwmap.Collection)
            {
                var cc = cDef.CentralCoordinate;
                if (_ortho.ToTangentPlane(cc.Latitude, cc.Longitude, out x, out y))
                {
                    cc.ProjectedX = x;
                    cc.ProjectedY = y;
                }
                else
                {
                    cc.ProjectedX = double.NaN;
                    cc.ProjectedY = double.NaN;
                }

                foreach (WSMPolygon poly in cDef.Collection)
                {
                    int nOk = 0;
                    poly.MinX = poly.MinY = double.MaxValue;
                    poly.MaxX = poly.MaxY = double.MinValue;
                    foreach (BaseCoordinate item in poly.Collection)
                    {
                        if (_ortho.ToTangentPlane(item.Latitude, item.Longitude, out x, out y))
                        {
                            item.ProjectedX = x;
                            item.ProjectedY = y;
                            nOk++;
                            poly.MinX = Math.Min(poly.MinX, x);
                            poly.MinY = Math.Min(poly.MinY, y);
                            poly.MaxX = Math.Max(poly.MaxX, x);
                            poly.MaxY = Math.Max(poly.MaxY, y);
                        }
                        else
                        {
                            item.ProjectedX = double.NaN;
                            item.ProjectedY = double.NaN;
                        }
                    }
                    poly.Draw = nOk >= 3;
                }
            }


            _invalidateAllLayers(false);
        }


        WSMCities cities
        {
            get
            {
                var sel = toolStripDropDownButton1.SelectedItem;
                if (sel == "Major") return (_citiesMAJOR);
                if (sel == "All") return (_citiesALL);
                return (null);
            }
        }

        void _invalidateAllLayers(Boolean immediate)
        {
            _layerMap.Dirty = true;
            _layerStress.Dirty = true;
            _layerDynamic1.Dirty = true;
            _layerDynamic2.Dirty = true;
            if (immediate)
                pictureBox1.Refresh();
            else
                pictureBox1.Invalidate();
        }

        // events
        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            _layerMap.Resize(pictureBox1.Size);
            _layerStress.Resize(pictureBox1.Size);
            _layerDynamic1.Resize(pictureBox1.Size);
            _layerDynamic2.Resize(pictureBox1.Size);
            pictureBox1.Invalidate();
        }

        private void textBoxFILTER_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            _rwmap.Color();
            _rwmap.Color(RegionInfo.CurrentRegion.EnglishName, Brushes.Orchid);
            _azimImageUser = _buildAZIM(Color.HotPink);
            _mapLines();
            _updateProjection();
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            _mdOrtho = _ortho;
            _updateProjection();
            _invalidateAllLayers(true);
        }

        private void trackBar_MouseUp(object sender, MouseEventArgs e)
        {
            _mdOrtho = null;
            _invalidateAllLayers(true);
        }

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            int v = trackBar3.Value - e.Delta;
            if (v < trackBar3.Minimum) v = trackBar3.Minimum;
            if (v > trackBar3.Maximum) v = trackBar3.Maximum;
            trackBar3.Value = v;
            _updateProjection();
            _invalidateAllLayers(true);
        }
        private void textBoxFILTER_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                _updateProjection();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // reset
            _myList.Clear();
            _updateProjection();
        }


        // ReSharper disable once MemberCanBePrivate.Global
        public StressCoordinate PickStressCoordinate(Point loc, StressCoordinate useSc, out List<StressCoordinate> closeList)
        {
            double x, y, lat, lon;
            _getXY(loc, out x, out y);
            _ortho.ToSphere(x, y, out lat, out lon);
            if (!double.IsNaN(lat) && !double.IsNaN(lon))
            {
                StressCoordinate sC;
                if (useSc == null) sC = new StressCoordinate(lat, lon);
                else
                {
                    sC = useSc;
                    sC.Latitude = lat;
                    sC.Longitude = lon;
                    sC.ProjectedX = 0.0;
                    sC.ProjectedY = 0.0;
                }

                sC.Azimuth = _stressTable.GetInterpolatedAzimuth(lat, lon, _pickRadius, _nVect, null, out closeList);
                if (!Double.IsNaN(sC.Azimuth)) return (sC);
            }
            closeList = null;
            return (null);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public StressCoordinate PickStressCoordinate(StressCoordinate sC)
        {
            List<StressCoordinate> closeList;
            Point loc = new Point((int)sC.ProjectedX, (int)sC.ProjectedY);
            StressCoordinate retSc = PickStressCoordinate(loc, sC, out closeList);
            sC.Draw = (retSc != null);
            return (retSc);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (toolStripButtonPICK.Checked)
            {
                if (ModifierKeys == Keys.Control)
                {
                    // use the actual stress coordinate to make a respository
                    // for the input/output so no allocation req in parallel threads
                    // remove bad ones after
                    List<StressCoordinate> buffer = new List<StressCoordinate>();
                    for (int dx = -100; dx <= 100; dx += 20)
                        for (int dy = -100; dy <= 100; dy += 20)
                        {
                            StressCoordinate sC = new StressCoordinate(0, 0)
                            {
                                ProjectedX = e.X + dx,
                                ProjectedY = e.Y + dy,
                                Draw = true
                            };
                            buffer.Add(sC);
                        }

                    Parallel.For(0, buffer.Count,
                   index =>
                   {
                       PickStressCoordinate(buffer[index]);
                   });
                    buffer.RemoveAll(sC => (!sC.Draw));
                    if (buffer.Count > 0) _myList.AddRange(buffer);
                    _closeList = null;
                    _updateProjection();
                }
                else
                {
                    StressCoordinate sC = PickStressCoordinate(e.Location, null, out _closeList);
                    if (sC != null) _myList.Add(sC);
                    _updateProjection();
                }
            }
        }



        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (toolStripButtonPICK.Checked) return;
            _mdsx = e.Location.X;
            _mdsy = e.Location.Y;
            _mdOrtho = _ortho;
        }

        private string _pointerText;
        private GeoCoordinate _pointerGeoCoordinate;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            double x, y, lat, lon;
            _getXY(e.Location, out x, out y);
            _ortho.ToSphere(x, y, out lat, out lon);
            string text = null;
            WSMCountry cDef = _ttC.GetDataAtPointer(e) as WSMCountry;
            string country = (cDef == null ? "Sea" : cDef.Name);
            if (!double.IsNaN(lat) && !double.IsNaN(lon))
            {
                text = "Latitude:" + GeoCoordinateUtilities.FormatLatitude(lat) + "   Longitude:" +
                       GeoCoordinateUtilities.FormatLongitude(lon);
                _pointerGeoCoordinate = new GeoCoordinate(lat, lon);
            }
            else
            {
                _pointerGeoCoordinate = null;
                country = "SPACE";
            }
            text += ("   [" + country + "]");
            _pointerText = text;

            if (!toolStripButtonPICK.Checked && _mdOrtho != null)
            {
                // moving view
                double deltaDeg = 90.0 / _worldScreenRadius;
                double φ0 = _mdOrtho.φ0 + deltaDeg * (e.Location.Y - _mdsy);
                if (φ0 > 90) φ0 = 90;
                if (φ0 < -90) φ0 = -90;
                double λ0 = _mdOrtho.λ0 - deltaDeg * (e.Location.X - _mdsx);
                λ0 = OrthographicProjection.SanitizeLongitude(λ0);
                trackBar1.Value = (int)Math.Round(φ0 * 100);
                trackBar2.Value = (int)Math.Round(λ0 * 100);
                _updateProjection();
                _invalidateAllLayers(true);
                return;
            }

            // not moving view
            _mouseOverCountry = cDef;
            _layerDynamic1.Dirty = true;
            _layerDynamic2.Dirty = true;
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _mdOrtho = null;
            _layerStress.Dirty = true;
            pictureBox1.Invalidate();
        }


        // draw
        private void _getXY(Point location, out double x, out double y)
        {
            x = (location.X - _scX) / _cFactor;
            y = -(location.Y - _scY) / _cFactor;
        }
        PointF _convertF(BaseCoordinate fC)
        {
            return (new PointF((float)(_scX + fC.ProjectedX * _cFactor), (float)(_scY - fC.ProjectedY * _cFactor)));
        }
        Point _convert(double x, double y)
        {
            return (new Point((int)Math.Round(_scX + x * _cFactor), (int)Math.Round(_scY - y * _cFactor)));
        }
        PointF _convertF(double x, double y)
        {
            return (new PointF((float)(_scX + x * _cFactor), (float)(_scY - y * _cFactor)));
        }
        PointF[] _getPoly(WSMPolygon poly)
        {
            var _poly = new List<PointF>();
            foreach (BaseCoordinate item in poly.Collection)
                if (!double.IsNaN(item.ProjectedX)) _poly.Add(_convertF(item));
            if (_poly.Count <= 2) return (null);
            return (_poly.ToArray());
        }

        PointF[] _getLine(MapLine mLine)
        {
            List<PointF> poly = new List<PointF>();
            foreach (BaseCoordinate item in mLine.Collection)
                poly.Add(!double.IsNaN(item.ProjectedX) ? _convertF(item) : new PointF(float.NaN, float.NaN));
            return (poly.ToArray());
        }

        private void toolStripButton1_CheckedChanged(object sender, EventArgs e)
        {
            // show?
            _updateProjection();
            _layerStress.Dirty = true;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (_zoomRadius <= 0.0) return;

            // place zero radius at center
            // smallest is??
            _screenDim = Math.Min(pictureBox1.Width, pictureBox1.Height) - 6; // a border
            _scX = pictureBox1.Width / 2;
            _scY = pictureBox1.Height / 2;
            _cFactor = _screenDim / (_zoomRadius * 2);
            _worldScreenRadius = (float)(_cFactor * _ortho.Radius);
            _screenRad = Math.Sqrt(pictureBox1.Width * pictureBox1.Width + pictureBox1.Height * pictureBox1.Height) / 2;

            double x, y;
            int bdr = 20;
            _getXY(new Point(-bdr, -bdr), out x, out y);
            _xviz0 = x;
            _yviz1 = y;
            _getXY(new Point(pictureBox1.Width + bdr, pictureBox1.Height + bdr), out x, out y);
            _xviz1 = x;
            _yviz0 = y;

            _layerMap.Paint(e.Graphics);

            _layerDynamic1.Paint(e.Graphics);
            _layerStress.Paint(e.Graphics);
            _layerDynamic2.Paint(e.Graphics);


        }



        private void _paintMAP(Graphics g)
        {
            _ttC.Clear();
            _ttC.Active = false;

            if (_screenRad < _worldScreenRadius && _screenRad < _worldScreenRadius)
                g.FillRectangle(_seaBrush, 0, 0, pictureBox1.Width, pictureBox1.Height);
            else
                g.FillEllipse(_seaBrush, _scX - _worldScreenRadius, _scY - _worldScreenRadius, _worldScreenRadius * 2, _worldScreenRadius * 2);

            // settle for fast and dirty draw!
            foreach (WSMCountry cDef in _rwmap.Collection)
            {
                Region reg = _internalBorderRegion(cDef);
                // fill
                if (reg != null)
                {
                    g.FillRegion(cDef.Brush, reg);
                    _ttC.Add(reg, cDef.Name, cDef);
                }
                _drawCountryOutline(g, cDef, Pens.Gold);
            }

            if (cities != null)
            {
                int maxX = pictureBox1.Width + 20;
                int maxY = pictureBox1.Height + 20;
                int nCitiesDrawn = 0;
                g.TranslateTransform(-1, -1);
                foreach (NamedCoordinate nC in cities.Collection)
                {
                    if (double.IsNaN(nC.ProjectedX)) continue;
                    PointF pt = _convertF(nC);

                    if (pt.X < -20) continue;
                    if (pt.X > maxX) continue;
                    if (pt.Y > maxY) continue;
                    if (pt.Y < -20) continue;

                    g.FillRectangle(Brushes.Brown, pt.X, pt.Y, 3, 3);

                    _drawStandoutString(g, nC.Name, Brushes.Beige, (int)pt.X + 3, (int)pt.Y);
                    nCitiesDrawn++;
                }
                g.ResetTransform();
                _drawStandoutString(g, "NCITIES:" + nCitiesDrawn, Brushes.White, 10, 60);
            }

            foreach (var mLine in mapLines)
            {
                bool stat = ((mLine.Name == "Lat:0") || (mLine.Name == "Lon:0"));
                PointF[] line = _getLine(mLine);
                if (line == null) continue;
                for (int i = 1; i < line.Length; i++)
                {
                    PointF p0 = line[i - 1];
                    if (float.IsNaN(p0.X)) continue;
                    PointF p1 = line[i];
                    if (float.IsNaN(p1.X)) continue;
                    g.DrawLine(stat ? _mapThickPen : _mapLinePen, p0, p1);
                }
            }


        }


        private bool _showNulls;


        private void _paintSTRESS(Graphics g)
        {
            //  if (_mdOrtho != null) return;
            _showNulls = tsbNulls.Checked;
            if (_mdOrtho == null) g.TranslateTransform(-8, -8);
            else g.TranslateTransform(-1, -1);
            int nzonesDrawn = 0;



            if (tsbStress.Checked)
            {
                foreach (var zLayer in _stressTable.Zone)
                {
                    foreach (var zone in zLayer)
                    {
                        if (zone.Draw && _isOnScreen(zone))
                        {
                            nzonesDrawn++;
                            if (_mdOrtho == null) // not in motion
                                _drawStressCoordinates(g, zone.Collection, _azimImage);
                            else
                                _drawStressCoordinateDot(g, zone.Collection, Brushes.White); // in motion, faster draw
                        }
                    }
                }
            }

            g.ResetTransform();
            g.TranslateTransform(-8, -8);
            _drawStressCoordinates(g, _myList, _azimImageUser);
            _drawStressCoordinates(g, _closeList, _azimImageClose);
            g.ResetTransform();

            _drawStandoutString(g, nzonesDrawn + " zones drawn", Brushes.SkyBlue, 2, 10);
        }


        void _drawStandoutString(Graphics g, string str, Brush color, int x, int y)
        {
            if (str == null) return;
            g.DrawString(str, Font, Brushes.Black, x - 1, y);
            g.DrawString(str, Font, Brushes.Black, x + 1, y);
            g.DrawString(str, Font, Brushes.Black, x, y - 1);
            g.DrawString(str, Font, Brushes.Black, x, y + 1);
            g.DrawString(str, Font, color, x, y);
        }


        private bool _isOnScreen(GlobalZone zone)
        {
            if (zone.MinProjectedX > _xviz1) return (false);
            if (zone.MinProjectedY > _yviz1) return (false);
            if (zone.MaxProjectedX < _xviz0) return (false);
            if (zone.MaxProjectedY < _yviz0) return (false);
            return (true);
        }

        Region _internalBorderRegion(WSMCountry cDef)
        {
            Region reg = null;
            foreach (WSMPolygon poly in cDef.Collection)
            {
                if (!poly.Draw) continue;

                if (!poly.IsInView(_zoomRadius * 2))
                    continue;


                var _poly = _getPoly(poly);
                if (_poly == null) continue;

                GraphicsPath gp = new GraphicsPath();
                gp.AddPolygon(_poly);

                if (poly.Polytype == 0)
                {
                    // external
                    if (reg == null) reg = new Region(gp);
                    else reg.Union(gp);
                }
                else
                {
                    // remove internal
                    reg?.Exclude(gp);
                }
            }

            return (reg);
        }


        void _drawCountryOutline(Graphics g, WSMCountry cDef, Pen pen)
        {
            // outlines
            foreach (WSMPolygon poly in cDef.Collection)
            {
                if (!poly.Draw) continue;
                PointF[] _poly = _getPoly(poly);
                if (_poly == null) continue;
                g.DrawLines(pen, _poly);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // show table
            string path = System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllText(path, _stressTable.TableText);

            // Get the Excel application object.
            Excel.Application excel_app = new Excel.Application()
            {

                // Make Excel visible (optional).
                Visible = true
            };

            // Open the file.
            excel_app.Workbooks.Open(
                path,               // Filename
                Type.Missing,
                Type.Missing,
                Excel.XlFileFormat.xlCSV,   // Format
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                ",",          // Delimiter
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing);


        }

        private void toolStripDropDownButton1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _layerMap.Dirty = true;
            _updateProjection();
            pictureBox1.Invalidate();
        }

        private void textBoxPickRadius_TextChanged(object sender, EventArgs e)
        {
            double ret;
            int n;
            if (double.TryParse(textBoxPickRadius.Text, out ret)) _pickRadius = ret;
            if (int.TryParse(textBoxMaxVect.Text, out n)) _nVect = n;
        }





        private void _paintDYNAMIC1(Graphics g)
        {
            if (_mouseOverCountry != null)
            {
                Region reg = _internalBorderRegion(_mouseOverCountry);
                // fill
                if (reg != null) g.FillRegion(_activeTransBrush, reg);
            }
        }

        private void _paintDYNAMIC2(Graphics g)
        {

            Point loc = pictureBox1.PointToClient(MousePosition);

            if (_mouseOverCountry != null)
            {
                // outlines
                _drawCountryOutline(g, _mouseOverCountry, Pens.Red);
                if (Math.Abs(loc.X - _countryAnnotPos.X) > 20 || Math.Abs(loc.Y - _countryAnnotPos.Y) > 20)
                    _countryAnnotPos = loc;
                _drawStandoutString(g, _mouseOverCountry.Name, Brushes.White, _countryAnnotPos.X, _countryAnnotPos.Y - 20);
            }



            if (toolStripButtonPICK.Checked)
            {

                if (_pointerGeoCoordinate != null)
                {
                    List<PointF> ptList = new List<PointF>();
                    double x, y;
                    for (double azimuth = 0; azimuth < 360; azimuth += 10)
                    {
                        GeoCoordinate gc = GeoCoordinateUtilities.GetEndPoint(_pointerGeoCoordinate, azimuth, _pickRadius);
                        if (_ortho.ToTangentPlane(gc.Latitude, gc.Longitude, out x, out y))
                        {
                            PointF pt = _convertF(x, y);
                            ptList.Add(pt);
                        }
                    }
                    if (ptList.Count > 2)
                        g.FillPolygon(_radBrush, ptList.ToArray());
                }
            }

            _drawStandoutString(g, _pointerText, Brushes.White, 2, pictureBox1.Height - 20);
        }



        void _drawStressCoordinates(Graphics g, List<StressCoordinate> list, List<Image> imageList)
        {
            if (list == null) return;

            foreach (var sC in list)
            {
                if (!sC.Draw) continue;

                if (sC.Azimuth >= 999 && _showNulls) // null
                {
                    g.DrawImage(imageList[_azimCount], _convert(sC.ProjectedX, sC.ProjectedY));
                }
                else
                {
                    int idx = (int)Math.Round(sC.Azimuth / _azimInc);
                    if (idx < 0 || idx >= _azimCount) continue;
                    g.DrawImage(imageList[idx], _convert(sC.ProjectedX, sC.ProjectedY));
                }
            }
        }
        void _drawStressCoordinateDot(Graphics g, List<StressCoordinate> list, Brush brush)
        {
            if (list == null) return;
            List<Rectangle> rectList = new List<Rectangle>();
            Size size = new Size(1, 1);
            foreach (var sC in list)
            {
                if (!sC.Draw) continue;
                rectList.Add(new Rectangle(_convert(sC.ProjectedX, sC.ProjectedY), size));
            }
            if (rectList.Count > 0)
                g.FillRectangles(brush, rectList.ToArray());
        }


    }



    delegate void Layerpainteventmethod(Graphics g);

    class Layer
    {
        private Graphics Graphics { get; set; }
        public Bitmap Bitmap { get; set; }
        public Boolean Dirty { private get; set; }
        private readonly Layerpainteventmethod _paint;

        public Layer(Layerpainteventmethod d)
        {
            _paint = d;
        }

        public void Resize(Size size)
        {
            //    Bitmap = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Bitmap = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            Graphics = Graphics.FromImage(Bitmap);

            Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            //
            //            Graphics.CompositingMode = CompositingMode.SourceOver;
            //            Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            //            Graphics.SmoothingMode = SmoothingMode.None;
            //            Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            //            Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;


            Graphics.CompositingMode = CompositingMode.SourceOver;
            Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            Graphics.SmoothingMode = SmoothingMode.None;
            Graphics.PixelOffsetMode = PixelOffsetMode.None;
            Graphics.InterpolationMode = InterpolationMode.Default;

            Dirty = true;
        }


        public void Paint(Graphics g)
        {
            if (Dirty)
            {
                Graphics.Clear(Color.Transparent);
                _paint(Graphics);
            }
            g.DrawImage(Bitmap, 0, 0);
            Dirty = false;
        }
    }
}

