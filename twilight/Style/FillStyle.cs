using System;

namespace twilight
{
	public class FillStyle:IStyle
	{
		LineStyle m_OutLinestyle;
		RgbColor m_FillColor = new RgbColor(0, 0, 0);

		public FillStyle()
		{ }
		public FillStyle (RgbColor FillColor ,LineStyle OutLinestyle)
		{
			m_FillColor = FillColor;
			m_OutLinestyle = OutLinestyle;
		}

		public LineStyle OutLinestyle
		{ get { return m_OutLinestyle; } set { m_OutLinestyle = value; }}

		public RgbColor FillColor
		{ get { return m_FillColor; } set { m_FillColor = value;}}
	}
}

