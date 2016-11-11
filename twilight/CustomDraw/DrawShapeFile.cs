using System;
using System.Collections.Generic;
using System.Drawing;

namespace twilight
{
	public class DrawShapeFile:DrawGeometry
	{
		public DrawShapeFile ()
		{
		}

		Envelope GetMapEnv ()
		{
			return new Envelope (-180, 180, -90, 90);
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
	}
}

