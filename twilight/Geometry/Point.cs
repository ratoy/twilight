using System;

namespace twilight
{
	public class Point:IGeometry
	{
		public Point ()
		{
		}

		public Point (double x, double y)
		{
			Init (x, y, 0);
		}

		public Point (double x, double y, double z)
		{
			Init (x, y, z);
		}

		void Init (double x, double y, double z)
		{			
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public double X
		{ get; set; }

		public double Y
		{ get; set; }

		public double Z
		{ get; set; }

		public EnumGeoType GeoType {
			get {
				return EnumGeoType.Point;
			}
		}

		public Envelope Extent {
			get {
				return new Envelope (this.X, this.X, this.Y, this.Y);
			}
		}

		public void PutCoords (double x, double y, double z)
		{
			Init (x, y, z);
		}

		public void PutCoords (double x, double y)
		{
			Init (x, y, 0);
		}
	}
}

