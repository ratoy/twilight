using System;
using System.Collections.Generic;

namespace twilight
{
	public class Polygon : IGeometry
	{
		public Polygon()
		{
			this.RingList = new List<Ring>();
		}

		public EnumGeoType GeoType
		{
			get
			{
				return EnumGeoType.Polygon;
			}
		}

		public List<Ring> RingList
		{ get; set; }

		public Envelope Extent
		{
			get
			{
				return GetEnv(this.RingList);
			}
		}

		Envelope GetEnv(List<Ring> SrcRings)
		{
			if (SrcRings == null || SrcRings.Count == 0)
			{
				return new Envelope(0, 0, 0, 0);
			}
			double xmin = double.MaxValue, xmax = double.MinValue,
			ymin = double.MaxValue, ymax = double.MinValue;

			foreach (var p in SrcRings)
			{
				xmin = Math.Min(xmin, p.Extent.XMin);
				xmax = Math.Max(xmax, p.Extent.XMax);
				ymin = Math.Min(ymin, p.Extent.YMin);
				ymax = Math.Max(ymax, p.Extent.YMax);
			}

			return new Envelope(xmin, xmax, ymin, ymax);
		}
	}
}
