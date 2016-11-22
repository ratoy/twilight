using System;
using System.Drawing;

namespace twilight
{
	public class LineStyle:IStyle
	{
		double m_LineWidth = 1;
		Color m_LineColor = Color.Black;

		public LineStyle()
		{ }

		public LineStyle (Color LineColor, double LineWidth)
		{
			m_LineColor = LineColor;
			m_LineWidth = LineWidth;
		}

		public Color LineColor
		{ get { return m_LineColor; } set { m_LineColor = value; } }

		public double LineWidth
		{ get { return m_LineWidth;} set { m_LineWidth = value; } }
	}
}

