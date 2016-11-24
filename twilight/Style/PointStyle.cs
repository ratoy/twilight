using System;

namespace twilight
{
	public class PointStyle:IStyle
	{
		double m_Size = 1;
		RgbColor m_Color = new RgbColor (255,0,0); 
		EnumPointType m_PointType = EnumPointType.Circle;

		public PointStyle()
		{ }
		public PointStyle (RgbColor PointColor,double PointSize, EnumPointType PointType)
		{
			m_Color = PointColor;
			m_Size = PointSize;
			m_PointType = PointType;
		}

		public EnumPointType PointType
		{ get { return m_PointType;}set { m_PointType = value;} }

		public RgbColor PointColor
		{ get { return m_Color; } set { m_Color = value; } }

		public double PointSize
		{ get { return m_Size; } set { m_Size = value; } }
	}
}

