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
				return EnumGeoType.MultiPoint;
			}
		}

		public Envelope Extent {
			get {
				throw new NotImplementedException ();
			}
	}
}
}
