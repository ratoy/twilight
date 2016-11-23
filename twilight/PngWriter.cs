using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace twilight
{
	public class PngWriter
	{
		int m_width = 1280, m_height = 720;
		Color m_BackgroundColor = Color.FromArgb(181, 208, 208);
		String m_DefaultFileName = "screen.png", m_ExeFolder = "";
		DateTime m_PngTime = DateTime.Now;
		bool m_DebugMode = false;

		public PngWriter()
		{
			m_ExeFolder = Application.StartupPath;
			//get screen resolution 
			Size Resolution = GetScreenRes();
			this.m_width = Resolution.Width;
			this.m_height = Resolution.Height;
		}

		public bool DebugMode
		{
			get { return m_DebugMode; }
			set { m_DebugMode = value; }
		}

		void OutputMsg(string Msg)
		{
			if (m_DebugMode)
			{
				Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + Msg);
			}
		}

		public bool SavePng2(DateTime dt, int width, int height, string pngfile)
		{
			string[] ShpFileNames = System.IO.Directory.GetFiles(m_ExeFolder, "*.shp");
			List<string> ShpFileList = new List<string>();
			ShpFileList.AddRange(ShpFileNames);

			string ShpFolder2 = System.IO.Path.Combine(m_ExeFolder, "shape");
			if (System.IO.Directory.Exists(ShpFolder2))
			{
				string[] ShpFileNames2 = System.IO.Directory.GetFiles(ShpFolder2, "*.shp");
				ShpFileList.AddRange(ShpFileNames2);
			}

			using (Bitmap b = new Bitmap(m_width, m_height))
			{
				using (Graphics g = Graphics.FromImage(b))
				{
					g.SmoothingMode = SmoothingMode.HighQuality;
					g.Clear(m_BackgroundColor);

					// drawimg di=new drawimg(m_width,m_height);
					//di.draw(g,imgfile,pos);
					//di.draw(g,imgfile,rectangle);

					foreach (var item in ShpFileList)
					{
						DrawShapeFile ds = new DrawShapeFile(m_width, m_height);
						ds.Draw(g, item);
					}
					//drawgeometry dg=new drawgeometry(m_width,m_height);
					//dg.draw(g,geo,style);
					//dg.draw(g,pointlist,geotype,style);

					//drawtext dt=new drawtext(m_width,m_height);
					//dt.draw(g,string,pos,style);

				}
			}
			return true;
		}

		public bool SavePng(DateTime dt, int width, int height, string pngfile)
		{
			OutputMsg("start");
			m_PngTime = dt;
			if (width <= 0 || height <= 0)
			{
				width = this.m_width;
				height = this.m_height;
			}

			if (System.IO.Path.GetExtension(pngfile) != ".png")
			{
				pngfile = m_DefaultFileName;
			}

			OutputMsg("width: " + width + " , " + height);
			OutputMsg("target file name: " + pngfile);
			using (Bitmap b = new Bitmap(m_width, m_height))
			{
				using (Graphics g = Graphics.FromImage(b))
				{
					g.SmoothingMode = SmoothingMode.HighQuality;
					g.Clear(m_BackgroundColor);
					//draw shapefile
					OutputMsg("draw shapefile...");
					//DrawShapefile (g);
					//draw sun and twilightline
					OutputMsg("draw twilightline...");
					DrawSun(g, dt);
					//additional lines
					OutputMsg("draw additional lines...");
					DrawAdditionalLines(g);
					//draw title
					OutputMsg("draw title...");
					DrawTitle(g);
				}
				OutputMsg("saving ...");
				b.Save(pngfile, ImageFormat.Png);
			}
			return true;
		}

		public bool SavePng()
		{
			return SavePng2(DateTime.Now, this.m_width, this.m_height, m_DefaultFileName);
		}

		Size GetScreenRes()
		{
			try
			{
				Rectangle resolution = Screen.PrimaryScreen.Bounds;
				return new Size(resolution.Width, resolution.Height);
			}
			catch (Exception ex)
			{
				OutputMsg("Getting resolution error: " + ex.Message);
				return new Size(this.m_width, this.m_height);
			}
		}

		List<PointF> TransPointsAsFloat(List<Point> pointlist)
		{
			return null;
		}
		void DrawSun(Graphics g, DateTime dt)
		{
			SunPos sp = new SunPos();
			Point p = sp.GetSunPos(dt.ToUniversalTime());

			List<Point> Twlightline = sp.GetTwilightLine();

			List<PointF> pfList = TransPointsAsFloat(new List<Point>() { p });
			PointF pf = pfList[0];
			float sunsize = 20;
			RectangleF rf = new RectangleF(pf.X - sunsize, pf.Y - sunsize, sunsize * 2, sunsize * 2);
			Brush FillBrush = new SolidBrush(Color.Yellow);
			Pen pen = new Pen(new SolidBrush(Color.Red), 2f);
			g.FillEllipse(FillBrush, rf);
			g.DrawEllipse(pen, rf);

			pen.Dispose();
			FillBrush.Dispose();
			//draw twilightline
			DrawTwilightline(g, Twlightline, p.Y > 0);
		}

		void DrawTwilightline(Graphics g, List<Point> Twilightline, bool SunInNorth)
		{
			List<PointF> pfList = TransPointsAsFloat(new List<Point>(Twilightline));

			PointF pf0 = new PointF();
			PointF pf1 = new PointF();

			if (SunInNorth)
			{
				pf0.X = 0;
				pf0.Y = m_height;
				pf1.X = m_width;
				pf1.Y = m_height;
			}
			else {
				pf0.X = 0;
				pf0.Y = 0;
				pf1.X = m_width;
				pf1.Y = 0;
			}

			pfList.Insert(0, pf0);
			pfList.Add(pf1);
			foreach (var point in pfList)
			{
				//Console.WriteLine (point.X);
			}

			Brush FillBrush = new SolidBrush(Color.FromArgb(100, Color.Black));
			Pen pen = new Pen(new SolidBrush(Color.Black));

			g.FillPolygon(FillBrush, pfList.ToArray());

			FillBrush.Dispose();
			pen.Dispose();
		}

		void DrawAdditionalLines(Graphics g)
		{
			Pen pen = new Pen(new SolidBrush(Color.FromArgb(80, 80, 80)), 1);
			pen.DashStyle = DashStyle.Dash;
			double[] lon = { -180, 180, -180, 180, -180, 180 };
			double[] lat = { 0, 0, 23.44, 23.44, -23.44, -23.44 };
			for (int i = 0; i < lon.Length - 1; i += 2)
			{
				Point p1 = new Point(lon[i], lat[i]);
				Point p2 = new Point(lon[i + 1], lat[i + 1]);

				List<PointF> pflist = TransPointsAsFloat(new List<Point>() { p1, p2 });
				g.DrawLine(pen, pflist[0], pflist[1]);
			}
			pen.Dispose();
		}

		void DrawTitle(Graphics g)
		{
			string text = m_PngTime.ToString("yyyy-MM-dd HH:mm:ss");
			g.DrawString(text, new Font("Arial", 10), new SolidBrush(Color.Lime), new PointF(0, 0));
		}
	}
}

