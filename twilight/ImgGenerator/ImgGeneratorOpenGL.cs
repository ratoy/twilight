using System;
namespace twilight
{
	public class ImgGeneratorOpenGL:BaseImgGenerator
	{
		public ImgGeneratorOpenGL():base()
		{
		}
		public ImgGeneratorOpenGL(RgbColor c):base(c)
		{
			m_BackgroundColor = c;
		}

		public ImgGeneratorOpenGL(int width, int height, RgbColor c):base(width,height,c)
		{
			this.m_Width = width;
			this.m_Height = height;
			m_BackgroundColor = c;
		}
	}
}
