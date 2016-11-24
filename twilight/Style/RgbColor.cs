using System;
namespace twilight
{
	public class RgbColor
	{

		public RgbColor()
		{
			Init(255, 0, 0, 0);
		}
		public RgbColor(byte r, byte g, byte b)
		{
			Init(255, r, g, b);
		}

		public RgbColor(byte a, byte r, byte g, byte b)
		{
			Init(a, r, g, b);
		}

		void Init(byte a, byte r, byte g, byte b)
		{
			this.Alpha = a;
			this.Red = r;
			this.Green = g;
			this.Blue = b;
		}
		public byte Alpha
		{ get; set; }
		public byte Red
		{ get; set; }
		public byte Green
		{ get; set; }
		public byte Blue
		{ get; set; }

		public int ToOleColor()
		{
			return (int)((Red) + (Green * 256) + (Blue * 256 * 256));
		}

		public void FromOLEColor(int OleColor)
		{
			this.Red = (byte)(OleColor & 0xff);
			this.Green = (byte)((OleColor >> 8) & 0xff);
			this.Blue = (byte)((OleColor >> 16) & 0xff);
		}

	}
}
