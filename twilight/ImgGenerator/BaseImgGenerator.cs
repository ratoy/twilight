using System;

namespace twilight
{
	public abstract class BaseImgGenerator
	{
		protected RgbColor m_BackgroundColor = new RgbColor(255, 255, 255);
		protected int m_Width=1280, m_Height=800;
		public BaseImgGenerator()
		{ }

		public BaseImgGenerator(RgbColor c)
		{
			m_BackgroundColor = c;
		}

		public BaseImgGenerator(int width, int height, RgbColor c)
		{
			this.m_Width = width;
			this.m_Height = height;
			m_BackgroundColor = c;
		}

		public abstract void AddGeometry(IGeometry pGeometry, IStyle pStyle=null);
		public abstract void AddPoint(Point pPoint, PointStyle pStyle= null);
		public abstract void AddPolyline(Polyline pPolyline, LineStyle pStyle= null);
		public abstract void AddPolygon(Polygon pGeometry, FillStyle pStyle= null);

		public abstract void AddText(string pText,Point pPoint, TextStyle pStyle = null);
		public abstract void AddShapeFile(string ShapeFile,IStyle pStyle= null);
		public abstract bool Save(string FileName);
		protected abstract Point GetScreenRes();
	}
}
