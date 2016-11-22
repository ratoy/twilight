using System;
using System.Collections.Generic;
using System.Drawing;

namespace twilight
{
	public class DrawGeometry:BaseCustomDraw
	{
		protected PointStyle m_pointstyle;
		protected LineStyle m_linestyle;
		protected FillStyle m_fillstyle;

		public DrawGeometry ()
		{
			m_pointstyle = new PointStyle(Color.FromArgb(210,Color.Green),2,EnumPointType.Circle);
			m_linestyle = new LineStyle(Color.Blue,1);

			LineStyle outline = new LineStyle(Color.FromArgb(201, 140, 198), 1);
			m_fillstyle = new FillStyle(Color.FromArgb(242, 239, 233),outline);
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

		protected object TransGeometry (List<IGeometry> SrcGeoList)
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
	}
}

