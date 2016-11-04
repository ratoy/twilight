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
				throw new NotImplementedException ();
			}
		}

		public Envelope Extent {
			get {
				throw new NotImplementedException ();
			}
	}
}
}
