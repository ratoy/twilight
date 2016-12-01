using System;

namespace twilight
{
	public class MoonPos
	{
		/*  Astronomical constants  */
		const double epoch = 2444238.5;
		/* 1980 January 0.0 */
		/*  Constants defining the Sun's apparent orbit  */
		const double elonge = 278.833540;
		/* Ecliptic longitude of the Sun
                                      at epoch 1980.0 */
		const double elongp = 282.596403;
		/* Ecliptic longitude of the Sun at
                                      perigee */
		const double eccent = 0.016718;
		/* Eccentricity of Earth's orbit */
		const double sunsmax = 1.495985e8;
		/* Semi-major axis of Earth's orbit, km */
		const double sunangsiz = 0.533128;
		/* Sun's angular size, degrees, at
                                      semi-major axis distance */
		/*  Elements of the Moon's orbit, epoch 1980.0  */
		const double mmlong = 64.975464;
		/* Moon's mean longitude at the epoch */
		const double mmlongp = 349.383063;
		/* Mean longitude of the perigee at the
                                      epoch */
		const double mlnode = 151.950429;
		/* Mean longitude of the node at the
                                      epoch */
		const double minc = 5.145396;
		/* Inclination of the Moon's orbit */
		const double mecc = 0.054900;
		/* Eccentricity of the Moon's orbit */
		const double mangsiz = 0.5181;
		/* Moon's angular size at distance a
                                      from Earth */
		const double msmax = 384401.0;
		/* Semi-major axis of Moon's orbit in km */
		const double mparallax = 0.9507;
		/* Parallax at distance a from Earth */
		const double synmonth = 29.53058868;
		/* Synodic month (new Moon to new Moon) */
		const double lunatbase = 2423436.0;
		/* Base date for E. W. Brown's numbered
                                      series of lunations (1923 January 16) */
		/*  Properties of the Earth  */
		const double earthrad = 6378.16;
		/* Radius of Earth in kilometres */
		const double PI = 3.14159265358979323846;
		/* Assume not near black hole nor in
                                      Tennessee */
		Julian julian = new Julian ();

		public MoonPos ()
		{
		}

		double DegToRad (double deg)
		{
			return deg * Math.PI / 180.0;
		}

		double RadToDeg (double rad)
		{
			return rad * 180.0 / Math.PI;
		}

		double FixAngle (double a)
		{
			return a - 360.0 * (Math.Floor (a / 360.0));
		}
		/* Fix angle    */
		/*  KEPLER  --   Solve the equation of Kepler.  */
		double kepler (double m, double ecc)
		{
			double e, delta;
			double EPSILON = 1E-6;

			e = m = DegToRad (m);
			do {
				delta = e - ecc * Math.Sin (e) - m;
				e -= delta / (1 - ecc * Math.Cos (e));
			} while (Math.Abs(delta) > EPSILON);
			return e;
		}

		public Point GetMoonPos ()
		{
			return GetMoonPos (DateTime.UtcNow);
		}

		public Point GetMoonPos (DateTime dtUTC)
		{
			double Day, N, M, Ec, Lambdasun, ml, MM, MN, Ev, Ae, A3, MmP,
			mEc, A4, lP, V, lPP, NP, y, x, Lambdamoon, BetaM,
			F, SunDist, SunAng, pdate;

			pdate = julian.JulianDate (dtUTC.Year, dtUTC.Month, dtUTC.Day, dtUTC.Hour, dtUTC.Minute, dtUTC.Second);

			/* Calculation of the Sun's position */

			Day = pdate - epoch;                    /* Date within epoch */
			N = FixAngle ((360 / 365.2422) * Day);   /* Mean anomaly of the Sun */
			M = FixAngle (N + elonge - elongp);      /* Convert from perigee
                                               co-ordinates to epoch 1980.0 */
			Ec = kepler (M, eccent);                 /* Solve equation of Kepler */
			Ec = Math.Sqrt ((1 + eccent) / (1 - eccent)) * Math.Tan (Ec / 2);
			Ec = 2 * RadToDeg (Math.Atan (Ec));               /* True anomaly */
			Lambdasun = FixAngle (Ec + elongp);      /* Sun's geocentric ecliptic
                                               longitude */
			/* Orbital distance factor */
			F = ((1 + eccent * Math.Cos (DegToRad (Ec))) / (1 - eccent * eccent));
			SunDist = sunsmax / F;                  /* Distance to Sun in km */
			SunAng = F * sunangsiz;                 /* Sun's angular size in degrees */

			/* Calculation of the Moon's position */

			/* Moon's mean longitude */
			ml = FixAngle (13.1763966 * Day + mmlong);

			/* Moon's mean anomaly */
			MM = FixAngle (ml - 0.1114041 * Day - mmlongp);

			/* Moon's ascending node mean longitude */
			MN = FixAngle (mlnode - 0.0529539 * Day);

			/* Evection */
			Ev = 1.2739 * Math.Sin (DegToRad (2 * (ml - Lambdasun) - MM));

			/* Annual equation */
			Ae = 0.1858 * Math.Sin (DegToRad (M));

			/* Correction term */
			A3 = 0.37 * Math.Sin (DegToRad (M));

			/* Corrected anomaly */
			MmP = MM + Ev - Ae - A3;

			/* Correction for the equation of the centre */
			mEc = 6.2886 * Math.Sin (DegToRad (MmP));

			/* Another correction term */
			A4 = 0.214 * Math.Sin (DegToRad (2 * MmP));

			/* Corrected longitude */
			lP = ml + Ev + mEc - Ae + A4;

			/* Variation */
			V = 0.6583 * Math.Sin (DegToRad (2 * (lP - Lambdasun)));

			/* True longitude */
			lPP = lP + V;

			/* Corrected longitude of the node */
			NP = MN - 0.16 * Math.Sin (DegToRad (M));

			/* Y inclination coordinate */
			y = Math.Sin (DegToRad (lPP - NP)) * Math.Cos (DegToRad (minc));

			/* X inclination coordinate */
			x = Math.Cos (DegToRad (lPP - NP));

			/* Ecliptic longitude */
			Lambdamoon = RadToDeg (Math.Atan2 (y, x));
			Lambdamoon += NP;

			//lon in -180~180
			double lon = Lambdamoon;
			lon = lon > 180 ? lon - 360 : lon;
			lon = lon < -180 ? lon + 360 : lon;
			/* Ecliptic latitude */
			BetaM = RadToDeg (Math.Asin (Math.Sin (DegToRad (lPP - NP)) * Math.Sin (DegToRad (minc))));
			double lat = BetaM;
			return new Point (lon, lat);
		}
	}
}


