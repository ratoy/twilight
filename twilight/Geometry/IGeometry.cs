using System;

namespace twilight
{
	public interface IGeometry
	{
		EnumGeoType GeoType{ get;}
		Envelope Extent{ get;}
	}
}

