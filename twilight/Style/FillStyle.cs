using System;
using System.Drawing;

namespace twilight
{
	public class FillStyle:IStyle
	{
		LineStyle m_OutLinestyle;
		Color m_FillColor = Color.Black;

		public FillStyle()
		{ }
		public FillStyle (Color FillColor ,LineStyle OutLinestyle)
		{
			m_FillColor = FillColor;
			m_OutLinestyle = OutLinestyle;
		}

		public LineStyle OutLinestyle
		{ get { return m_OutLinestyle; } set { m_OutLinestyle = value; }}

		public Color FillColor
		{ get { return m_FillColor; } set { m_FillColor = value;}}
	}
}

