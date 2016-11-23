using System;
using System.Drawing;
using System.Collections.Generic;

namespace twilight
{
	public abstract class BaseCustomDraw
	{
		protected int m_Width = 1280, m_Height = 800;

		public BaseCustomDraw(int Width, int Height)
		{
			this.m_Width = Width;
			this.m_Height = Height;
		}
		public int ImgWidth
		{ get { return m_Width; } }

		public int ImgHeight
		{
			get
			{
				return m_Height;
			}
		}

		public abstract void Draw(Graphics g, object obj, IStyle style);

		protected Envelope GetMapEnv()
		{
			return new Envelope(-180, 180, -90, 90);
		}

		protected List<Point> TransPoints(List<Point> SrcPoints)
		{
			Envelope Env = GetMapEnv();

			double xscale = m_Width / Env.Width, yscale = m_Height / Env.Height;
			List<Point> DestPoints = new List<Point>();

			foreach (var p in SrcPoints)
			{
				Point pf = new Point();
				pf.X = ((p.X - Env.XMin) * xscale);
				pf.Y = (m_Height - (p.Y - Env.YMin) * yscale);
				DestPoints.Add(pf);
			}
			return DestPoints;
		}

		protected List<PointF> TransPointsAsFloat(List<Point> SrcPoints)
		{
			List<Point> DestPoints = TransPoints(SrcPoints);

			return ConvertToPointF(DestPoints);
		}

		protected List<PointF> ConvertToPointF(List<Point> SrcPoints)
		{
			/*
			List<PointF> DestPoints = new List<PointF> ();

			foreach (var item in SrcPoints) {
				PointF pf = new PointF ();
				pf.X = (float)item.X;
				pf.Y = (float)item.Y;
				DestPoints.Add (pf);
			}
			return DestPoints;
			*/
			List<PointF> DestPoints = SrcPoints.ConvertAll(s =>
			{
				return new PointF((float)s.X, (float)s.Y);
			});

			return DestPoints;
		}
	}
}

