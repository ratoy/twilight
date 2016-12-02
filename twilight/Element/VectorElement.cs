using System;
namespace twilight
{
	public class VectorElement : IElement
	{
		public VectorElement()
		{
		}
		public IStyle Style { get; set; }
		public IGeometry Geometry { get; set; }
	}
}
