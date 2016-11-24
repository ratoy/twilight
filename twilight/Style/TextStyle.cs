using System;
namespace twilight
{
	public class TextStyle: IStyle
	{
		public TextStyle()
		{
			this.FontName = "Arial";
			this.FontSize = 10;
			this.FontColor = new RgbColor(0, 0, 0);
		}
		public TextStyle(string FontName, double FontSize, RgbColor FontColor)
		{
			this.FontName = FontName;
			this.FontSize = FontSize ;
			this.FontColor = FontColor;
		}

		public TextStyle(string FontName, double FontSize, byte Red,byte Green,byte Blue)
		{
			this.FontName = FontName;
			this.FontSize = FontSize;
			this.FontColor = new RgbColor(Red, Green, Blue);
		}
		public string FontName
		{ get; set; }
		public double FontSize
		{ get; set; }
		public RgbColor FontColor
		{ get; set; }
	}
}
