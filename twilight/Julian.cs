using System;

namespace twilight
{
	public class Julian
	{
		public Julian ()
		{
		}

		bool IsLeapYear (int y)
		{
			return (y % 4 == 0 && y % 100 != 0) || (y % 400 == 0);
		}

		public double JulianDate (int year,               // i.e., 2004
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

			if (IsLeapYear (year)) {
				// Leap year
				N = F1 - F2 + day - 30;
			} else {
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
				(int)(30.6001 * 14) + 
				1720994.5 + B;  // 1720994.5 = Oct 30, year -1

			double m_Date = NewYears + dblDay;
			return m_Date;
		}
	}
}

