using System;
using System.Drawing;
using System.Windows.Forms;

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

		protected override Point GetScreenRes()
		{
			try
			{
				Rectangle resolution = Screen.PrimaryScreen.Bounds;
				return new Point(resolution.Width, resolution.Height);
			}
			catch (Exception ex)
			{
				//OutputMsg("Getting resolution error: " + ex.Message);
				return new Point(this.m_Width, this.m_Height);
			}
		}
	}
}
