using System;

namespace twilight
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			bool SpecificTime = false, SpecificFile = false, SpecificSize = false;
			string PngFile = "";
			DateTime dtTime = DateTime.Now;
			int m_Width = -1, m_Height = -1;

			foreach (string str in args) {
				switch (str.Trim ()) { 
				case "-t":
					SpecificTime = true;
					break; 
				case "-o":
					SpecificFile = true;
					break;
				case "-s":
					SpecificSize = true;
					break;
				case "-h":
				case "-help":
					Console.WriteLine ();
					Console.WriteLine ("Generate a png file filled with worldmap and twilightline");
					Console.WriteLine ("twilight.exe -t [utc time] -o [output file name] -s [width,height]");
					Console.WriteLine ("-t : specific local time");
					Console.WriteLine ("-o : output file name");
					Console.WriteLine ("-s : specific png size, including width and height, split with ','");
					Console.WriteLine ("-h : help");
					return;
				default: 
					if (SpecificFile) {
						PngFile = str;
						SpecificFile = false;
					} else if (SpecificTime) {
						try {
							dtTime = DateTime.Parse (str);
						} catch (Exception ex) {
							dtTime = DateTime.Now;
						}
						SpecificTime = false;
					} else if (SpecificSize) {
						string[] strsize = str.Split (',');
						if (strsize.Length >= 2) {
							int.TryParse (strsize [0], out m_Width);
							int.TryParse (strsize [1], out m_Height);
						}
						SpecificSize = false;
					}
					break;
				}
			}
			//write to png
			PngWriter pw = new PngWriter ();
			pw.SavePng (dtTime, m_Width, m_Height, PngFile);
		}
	}
}
