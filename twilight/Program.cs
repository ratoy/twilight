using System;

namespace twilight
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			PngWriter pw = new PngWriter ();
			pw.SavePng ();
		}
	}
}
