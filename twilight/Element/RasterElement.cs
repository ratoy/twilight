using System;
namespace twilight
{
	public class RasterElement : IElement
	{
		public RasterElement()
		{
		}

		public IStyle Style { get; set; }
		public string ImageName { get; set; }
	}
}
