using System;

namespace twilight
{
	public class Segment:IGeometry
	{
		public Segment ()
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
