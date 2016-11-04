using System;

namespace twilight
{
	public class Polyline:IGeometry
	{
		public Polyline ()
		{
		}
		public EnumGeoType GeoType {
			get {
				return EnumGeoType.Polyline;
			}
		}

		public Envelope Extent {
			get {
				throw new NotImplementedException ();
			}
	}
}
}
