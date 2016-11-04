using System;

namespace twilight
{
	public class Polygon:IGeometry
	{
		public Polygon ()
		{
		}
		public EnumGeoType GeoType {
			get {
				return EnumGeoType.Polygon;
			}
		}

		public Envelope Extent {
			get {
				throw new NotImplementedException ();
			}
	}
}
}
