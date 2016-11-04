using System;

namespace twilight
{
	public class SunPos
	{
		public SunPos ()
		{
		}


		
		//由儒略日计算当前的太阳位置，输出其经纬度坐标 
		private void SunPosition(DateTime time,out double lon,out double lat,out double alt)
		{
			double twopi = 2.0 * Math.PI;
			double deg2rad = Math.PI / 180.0;
			DateTime universaltime = time.ToUniversalTime();//当前时间转换成格林威治时间
			Julian CurrentJul = new Julian(universaltime);//当前时间转换成儒略日

			Console.WriteLine ("juldate: "+CurrentJul.Date);

			double tut1, meanlong, ttdb, meananomaly, eclplong, obliquity, magr, dbi, dbj, dbk;
			tut1 = (CurrentJul.Date - 2451545.0) / 36525.0;
			meanlong = 280.460 + 36000.77 * tut1;
			meanlong %= 360.0;
			ttdb = tut1;
			meananomaly = 357.5277233 + 35999.05034 * ttdb;
			meananomaly = meananomaly * deg2rad % twopi;
			if (meananomaly < 0.0)
			{
				meananomaly += twopi;
			}
			eclplong = meanlong + 1.914666471 * Math.Sin(meananomaly) + 0.019994643 * Math.Sin(2.0 * meananomaly);
			obliquity = 23.439291 - 0.0130042 * ttdb;
			meanlong = meanlong * deg2rad;
			if (meanlong < 0.0)
				meanlong = twopi + meanlong;
			eclplong = eclplong * deg2rad;
			obliquity = obliquity * deg2rad;
			magr = 1.000140612 - 0.016708617 * Math.Cos(meananomaly) - 0.000139589 * Math.Cos(2.0 * meananomaly);
			//计算得出太阳的地心坐标系坐标
			dbi = magr * Math.Cos(eclplong);
			dbj = magr * Math.Cos(obliquity) * Math.Sin(eclplong);
			dbk = magr * Math.Sin(obliquity) * Math.Sin(eclplong);

			Console.WriteLine (string.Format("eci: {0},{1},{2}",dbi,dbj,dbk));
			//利用orbittools.dll中的相关函数,把地心坐标系转换成经纬度
			Vector pos ,vel;
			pos = new Vector();
			vel = new Vector();
			pos.X =dbi;
			pos.Y =dbj;
			pos.Z =dbk;
			vel.X =1;//仅用于初始化eci对象
			vel.Y =2;
			vel.Z =3;
			Eci eci = new Eci(pos, vel, CurrentJul, true);
			//转换,并把经度范围设在-180~180之内
			lon=eci.toGeo().Longitude*180/Math.PI;
			lon = lon > 180 ? lon - 360 : lon;
			lon = lon <-180 ? lon + 360 : lon;
			lat = eci.toGeo().Latitude*180/Math.PI;
			alt = eci.toGeo().Altitude;

		}
	}
}

