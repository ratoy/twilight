using System;
namespace twilight
{
	public class TextStyle : IStyle
	{
		public TextStyle()
		{
			Init("Arial", 10, false, false, 0, 0, 0);
		}
		public TextStyle(string FontName, double FontSize, RgbColor FontColor)
		{
			Init(FontName, FontSize, false, false, FontColor.Red, FontColor.Green, FontColor.Blue);
		}

		public TextStyle(string FontName, double FontSize, byte Red, byte Green, byte Blue)
		{
			Init(FontName, FontSize, false, false, Red, Green, Blue);
		}

		public TextStyle(string FontName, double FontSize, bool Bold, bool Italic, byte Red, byte Green, byte Blue)
		{
			Init(FontName, FontSize, Bold, Italic, Red, Green, Blue);
		}

		void Init(string FontName, double FontSize, bool Bold, bool Italic, byte Red, byte Green, byte Blue)
		{
			this.FontName = FontName;
			this.FontSize = FontSize;
			this.FontColor = new RgbColor(Red, Green, Blue);
			this.Bold = Bold;
			this.Italic = Italic;
		}

		public string FontName
		{ get; set; }
		public double FontSize
		{ get; set; }
		public RgbColor FontColor
		{ get; set; }
		public bool Bold
		{ get; set; }
		public bool Italic
		{ get; set; }
	}
}
