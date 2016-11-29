using System;
using System.Collections.Generic;

namespace twilight
{
	public class Polyline : IGeometry
	{
		public Polyline()
		{
			this.SegmentList = new List<Segment>();
		}

		public EnumGeoType GeoType
		{
			get
			{
				return EnumGeoType.Polyline;
			}
		}

		public List<Segment> SegmentList
		{ get; set; }

		public Envelope Extent
		{
			get
			{
				return GetEnv(this.SegmentList);
			}
		}

		Envelope GetEnv(List<Segment> SrcSegments)
		{
			if (SrcSegments == null || SrcSegments.Count == 0)
			{
				return new Envelope(0, 0, 0, 0);
			}
			double xmin = double.MaxValue, xmax = double.MinValue,
			ymin = double.MaxValue, ymax = double.MinValue;

			foreach (var p in SrcSegments)
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
