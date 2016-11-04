using System;
using System.Collections.Generic;

namespace twilight
{
	public class Ring:IGeometry
	{
		bool m_Outter = true;

		public Ring ()
		{
		}

		public Ring (List<Point> Points)
		{
			this.PointList = Points;
		}

		public bool OutterRing {
			get{ return m_Outter;}
			set{ m_Outter = value;}
		}

		public EnumGeoType GeoType {
			get {
				return EnumGeoType.Ring;
			}
		}

		public List<Point> PointList
		{ get; set; }

		public Envelope Extent {
			get {
				return GetEnv (this.PointList);
			}
		}

		Envelope GetEnv (List<Point> SrcPoints)
		{
			if (SrcPoints == null || SrcPoints.Count == 0) {
				return new Envelope (0, 0, 0, 0);
			}
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
	}
}
