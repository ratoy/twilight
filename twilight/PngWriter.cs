using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace twilight
{
	public class PngWriter
	{
		RgbColor m_BackgroundColor = new RgbColor(181, 208, 208);
		String m_DefaultFileName = "screen.png", m_ExeFolder = "";
		DateTime m_PngTime = DateTime.Now;
		bool m_DebugMode = false;

		public PngWriter()
		{
			m_ExeFolder = Application.StartupPath;
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

		BaseImgGenerator GetGenerator(int width, int height)
		{
			return new ImgGeneratorGDI(width, height, m_BackgroundColor);
		}

		List<string> GetShpFileNames()
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
			return ShpFileList;
		}

		public bool SavePng2(DateTime dt, string pngfile, int width = -1, int height = -1)
		{
			List<string> ShpFileList = GetShpFileNames();

			OutputMsg("begin...");

			BaseImgGenerator big = GetGenerator(width, height);
			OutputMsg("generator inited ...");
			//add shapefile
			foreach (var item in ShpFileList)
			{
				big.AddShapeFile(item, new FillStyle(242, 239, 233, 201, 140, 198, 1));
			}
			OutputMsg("shapfile added ...");
			//add twilightline
			SunPos sp = new SunPos();
			Point sunpos = sp.GetSunPos(dt.ToUniversalTime());
			OutputMsg("got sun position ...");
			List<Point> Twlightline = sp.GetTwilightLine();
			OutputMsg("got twilightline position ...");
			if (sunpos.Y > 0)
			{
				//sun in north
				Twlightline.Insert(0, new Point(-180, -90));
				Twlightline.Add(new Point(180, -90));
			}
			else
			{
				//sun in south
				Twlightline.Insert(0, new Point(-180, 90));
				Twlightline.Add(new Point(180, 90));
			}
			Polygon Polygontw = new Polygon();
			Ring ringtw = new Ring(Twlightline);
			Polygontw.RingList.Add(ringtw);

			//style
			FillStyle Styletw = new FillStyle(100, 0, 0, 0, 255, 0, 0, 0, 1);
			big.AddPolygon(Polygontw, Styletw);
			OutputMsg("twilightline added ...");
			//add sun
			PointStyle sunstyle = new PointStyle(255, 255, 0, 5, EnumPointType.Circle);
			big.AddPoint(sunpos, sunstyle);
			OutputMsg("sun added ...");
			//additional lines
			LineStyle addlinestyle = new LineStyle(80, 80, 80, 1);
			double[] lon = { -180, 180, -180, 180, -180, 180 };
			double[] lat = { 0, 0, 23.44, 23.44, -23.44, -23.44 };
			for (int i = 0; i < lon.Length - 1; i += 2)
			{
				Point p1 = new Point(lon[i], lat[i]);
				Point p2 = new Point(lon[i + 1], lat[i + 1]);

				Polyline polyline = new Polyline();
				Segment seg = new Segment(new List<Point>() { p1, p2 });
				polyline.SegmentList.Add(seg);
				big.AddPolyline(polyline, addlinestyle);
			}
			OutputMsg("additional lines added ...");
			//add title
			TextStyle titlestyle = new TextStyle("Arial", 10, 50, 205, 50);
			string text = m_PngTime.ToString("yyyy-MM-dd HH:mm:ss");
			big.AddText(text, new Point(-180, 90));
			OutputMsg("title added ...");

			OutputMsg("saving...");
			string filename = pngfile.Trim().Length == 0 ? m_DefaultFileName : pngfile;
			big.Save(filename);
			OutputMsg("finished!");

			return true;
		}

		public bool SavePng()
		{
			return SavePng2(DateTime.Now, m_DefaultFileName);
		}

	}
}

