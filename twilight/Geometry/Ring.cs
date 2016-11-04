using System;

namespace twilight
{
	public class Ring:IGeometry
	{
		public Ring ()
		{
		}
		public EnumGeoType GeoType {
			get {
				return EnumGeoType.Ring;
			}
		}

		public Envelope Extent {
			get {
				throw new NotImplementedException ();
			}
	}
}
}
