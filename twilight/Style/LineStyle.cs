using System;

namespace twilight
{
	public class LineStyle:IStyle
	{
		double m_LineWidth = 1;
		RgbColor m_LineColor = new RgbColor();

		public LineStyle()
		{ }

		public LineStyle (RgbColor LineColor, double LineWidth)
		{
			m_LineColor = LineColor;
			m_LineWidth = LineWidth;
		}

		public LineStyle(byte Red,byte Green,byte Blue, double LineWidth)
		{
			m_LineColor = new RgbColor(Red,Green,Blue);
			m_LineWidth = LineWidth;
		}

		public LineStyle(byte Alpha,byte Red, byte Green, byte Blue, double LineWidth)
		{
			m_LineColor = new RgbColor(Alpha,Red, Green, Blue);
			m_LineWidth = LineWidth;
		}

		public RgbColor LineColor
		{ get { return m_LineColor; } set { m_LineColor = value; } }

		public double LineWidth
		{ get { return m_LineWidth;} set { m_LineWidth = value; } }
	}
}

