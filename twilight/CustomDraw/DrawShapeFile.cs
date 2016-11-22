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

		void DrawShapefile (Graphics g,String ShpFileName,IStyle style=null)
		{
			//read
			List<IGeometry> GeoList = ReadShpFile(ShpFileName);
			if (GeoList.Count == 0)
			{
				return;
			}
			//transform
			EnumGeoType GeoType = GeoList[0].GeoType;
			List<IGeometry> GeoListFs = (List<IGeometry>)TransGeometry(GeoList);
		
			//draw
			switch (GeoType)
			{
				case EnumGeoType.Point:
					DrawPoint(g, GeoListFs,style as PointStyle );
					break;
				case EnumGeoType.Polyline:
					DrawPolyline(g, GeoListFs, style as LineStyle);
					break;
				case EnumGeoType.Polygon:
					DrawPolygon(g,GeoListFs, style as FillStyle );
					break;
				default:
					break;
			} 
		}

		EnumGeoType GetGeoType(string ShpFile)
		{
			if (!System.IO.File.Exists(ShpFile))
			{
				return EnumGeoType.Unknown;
			}
			ShpReader ShpRd = new ShpReader(ShpFile);
			return ShpRd.GeoType;
		}

		void DrawPoint(Graphics g, List<IGeometry> layer, PointStyle style = null)
		{
			PointStyle mystyle = style;
			if (mystyle ==null )
			{
				mystyle = m_pointstyle;
			}
			float pointsize = (float)mystyle.PointSize ;
			Brush PointBrush = new SolidBrush(mystyle.PointColor);
			Pen PointPen = new Pen(new SolidBrush(mystyle.PointColor));
			 
				foreach (var geometry in layer)
				{
					List<Point> PointList = new List<Point>();
					if (geometry.GeoType == EnumGeoType.Point)
					{
						PointList.Add(geometry as Point);
					}
					else {
						PointList = (geometry as MultiPoint).PointList;
					}
					foreach (var point in PointList)
					{
						RectangleF rf = new RectangleF((float)point.X - pointsize, (float)point.Y - pointsize,
														(float)pointsize * 2, (float)pointsize * 2);
						g.FillEllipse(PointBrush, rf);
						g.DrawEllipse(PointPen, rf);
					}
				}
			 
			PointBrush.Dispose();
			PointPen.Dispose();
		}

		void DrawPolyline(Graphics g, List<IGeometry> layer, LineStyle style = null)
		{ 
			LineStyle mystyle = style;
			if (mystyle == null)
			{
				mystyle = m_linestyle;
			}
			Pen PolylinePen = new Pen(new SolidBrush(mystyle.LineColor),(float)mystyle.LineWidth);
			
				foreach (var geometry in layer)
				{
					foreach (var segment in (geometry as Polyline).SegmentList)
					{
						g.DrawLines(PolylinePen, ConvertToPointF(segment.PointList).ToArray());
					}
				}
			PolylinePen.Dispose();
			}

		void DrawPolygon(Graphics g,List<IGeometry> layer,FillStyle style=null)
		{
			FillStyle mystyle = style;
			if (mystyle == null)
			{
				mystyle = m_fillstyle;
			}
			Brush PolygonBrush = new SolidBrush(mystyle.FillColor);
			Pen PolygonPen = new Pen(new SolidBrush(mystyle.OutLinestyle.LineColor),
			                         (float)mystyle.OutLinestyle.LineWidth );
			foreach (var geometry in layer)
			{
				foreach (var ring in (geometry as Polygon).RingList)
				{
					g.FillPolygon(PolygonBrush, ConvertToPointF(ring.PointList).ToArray());
					g.DrawPolygon(PolygonPen, ConvertToPointF(ring.PointList).ToArray());
				}
			}
			PolygonBrush.Dispose();
			PolygonPen.Dispose();

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

