using System;
using System.Collections.Generic;
using System.Drawing;

namespace twilight
{
	public class DrawGeometry : BaseCustomDraw
	{
		protected PointStyle m_pointstyle;
		protected LineStyle m_linestyle;
		protected FillStyle m_fillstyle;

		public DrawGeometry(int Width, int Height):base(Width,Height)
		{
			m_pointstyle = new PointStyle(new RgbColor(210,0,255,0), 2, EnumPointType.Circle);
			m_linestyle = new LineStyle(new RgbColor(0,0,255), 1);

			LineStyle outline = new LineStyle(new RgbColor(201, 140, 198), 1);
			m_fillstyle = new FillStyle(new RgbColor(242, 239, 233), outline);
		}

		Envelope GetEnv(List<Point> SrcPoints)
		{
			double xmin = double.MaxValue, xmax = double.MinValue,
			ymin = double.MaxValue, ymax = double.MinValue;

			foreach (var p in SrcPoints)
			{
				xmin = Math.Min(xmin, p.X);
				xmax = Math.Max(xmax, p.X);
				ymin = Math.Min(ymin, p.Y);
				ymax = Math.Max(ymax, p.Y);
			}

			return new Envelope(xmin, xmax, ymin, ymax);
		}

		public override void Draw(Graphics g, object obj, IStyle style = null)
		{
			List<IGeometry> GeoList = new List<IGeometry>();
			if (obj == null)
			{
				return;
			}
			if (obj is IGeometry)
			{
				InnerDrawGeometry(g, obj as IGeometry, style);
			}
		}

		public void Draw(Graphics g, List<IGeometry> GeoList, IStyle style = null)
		{
			if (GeoList == null || GeoList.Count == 0)
			{
				return;
			}

			switch (GeoList[0].GeoType)
			{
				case EnumGeoType.Point:
				case EnumGeoType.MultiPoint:
					DrawPoint(g, GeoList, style as PointStyle);
					break;
				case EnumGeoType.Polyline:
					DrawPolyline(g, GeoList, style as LineStyle);
					break;
				case EnumGeoType.Polygon:
					DrawPolygon(g, GeoList, style as FillStyle);
					break;
				default:
					break;
			}
		}

		protected object TransGeometry(List<IGeometry> SrcGeoList)
		{
			List<IGeometry> DestGeoList = new List<IGeometry>();
			foreach (var item in SrcGeoList)
			{
				IGeometry pGeometry = null;
				switch (item.GeoType)
				{
					case EnumGeoType.Envelope:
						List<Point> SrcEnvPoints = new List<Point>();
						SrcEnvPoints.Add(new Point((item as Envelope).XMin, (item as Envelope).YMin));
						SrcEnvPoints.Add(new Point((item as Envelope).XMax, (item as Envelope).YMax));
						List<Point> DestEnvPoints = TransPoints(SrcEnvPoints);

						pGeometry = new Envelope();
						(pGeometry as Envelope).XMin = DestEnvPoints[0].X;
						(pGeometry as Envelope).YMin = DestEnvPoints[0].Y;
						(pGeometry as Envelope).XMax = DestEnvPoints[1].X;
						(pGeometry as Envelope).YMax = DestEnvPoints[1].Y;
						break;
					case EnumGeoType.Polygon:
						pGeometry = new Polygon();
						List<IGeometry> SrcRingList = new List<IGeometry>();
						foreach (var ring in (item as Polygon).RingList)
						{
							SrcRingList.Add(ring as IGeometry);
						}
						List<IGeometry> DestRingList = (List<IGeometry>)TransGeometry(SrcRingList);
						List<Ring> RingList = new List<Ring>();
						foreach (var ring in DestRingList)
						{
							RingList.Add(ring as Ring);
						}
					(pGeometry as Polygon).RingList = RingList;
						break;
					case EnumGeoType.Ring:
						pGeometry = new Ring();
						(pGeometry as Ring).PointList = TransPoints((item as Ring).PointList);
						break;
					case EnumGeoType.Point:
						pGeometry = TransPoints(new List<Point>() { item as Point })[0];
						break;
					case EnumGeoType.MultiPoint:
						pGeometry = new MultiPoint();
						(pGeometry as MultiPoint).PointList = TransPoints((item as MultiPoint).PointList);
						break;
					case EnumGeoType.Segment:
						pGeometry = new Segment();
						(pGeometry as Segment).PointList = TransPoints((item as Segment).PointList);
						break;
					case EnumGeoType.Polyline:
						pGeometry = new Polyline();
						List<IGeometry> SrcSegList = new List<IGeometry>();
						foreach (var seg in (item as Polyline).SegmentList)
						{
							SrcSegList.Add(seg as IGeometry);
						}
						List<IGeometry> DestSegList = (List<IGeometry>)TransGeometry(SrcSegList);
						List<Segment> SegList = new List<Segment>();
						foreach (var seg in DestSegList)
						{
							SegList.Add(seg as Segment);
						}
					(pGeometry as Polyline).SegmentList = SegList;
						break;
					default:
						break;
				}
				if (pGeometry != null)
				{
					DestGeoList.Add(pGeometry);
				}
			}
			return DestGeoList;
		}

		protected void InnerDrawGeometry(Graphics g, IGeometry geometry, IStyle style = null)
		{
			if (geometry == null)
			{
				return;
			}
			switch (geometry.GeoType)
			{
				case EnumGeoType.Point:
				case EnumGeoType.MultiPoint:
					PointStyle pointStyle = style as PointStyle;
					if (pointStyle == null)
					{
						pointStyle = m_pointstyle;
					}
					float pointsize = (float)pointStyle.PointSize;
					Brush PointBrush = new SolidBrush(pointStyle.PointColor);
					Pen PointPen = new Pen(new SolidBrush(pointStyle.PointColor));

					DrawPoint(g, geometry, pointsize, PointBrush, PointPen);
					PointBrush.Dispose();
					PointPen.Dispose();
					break;
				case EnumGeoType.Polyline:
					LineStyle lineStyle = style as LineStyle;
					if (lineStyle == null)
					{
						lineStyle = m_linestyle;
					}
					Pen PolylinePen = new Pen(new SolidBrush(lineStyle.LineColor), (float)lineStyle.LineWidth);

					DrawPolyline(g, geometry, PolylinePen);
					PolylinePen.Dispose();
					break;
				case EnumGeoType.Polygon:
					FillStyle mystyle = style as FillStyle;
					if (mystyle == null)
					{
						mystyle = m_fillstyle;
					}
					Brush PolygonBrush = new SolidBrush(mystyle.FillColor);
					Pen PolygonPen = new Pen(new SolidBrush(mystyle.OutLinestyle.LineColor),
											 (float)mystyle.OutLinestyle.LineWidth);

					DrawPolygon(g, geometry, PolygonBrush, PolygonPen);
					PolygonBrush.Dispose();
					PolygonPen.Dispose();
					break;
				default:
					break;
			}

		}
		void DrawPoint(Graphics g, IGeometry geometry, float pointsize, Brush PointBrush, Pen PointPen)
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

		protected void DrawPoint(Graphics g, List<IGeometry> layer, PointStyle style = null)
		{
			PointStyle mystyle = style;
			if (mystyle == null)
			{
				mystyle = m_pointstyle;
			}
			float pointsize = (float)mystyle.PointSize;
			Brush PointBrush = new SolidBrush(mystyle.PointColor);
			Pen PointPen = new Pen(new SolidBrush(mystyle.PointColor));

			foreach (var geometry in layer)
			{
				DrawPoint(g, geometry, pointsize, PointBrush, PointPen);
			}

			PointBrush.Dispose();
			PointPen.Dispose();
		}

		void DrawPolyline(Graphics g, IGeometry geometry, Pen PolylinePen)
		{
			foreach (var segment in (geometry as Polyline).SegmentList)
			{
				g.DrawLines(PolylinePen, ConvertToPointF(segment.PointList).ToArray());
			}
		}

		protected void DrawPolyline(Graphics g, List<IGeometry> layer, LineStyle style = null)
		{
			LineStyle mystyle = style;
			if (mystyle == null)
			{
				mystyle = m_linestyle;
			}
			Pen PolylinePen = new Pen(new SolidBrush(mystyle.LineColor), (float)mystyle.LineWidth);

			foreach (var geometry in layer)
			{
				DrawPolyline(g, geometry, PolylinePen);
			}
			PolylinePen.Dispose();
		}

		void DrawPolygon(Graphics g, IGeometry geometry, Brush PolygonBrush, Pen PolygonPen)
		{
			foreach (var ring in (geometry as Polygon).RingList)
			{
				g.FillPolygon(PolygonBrush, ConvertToPointF(ring.PointList).ToArray());
				g.DrawPolygon(PolygonPen, ConvertToPointF(ring.PointList).ToArray());
			}
		}
		protected void DrawPolygon(Graphics g, List<IGeometry> layer, FillStyle style = null)
		{
			FillStyle mystyle = style;
			if (mystyle == null)
			{
				mystyle = m_fillstyle;
			}
			Brush PolygonBrush = new SolidBrush(mystyle.FillColor);
			Pen PolygonPen = new Pen(new SolidBrush(mystyle.OutLinestyle.LineColor),
									 (float)mystyle.OutLinestyle.LineWidth);
			foreach (var geometry in layer)
			{
				DrawPolygon(g, geometry, PolygonBrush, PolygonPen);
			}
			PolygonBrush.Dispose();
			PolygonPen.Dispose();

		}

	}
}

