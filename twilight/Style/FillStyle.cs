using System;

namespace twilight
{
	public class FillStyle : IStyle
	{
		LineStyle m_OutLinestyle;
		RgbColor m_FillColor = new RgbColor(0, 0, 0);

		public FillStyle()
		{ }
		public FillStyle(RgbColor FillColor, LineStyle OutLinestyle)
		{
			m_FillColor = FillColor;
			m_OutLinestyle = OutLinestyle;
		}
		public FillStyle(byte Red, byte Green, byte Blue, LineStyle OutLineStyle)
		{
			m_FillColor = new RgbColor(Red, Green, Blue);
			m_OutLinestyle = OutLinestyle;
		}
		public FillStyle(byte Red, byte Green, byte Blue,
						 byte LineRed, byte LineGreen, byte LineBlue,
						 double LineWidth)
		{
			m_FillColor = new RgbColor(Red, Green, Blue);
			m_OutLinestyle = new LineStyle(LineRed, LineGreen, LineBlue, LineWidth);
		}
		public FillStyle(byte Alpha, byte Red, byte Green, byte Blue,
					 byte LineAlpha, byte LineRed, byte LineGreen, byte LineBlue,
					 double LineWidth)
		{
			m_FillColor = new RgbColor(Alpha, Red, Green, Blue);
			m_OutLinestyle = new LineStyle(LineAlpha, LineRed, LineGreen, LineBlue, LineWidth);
		}

		public LineStyle OutLinestyle
		{ get { return m_OutLinestyle; } set { m_OutLinestyle = value; } }

		public RgbColor FillColor
		{ get { return m_FillColor; } set { m_FillColor = value; } }
	}
}

