using System;
using System.Collections.Generic;

namespace twilight
{
	public class SunPos
	{ 

		const double EPOCH_JAN1_12H_2000 = 2451545.0;
		const double SEC_PER_DAY = 86400.0;       // Seconds per day (solar)
		const double OMEGA_E     = 1.00273790934; // Earth rotation per sidereal day
		const double PI           = 3.141592653589793;
		const double F           = 1.0 / 298.26;
		const double XKMPER      = 6378.135;  

		double lon1,lat1,alt1;

		bool IsLeapYear(int y)
		{ return (y % 4 == 0 && y % 100 != 0) || (y % 400 == 0); }

		double sqr(double x) 
		{
			return (x * x);
		}

		double JulianDate(int year,               // i.e., 2004
		                  int mon,                // 1..12
		                  int day,                // 1..31
		                  int hour,               // 0..23
		                  int min,                // 0..59
		                  double sec /* = 0.0 */) // 0..(59.999999...)
		{
			// Calculate N, the day of the year (1..366)
			int N;
			int F1 = (int)((275.0 * mon) / 9.0);
			int F2 = (int)((mon + 9.0) / 12.0);

			if (IsLeapYear(year))
			{
				// Leap year
				N = F1 - F2 + day - 30;
			}
			else
			{
				// Common year
				N = F1 - (2 * F2) + day - 30;
			}

			double dblDay = N + (hour + (min + (sec / 60.0)) / 60.0) / 24.0;

			// Now calculate Julian date
			year--;
			// Centuries are not leap years unless they divide by 400
			int A = (year / 100);
			int B = 2 - A + (A / 4);

			double NewYears = (int)(365.25 * year) +
				(int)(30.6001 * 14)  + 
					1720994.5 + B;  // 1720994.5 = Oct 30, year -1

			double m_Date = NewYears + dblDay;
			return m_Date;
		}

		double Mod(double a,double b)
		{
			return a % b;
		}

		double toGMST(double m_Date) 
		{
			  double UT = Mod(m_Date + 0.5, 1.0);
			  double TU = ( m_Date - EPOCH_JAN1_12H_2000 - UT) / 36525.0;

			double GMST = 24110.54841 + TU * 
				(8640184.812866 + TU * (0.093104 - TU * 6.2e-06));

			GMST = Mod(GMST + SEC_PER_DAY * OMEGA_E * UT, SEC_PER_DAY);

			if (GMST < 0.0)
				GMST += SEC_PER_DAY;  // "wrap" negative modulo value

			GMST=(2* PI * (GMST / SEC_PER_DAY));
			return GMST;
		}

		Point toGeo(Point eci,double gmst)
		{
			double theta =Math.Atan2(eci.Y,eci.X);
			double lon   = Mod(theta -gmst,2*PI);

			if (lon < 0.0) 
				lon += (2*PI);  // "wrap" negative modulo

			double r   =Math.Sqrt(sqr(eci.X) + sqr(eci.Y));

			double e2  = F * (2.0 - F);
			double lat =Math.Atan2(eci.Z, r);

			const double delta = 1.0e-07;
			double phi;
			double c;

			do   
			{
				phi = lat;
				c   = 1.0 / Math.Sqrt(1.0 - e2 * sqr(Math.Sin(phi)));
				lat = Math.Atan2(eci.Z + XKMPER * c * e2 * Math.Sin(phi), r);
			}
			while (Math.Abs(lat - phi) > delta);

			double alt = r / Math.Cos(lat) - XKMPER * c;

			Point geo=new Point();
			geo.X=lon*180/PI;
			if(geo.X>180)
			{
				geo.X=geo.X-360;
			}
			else if(geo.X<-180)
			{
				geo.X+=360;
			}
			geo.Y =lat*180/PI;
			geo.Z =alt;

			return geo;// degree, degree, kilometers
		}

		public Point GetSunPos()
		{
			return GetSunPos (DateTime.UtcNow);
		}

		/// <summary>
		/// Gets the sun position by UTC time.
		/// </summary>
		/// <returns>The sun position.</returns>
		/// <param name="dt">Dt.</param>
		public Point GetSunPos(DateTime dt)
		{
			return GetSunPos (dt.Year,dt.Month,dt.Day,dt.Hour,dt.Minute,dt.Second);
		}

		/// <summary>
		/// Gets the sun position by UTC time.
		/// </summary>
		/// <returns>The sun position.</returns>
		/// <param name="year">Year.</param>
		/// <param name="mon">Mon.</param>
		/// <param name="day">Day.</param>
		/// <param name="hour">Hour.</param>
		/// <param name="min">Minimum.</param>
		/// <param name="sec">Sec.</param>
		public Point GetSunPos(int year,               // i.e., 2004
		             int mon,                // 1..12
		             int day,                // 1..31
		             int hour,               // 0..23
		             int min,                // 0..59
		             double sec /* = 0.0 */) // 0..(59.999999...)
		{
			double twopi = 2.0 * PI;
			double deg2rad = PI / 180.0;
			double tut1, meanlong, ttdb, meananomaly, eclplong, obliquity, magr, dbi, dbj, dbk;
			double jul=JulianDate(year,mon,day,hour,min,sec);//UTC time
			double gmst=toGMST(jul);

			tut1 = (jul - 2451545.0) / 36525.0;
			meanlong = 280.460 + 36000.77 * tut1;
			meanlong =Mod(meanlong, 360.0);
			ttdb = tut1;
			meananomaly = 357.5277233 + 35999.05034 * ttdb;
			meananomaly = Mod(meananomaly * deg2rad , twopi);
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

			//convert to geo
			Point eciPoint=new Point();
			eciPoint.X=dbi*XKMPER;
			eciPoint.Y=dbj*XKMPER;
			eciPoint.Z=dbk*XKMPER;

			Point geoPoint=toGeo(eciPoint,gmst);
			return geoPoint;
		}

		public List<Point> GetTwilightLine()
		{
			return GetTwilightLine (GetSunPos ());
		}

		List<Point> GetTwilightLine(Point sunpos)
		{
			double LonS, LatS, LonA, LatA, LonB, LatB, ANB, BN, sita;
			LonS = sunpos.X*PI/180;
			LatS = -sunpos.Y*PI/180;
			LonA = LonS - PI / 2;
			LatA = 0;
			List<Point> line=new List<Point>();
			int i=0;
			for (i = 0; i < 360;i++ )
			{
				sita = (i+0.001) * PI / 180;
				BN = Math.Acos(Math.Sin(sita) * Math.Cos(LatS));//(0~pi之间)
				double danb=Math.Sin(LatS) * Math.Sin(sita) / Math.Sin(BN);
				ANB = Math.Asin(danb);//(-pi/2 ~ pi/2之间)
				if(i>90&&i<270)
				{
					LonB = LonA + PI - ANB;
					LonB = LonB > PI ?  LonB -2 * PI : LonB;
					LonB = LonB < -PI ? 2 * PI + LonB : LonB;
					LatB = PI / 2 - BN;
				}
				else
				{
					LonB = LonA + ANB;
					LatB = PI / 2 - BN;
				}
				Point c=new Point();
				c.X=LonB * 180 / PI;
				c.Y=LatB * 180 / PI;
				line.Add(c);
			}
			line.Sort(new PointComparer());
			Point AddP1=new Point (-180, line [0].Y);
			Point AddP2=new Point(180,line[line.Count-1].Y);

			line.Insert (0, AddP1);
			line.Add (AddP2);
			foreach (var p in line) {
				Console.WriteLine(p.X);
				//Console.WriteLine(p.Y);
				//Console.WriteLine(String.Format("x: {0}, y:{1}",p.X,p.Y));
			}

			return line;
		}

		/*
		main()
		{
			Point geoPoint=SunPos(2016, 9, 14, 3, 0, 0);//UTC time
			//printf("%f,%f",geoPoint.x,geoPoint.y);
			DayNightLine(geoPoint);
		}
*/

	}
	public class PointComparer:IComparer<Point>
	{
		public int Compare(Point p1,Point p2)
		{
			return(p1.X .CompareTo(p2.X));
		}
	}
}

