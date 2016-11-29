using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace twilight
{
	public class ImgGeneratorGDI : BaseImgGenerator
	{
		Graphics g;
		Bitmap m_bitmap;
		public ImgGeneratorGDI()
		{
			Init();
		}
		public ImgGeneratorGDI(RgbColor c) : base(c)
		{
			Init();
		}

		public ImgGeneratorGDI(int width, int height, RgbColor c) : base(width, height, c)
		{
			Init();
		}

		~ImgGeneratorGDI()
		{
			g.Dispose();
			m_bitmap.Dispose();
		}

		void Init()
		{
			m_bitmap = new Bitmap(m_Width, m_Height);
			g = Graphics.FromImage(m_bitmap);
			g.Clear(StyleToColor(m_BackgroundColor));
		}

		#region translate
		Color StyleToColor(RgbColor rgb)
		{
			return Color.FromArgb(rgb.Alpha, rgb.Red, rgb.Blue, rgb.Green);
		}

		PointF TransPoint(Point p)
		{
			Point DestPoints = new Point();

			PointF pf = new PointF();
			pf.X = (float)((p.X - m_XMin) * m_XScale);
			pf.Y = (float)(m_Height - (p.Y - m_YMin) * m_YScale);
			return pf;
		}

		List<PointF> TransPointList(List<Point> SrcPoints)
		{
			List<PointF> DestPoints = SrcPoints.ConvertAll(s =>
				{
					return TransPoint(s);
				});

			return DestPoints;
		}

		#endregion

		public override void AddGeometry(IGeometry pGeometry, IStyle pStyle = null)
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
		public override void AddPoint(Point pPoint, PointStyle pStyle = null)
		{
			if (pPoint == null)
			{
				return;
			}
			if (pStyle == null)
			{
				pStyle = m_DefaultPointStyle as PointStyle;
			}
			Brush PointBrush = new SolidBrush(StyleToColor(pStyle.PointColor));
			Pen PointPen = new Pen(new SolidBrush(StyleToColor(pStyle.PointColor)));

			PointF point = TransPoint(pPoint);
			float pointsize = (float)pStyle.PointSize;

			RectangleF rf = new RectangleF((float)point.X - pointsize, (float)point.Y - pointsize,
											(float)pointsize * 2, (float)pointsize * 2);
			g.FillEllipse(PointBrush, rf);
			g.DrawEllipse(PointPen, rf);
			PointBrush.Dispose();
			PointPen.Dispose();
		}

		public override void AddPolyline(Polyline pPolyline, LineStyle pStyle = null)
		{
			if (pPolyline == null)
			{
				return;
			}
			if (pStyle == null)
			{
				pStyle = m_DefaultLineStyle as LineStyle;
			}
			Pen PolylinePen = new Pen(new SolidBrush(StyleToColor(pStyle.LineColor)),
												  (float)pStyle.LineWidth);
			foreach (var segment in pPolyline.SegmentList)
			{
				g.DrawLines(PolylinePen, TransPointList(segment.PointList).ToArray());
			}
			PolylinePen.Dispose();
		}
		public override void AddPolygon(Polygon pGeometry, FillStyle pStyle = null)
		{
			if (pGeometry == null)
			{
				return;
			}
			if (pStyle == null)
			{
				pStyle = m_DefaultFillStyle as FillStyle;
			}
			Brush PolygonBrush = new SolidBrush(StyleToColor(pStyle.FillColor));
			Pen PolygonPen = new Pen(new SolidBrush(StyleToColor(pStyle.OutLinestyle.LineColor)),
									 (float)pStyle.OutLinestyle.LineWidth);

			foreach (var ring in (pGeometry as Polygon).RingList)
			{
				g.FillPolygon(PolygonBrush, TransPointList(ring.PointList).ToArray());
				g.DrawPolygon(PolygonPen, TransPointList(ring.PointList).ToArray());
			}
			PolygonBrush.Dispose();
			PolygonPen.Dispose();
		}

		public override void AddText(string pText, Point pPoint, TextStyle pStyle = null)
		{
			if (pText == "")
			{
				return;
			}
			if (pStyle == null)
			{
				pStyle = m_DefaultTextStyle as TextStyle;
			}
			Font f = new Font(pStyle.FontName, (float)pStyle.FontSize);
			SolidBrush brush = new SolidBrush(StyleToColor(pStyle.FontColor));
			g.DrawString(pText, f, brush, TransPoint(pPoint));
			brush.Dispose();
			f.Dispose();
		}

		List<IGeometry> ReadShpFile(string ShpFile)
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

		public override void AddShapeFile(string ShapeFile, IStyle pStyle = null)
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

		public override bool Save(string FileName)
		{
			m_bitmap.Save(FileName);
			return true;
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
