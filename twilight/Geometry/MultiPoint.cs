using System;

namespace twilight
{
	public class MultiPoint:IGeometry
	{
		public MultiPoint ()
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
