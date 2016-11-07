using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace twilight
{
	public class PngWriter
	{
		int m_width = 1280, m_height = 720;
		Color m_BackgroundColor = Color.FromArgb (181, 208, 208);
		String m_DefaultFileName = "screen.png", m_ExeFolder = "";

		public PngWriter ()
		{
			m_ExeFolder = Application.StartupPath;
			//get screen resolution
			Size Resolution = GetScreenRes ();
			this.m_width = Resolution.Width;
			this.m_height = Resolution.Height;
		}

		public bool SavePng (DateTime dt, int width, int height, string pngfile)
		{
			if (width <= 0 || height <= 0) {
				width = this.m_width;
				height = this.m_height;
			}
 
			if (System.IO.Path.GetExtension (pngfile) != ".png") {
				pngfile = m_DefaultFileName;
			}
			using (Bitmap b = new Bitmap(m_width, m_height)) {
				using (Graphics g = Graphics.FromImage(b)) {
					g.SmoothingMode = SmoothingMode.HighQuality;
					g.Clear (m_BackgroundColor);
					//draw shapefile
					DrawShapefile (g);
					//draw sun and twilightline
					DrawSun (g, DateTime.Now);
				}
				b.Save (pngfile, ImageFormat.Png);
			}
			return true;
		}

		public bool SavePng ()
		{
			return SavePng (DateTime.Now, this.m_width, this.m_height, m_DefaultFileName);
		}

		Size GetScreenRes ()
		{
			Rectangle resolution = Screen.PrimaryScreen.Bounds;
			return new Size (resolution.Width, resolution.Height);
		}

		List<Point> TransPoints (List<Point> SrcPoints)
		{
			Envelope Env = GetMapEnv ();

			double xscale = m_width / Env.Width, yscale = m_height / Env.Height;
			List<Point> DestPoints = new List<Point> ();

			foreach (var p in SrcPoints) {
				Point pf = new Point ();
				pf.X = ((p.X - Env.XMin) * xscale);
				pf.Y = (m_height - (p.Y - Env.YMin) * yscale);
				DestPoints.Add (pf);
			}
			return DestPoints;
		}

		List<PointF> TransPointsAsFloat (List<Point> SrcPoints)
		{
			List<Point> DestPoints = TransPoints (SrcPoints);

			return ConvertToPointF (DestPoints);
		}

		List<PointF> ConvertToPointF (List<Point> SrcPoints)
		{
			List<PointF> DestPoints = new List<PointF> ();

			foreach (var item in SrcPoints) {
				PointF pf = new PointF ();
				pf.X = (float)item.X;
				pf.Y = (float)item.Y;
				DestPoints.Add (pf);
			}
			return DestPoints;
		}

		object TransGeometry (List<IGeometry> SrcGeoList)
		{
			List<IGeometry> DestGeoList = new List<IGeometry> ();
			foreach (var item in SrcGeoList) {
				IGeometry pGeometry = null;
				switch (item.GeoType) {
				case EnumGeoType.Envelope:
					List<Point> SrcEnvPoints = new List<Point> ();
					SrcEnvPoints.Add (new Point ((item as Envelope).XMin, (item as Envelope).YMin));
					SrcEnvPoints.Add (new Point ((item as Envelope).XMax, (item as Envelope).YMax));
					List<Point> DestEnvPoints = TransPoints (SrcEnvPoints);

					pGeometry = new Envelope ();
					(pGeometry as Envelope).XMin = DestEnvPoints [0].X;
					(pGeometry as Envelope).YMin = DestEnvPoints [0].Y;
					(pGeometry as Envelope).XMax = DestEnvPoints [1].X;
					(pGeometry as Envelope).YMax = DestEnvPoints [1].Y;
					break;
				case EnumGeoType.Polygon:
					pGeometry = new Polygon ();
					List<IGeometry> SrcRingList = new List <IGeometry> ();
					foreach (var ring in (item as Polygon).RingList) {
						SrcRingList.Add (ring as IGeometry);
					}
					List<IGeometry> DestRingList = (List<IGeometry>)TransGeometry (SrcRingList);
					List<Ring> RingList = new List<Ring> ();
					foreach (var ring in DestRingList) {
						RingList.Add (ring as Ring);
					}
					(pGeometry as Polygon).RingList = RingList;
					break;
				case EnumGeoType.Ring:
					pGeometry = new Ring ();
					(pGeometry as Ring).PointList = TransPoints ((item as Ring).PointList);
					break;
				case EnumGeoType.Point:
					pGeometry = TransPoints (new List<Point> () { item as Point }) [0];
					break;
				case EnumGeoType.MultiPoint:
					pGeometry = new MultiPoint ();
					(pGeometry as MultiPoint).PointList = TransPoints ((item as MultiPoint).PointList);
					break;
				case EnumGeoType.Segment:
					pGeometry = new Segment ();
					(pGeometry as Segment).PointList = TransPoints ((item as Segment).PointList);
					break;
				case EnumGeoType.Polyline:
					pGeometry = new Polyline ();
					List<IGeometry> SrcSegList = new List <IGeometry> ();
					foreach (var seg in (item as Polyline).SegmentList) {
						SrcSegList.Add (seg as IGeometry);
					}
					List<IGeometry> DestSegList = (List<IGeometry>)TransGeometry (SrcSegList);
					List<Segment> SegList = new List<Segment> ();
					foreach (var seg in DestSegList) {
						SegList.Add (seg as Segment);
					}
					(pGeometry as Polyline).SegmentList = SegList;
					break;
				default:
					break;
				}
				if (pGeometry != null) {
					DestGeoList.Add (pGeometry);
				}
			}
			return DestGeoList;
		}

		Envelope GetMapEnv ()
		{
			return new Envelope (-180, 180, -90, 90);
		}

		Envelope GetEnv (List<Point> SrcPoints)
		{
			double xmin = double.MaxValue, xmax = double.MinValue,
			ymin = double.MaxValue, ymax = double.MinValue;

			foreach (var p in SrcPoints) {
				xmin = Math.Min (xmin, p.X);
				xmax = Math.Max (xmax, p.X);
				ymin = Math.Min (ymin, p.Y);
				ymax = Math.Max (ymax, p.Y);
			}

			return new Envelope (xmin, xmax, ymin, ymax);
		}

		void DrawShapefile (Graphics g)
		{
			string[] ShpFileNames = System.IO.Directory.GetFiles (m_ExeFolder, "*.shp");
			List<string> ShpFileList = new List<string> ();
			ShpFileList.AddRange (ShpFileNames);

			string ShpFolder2 = System.IO.Path.Combine (m_ExeFolder, "shape");
			if (System.IO.Directory.Exists (ShpFolder2)) {
				string[] ShpFileNames2 = System.IO.Directory.GetFiles (ShpFolder2, "*.shp");
				ShpFileList.AddRange (ShpFileNames2);
			}

			List<List<IGeometry>> Polygons = new List<List<IGeometry>> (); 
			List<List<IGeometry>> Polylines = new List<List<IGeometry>> (); 
			List<List<IGeometry>> Points = new List<List<IGeometry>> (); 

			//read
			foreach (var shpfile in ShpFileList) {
				List<IGeometry> GeoList = ReadShpFile (shpfile);
				if (GeoList.Count == 0) {
					continue;
				}
				switch (GeoList [0].GeoType) {
				case EnumGeoType.Envelope:
				case EnumGeoType.Polygon:
				case EnumGeoType.Ring:
					Polygons.Add (GeoList);
					break;
				case EnumGeoType.Point:
				case EnumGeoType.MultiPoint:
					Points.Add (GeoList);
					break;
				case EnumGeoType.Segment:
				case EnumGeoType.Polyline:
					Polylines.Add (GeoList);
					break;
				default:
					break;
				}
			}

			//transform
			List<List<IGeometry>> PolygonFs = new List<List<IGeometry>> ();
			List<List<IGeometry>> PolylineFs = new List<List<IGeometry>> ();
			List<List<IGeometry>> PointFs = new List<List<IGeometry>> ();
			foreach (var item in Polygons) {
				PolygonFs.Add ((List<IGeometry>)TransGeometry (item));
			}
			foreach (var item in Polylines) {
				PolylineFs.Add ((List<IGeometry>)TransGeometry (item));
			}
			foreach (var item in Points) {
				PointFs.Add ((List<IGeometry>)TransGeometry (item));
			}
			//draw polygon
			Brush PolygonBrush = new SolidBrush (Color.FromArgb (242, 239, 233));
			Pen PolygonPen = new Pen (new SolidBrush (Color.FromArgb (201, 140, 198)));
			foreach (var layer in PolygonFs) {
				foreach (var geometry in layer) {
					foreach (var ring in (geometry as Polygon).RingList) {
						g.FillPolygon (PolygonBrush, ConvertToPointF (ring.PointList).ToArray ());
						g.DrawPolygon (PolygonPen, ConvertToPointF (ring.PointList).ToArray ());
					}
				}
			}
			//draw polyline
			Pen PolylinePen = new Pen (new SolidBrush (Color.Blue));
			foreach (var layer in PolylineFs) {
				foreach (var geometry in layer) {
					foreach (var segment in (geometry as Polyline).SegmentList) {
						g.DrawLines (PolylinePen, ConvertToPointF (segment.PointList).ToArray ());
					}
				}
			}
			//draw point
			float pointsize = 2;
			Brush PointBrush = new SolidBrush (Color.FromArgb (210, Color.Green));
			Pen PointPen = new Pen (new SolidBrush (Color.Green));
			foreach (var layer in PointFs) {
				foreach (var geometry in layer) {
					List<Point> PointList = new List<Point> ();
					if (geometry.GeoType == EnumGeoType.Point) {
						PointList.Add (geometry as Point);
					} else {
						PointList = (geometry as MultiPoint).PointList;
					}
					foreach (var point in PointList) {
						RectangleF rf = new RectangleF ((float)point.X - pointsize, (float)point.Y - pointsize, 
						                                (float)pointsize * 2, (float)pointsize * 2);
						g.FillEllipse (PointBrush, rf);
						g.DrawEllipse (PointPen, rf);
					}
				}
			}
			PolygonBrush.Dispose ();
			PolygonPen.Dispose ();
			PolylinePen.Dispose ();
			PointBrush.Dispose ();
			PointPen.Dispose ();
		}

		List<IGeometry> ReadShpFile (string ShpFile)
		{
			List<IGeometry> GeoList = new List<IGeometry> ();
			ShpReader ShpRd = new ShpReader (ShpFile);
			int count = ShpRd.FeatureCount;
			for (uint i = 0; i < count; i++) {
				//geometry
				IGeometry g = ShpRd.ReadGeometry (i);
				GeoList.Add (g);
			}
			return GeoList;
		}

		void DrawSun (Graphics g, DateTime dt)
		{
			SunPos sp = new SunPos ();
			Point p = sp.GetSunPos (dt.ToUniversalTime ());
			List<Point> Twlightline = sp.GetTwilightLine ();

			List<PointF> pfList = TransPointsAsFloat (new List<Point> () { p });
			PointF pf = pfList [0];
			float sunsize = 20;
			RectangleF rf = new RectangleF (pf.X - sunsize, pf.Y - sunsize, sunsize * 2, sunsize * 2);
			Brush FillBrush = new SolidBrush (Color.Yellow);
			Pen pen = new Pen (new SolidBrush (Color.Red), 2f);
			g.FillEllipse (FillBrush, rf);
			g.DrawEllipse (pen, rf);

			pen.Dispose ();
			FillBrush.Dispose ();
			//draw twilightline
			DrawTwilightline (g, Twlightline, p.Y > 0);
		}

		void DrawTwilightline (Graphics g, List<Point> Twilightline, bool SunInNorth)
		{
			List<PointF> pfList = TransPointsAsFloat (new List<Point> (Twilightline));

			PointF pf0 = new PointF ();
			PointF pf1 = new PointF ();

			if (SunInNorth) {
				pf0.X = 0;
				pf0.Y = m_height;
				pf1.X = m_width;
				pf1.Y = m_height;
			} else {
				pf0.X = 0;
				pf0.Y = 0;
				pf1.X = m_width;
				pf1.Y = 0;
			}

			pfList.Insert (0, pf0);
			pfList.Add (pf1);
			foreach (var point in pfList) {
				//Console.WriteLine (point.X);
			}

			Brush FillBrush = new SolidBrush (Color.FromArgb (100, Color.Black));
			Pen pen = new Pen (new SolidBrush (Color.Black));

			g.FillPolygon (FillBrush, pfList.ToArray ());
			 
			FillBrush.Dispose ();
			pen.Dispose ();
		}
	}
}

