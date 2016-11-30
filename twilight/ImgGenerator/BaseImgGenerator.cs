using System;
using System.Collections.Generic;

namespace twilight
{
	public abstract class BaseImgGenerator
	{
		protected RgbColor m_BackgroundColor = new RgbColor(255, 255, 255);
		protected int m_Width = 1280, m_Height = 800;
		protected double m_XMin = -180, m_XMax = 180, m_YMin = -90, m_YMax = 90;
		protected double m_XScale = 1, m_YScale = 1;
		protected IStyle m_DefaultFillStyle, m_DefaultLineStyle, m_DefaultPointStyle, m_DefaultTextStyle;

		public BaseImgGenerator()
		{
			GetScreenRes();
			Init(m_Width, m_Height, m_BackgroundColor);
		}

		public BaseImgGenerator(RgbColor c)
		{
			GetScreenRes();
			Init(m_Width, m_Height, c);
		}

		public BaseImgGenerator(int width, int height, RgbColor c)
		{
			Init(width, height, c);
		}

		void Init(int width, int height, RgbColor c)
		{
			if (width > 0 && height > 0)
			{
				this.m_Width = width;
				this.m_Height = height;
			}
			m_BackgroundColor = c;
			CalScale();
			InitStyles();
		}

		void InitStyles()
		{
			m_DefaultPointStyle = new PointStyle(230, 0, 0, 5, EnumPointType.Circle);
			m_DefaultLineStyle = new LineStyle(10, 10, 10, 1);
			m_DefaultFillStyle = new FillStyle(230, 0, 0, 0, 0, 230, 1);
			m_DefaultTextStyle = new TextStyle("Arial", 10, new RgbColor(0, 230, 0));
		}

		void CalScale()
		{
			m_XScale = m_Width / (m_XMax - m_XMin);
			m_YScale = m_Height / (m_YMax - m_YMin);
		}


		protected List<IGeometry> ReadShpFile(string ShpFile)
		{
			List<IGeometry> GeoList = new List<IGeometry>();
			ShpReader ShpRd = new ShpReader(ShpFile);

			int count = ShpRd.FeatureCount;
			for (uint i = 0; i < count; i++)
			{
				//geometry
				IGeometry g = ShpRd.ReadGeometry(i);
				GeoList.Add(g);
			}
			return GeoList;
		}

		public void AddShapeFile(string ShapeFile, IStyle pStyle = null)
		{
			//read
			List<IGeometry> GeoList = ReadShpFile(ShapeFile);
			if (GeoList.Count == 0)
			{
				return;
			}

			foreach (var item in GeoList)
			{
				AddGeometry(item, pStyle);
			}
		}

		public void AddGeometry(IGeometry pGeometry, IStyle pStyle = null)
		{
			if (pGeometry == null)
			{
				return;
			}
			switch (pGeometry.GeoType)
			{
				case EnumGeoType.Point:
				case EnumGeoType.MultiPoint:
					AddPoint(pGeometry as Point, pStyle as PointStyle);
					break;
				case EnumGeoType.Polyline:
					AddPolyline(pGeometry as Polyline, pStyle as LineStyle);
					break;
				case EnumGeoType.Polygon:
					AddPolygon(pGeometry as Polygon, pStyle as FillStyle);
					break;
				default:
					break;
			}
		}

		public abstract void AddPoint(Point pPoint, PointStyle pStyle = null);
		public abstract void AddPolyline(Polyline pPolyline, LineStyle pStyle = null);
		public abstract void AddPolygon(Polygon pGeometry, FillStyle pStyle = null);
		public abstract void AddImage(string ImgFileName, Point pPoint);
		public abstract void AddImage(string ImgFileName, Envelope pEnv);
		public abstract void AddText(string pText, Point pPoint, TextStyle pStyle = null);
		public abstract bool Save(string FileName);
		protected abstract Point GetScreenRes();
	}
}
