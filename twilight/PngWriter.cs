using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace twilight
{
	public class PngWriter
	{
		public PngWriter ()
		{

		}

		public bool SavePng()
		{
			using (Bitmap b = new Bitmap(50, 50)) {
				using (Graphics g = Graphics.FromImage(b)) {
					g.Clear(Color.Green);
					Pen pen = new Pen (new SolidBrush (Color.Red));
					g.DrawRectangle (pen, new Rectangle (20, 20, 10, 10));
					g.FillRectangle (new SolidBrush (Color.FromArgb (100, Color.Red)), new Rectangle (20, 20, 10, 10));
				}
				b.Save(@"green.png", ImageFormat.Png);
			}

			return true;
		}
	}
}

