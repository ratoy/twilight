using System;
namespace twilight
{
	public class ImgGeneratorGDI:BaseImgGenerator
	{
		public ImgGeneratorGDI()
		{
		}
		public ImgGeneratorGDI(RgbColor c):base(c)
		{
			m_BackgroundColor = c;
		}

		public ImgGeneratorGDI(int width, int height, RgbColor c):base(width,height,c)
		{
			this.m_Width = width;
			this.m_Height = height;
			m_BackgroundColor = c;
		}
	}
}
