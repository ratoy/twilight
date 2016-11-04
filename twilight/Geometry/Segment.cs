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
				return EnumGeoType.Segment;
			}
		}

		public Envelope Extent {
			get {
				throw new NotImplementedException ();
			}
	}
}
}
