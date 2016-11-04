using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace twilight
{
	public class PngWriter
	{
		int m_width=1280,m_height=720;
		public PngWriter (int width,int height)
		{
			this.m_width = width;
			this.m_height = height;
		}

		List<PointF> TransPoints(List<Point> SrcPoints)
		{
			Envelope Env = GetMapEnv ();
		
			double xscale = m_width / Env.Width,yscale=m_height/Env.Height;
			List<PointF> DestPoints = new List<PointF> ();

			foreach (var p in SrcPoints) {
				PointF pf=new PointF();
				pf.X =(float)( (p.X-Env.XMin) * xscale);
				pf.Y =(float)(m_height- (p.Y - Env.YMin) * yscale);
				DestPoints.Add (pf);
			}

			return DestPoints;
		}

		Envelope GetMapEnv()
		{
			return new Envelope (-180,180,-90,90);
		}

		Envelope GetEnv(List<Point> SrcPoints)
		{
			double xmin=double.MaxValue , xmax=double.MinValue ,
			ymin=double.MaxValue , ymax=double.MinValue;

			foreach (var p in SrcPoints) {
				xmin = Math.Min (xmin, p.X);
				xmax = Math.Max (xmax, p.X);
				ymin = Math.Min (ymin, p.Y);
				ymax = Math.Max (ymax, p.Y);
			}

			return new Envelope (xmin,xmax,ymin,ymax);
		}

		public bool SavePng()
		{
			using (Bitmap b = new Bitmap(m_width, m_height)) {
				using (Graphics g = Graphics.FromImage(b)) {
					g.Clear(Color.White);
					DrawSun (g);
					/*
					Pen pen = new Pen (new SolidBrush (Color.Red));
					g.DrawRectangle (pen, new Rectangle (20, 20, 10, 10));
					g.FillRectangle (new SolidBrush (Color.FromArgb (100, Color.Red)), new Rectangle (20, 20, 10, 10));
*/
}
				b.Save(@"green.png", ImageFormat.Png);
			}

			return true;
		}

		void DrawSun(Graphics g)
		{
			SunPos sp = new SunPos ();
			Point p = sp.GetSunPos ();
			List<Point> Twlightline = sp.GetTwilightLine();

			List<PointF> pfList = TransPoints (new List<Point> () { p });
			PointF pf = pfList [0];
			RectangleF rf = new RectangleF (pf.X - 20, pf.Y - 20, 40, 40);
			Brush FillBrush = new SolidBrush (Color.Yellow);
				Pen pen = new Pen (new SolidBrush (Color.Red));
			g.FillEllipse(FillBrush,rf);
			g.DrawEllipse (pen, rf);

			//draw twilightline
			DrawTwilightline (g, Twlightline);
		}

		void DrawTwilightline(Graphics g,List<Point> Twilightline)
		{
			List<PointF> pfList = TransPoints (new List<Point> ( Twilightline  ));

			for(int i=0;i<360;i++)
			{
				//Console.WriteLine(pfList[i].X);
				//Console.WriteLine(line[i].Y);
				//Console.WriteLine(String.Format("x: {0}, y:{1}",line[i].X,line[i].Y));
			}
			PointF pf0 = new PointF ();
			PointF pf1 = new PointF ();

			Brush FillBrush = new SolidBrush (Color.FromArgb(100,Color.Black));
			Pen pen = new Pen (new SolidBrush (Color.Black));

			for (int i = 0; i < pfList.Count-1; i++) {
				PointF pf2 = pfList [i];
				PointF pf3 = pfList [i + 1];
				pf0.X = pf2.X;
				pf0.Y = 0;
				pf1.X = pf3.X;
				pf1.Y = 0;

				g.FillPolygon(FillBrush,new PointF[4]{pf0,pf2,pf3,pf1});
			}
		}
	}
}

