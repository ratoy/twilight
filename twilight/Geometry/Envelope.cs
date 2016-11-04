using System;

namespace twilight
{
	public class Envelope:IGeometry
	{
		public Envelope (double xmin, double xmax, double ymin, double ymax)
		{
			this.XMin = xmin;
			this.XMax = xmax;
			this.YMin = ymin;
			this.YMax = ymax;
		}

		public double XMin
		{ get; set; }

		public double XMax
		{ get; set; }

		public double YMin
		{ get; set; }

		public double YMax
		{ get; set; }

		public double Width
		{ get { return this.XMax - this.XMin; } }

		public double Height
		{ get { return this.YMax - this.YMin; } }

		public EnumGeoType GeoType {
			get {
				return EnumGeoType.Envelope;
			}
		}

		public Envelope Extent {
			get {
				return new Envelope (XMin, XMax, YMin, YMax);
			}
		}
	}
}
