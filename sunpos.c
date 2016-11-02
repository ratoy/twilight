#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include "time.h"

const double EPOCH_JAN1_12H_2000 = 2451545.0;
const double SEC_PER_DAY = 86400.0;       // Seconds per day (solar)
const double OMEGA_E     = 1.00273790934; // Earth rotation per sidereal day
const double PI           = 3.141592653589793;
const double F           = 1.0 / 298.26;
const double XKMPER      = 6378.135;  

double lon1,lat1,alt1;

typedef struct st_coord  
{
    double x;
    double y;
    double z;
}coord;

IsLeapYear(int y)
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

double toGMST(double m_Date) 
{
    const double UT = fmod(m_Date + 0.5, 1.0);
    const double TU = ( m_Date - EPOCH_JAN1_12H_2000 - UT) / 36525.0;

    double GMST = 24110.54841 + TU * 
        (8640184.812866 + TU * (0.093104 - TU * 6.2e-06));

    GMST = fmod(GMST + SEC_PER_DAY * OMEGA_E * UT, SEC_PER_DAY);

    if (GMST < 0.0)
        GMST += SEC_PER_DAY;  // "wrap" negative modulo value

    GMST=(2* PI * (GMST / SEC_PER_DAY));
    return GMST;
}

coord toGeo(coord eci,double gmst)
{
    double theta = atan2(eci.y,eci.x);
    double lon   = fmod(theta -gmst,2*PI);

    if (lon < 0.0) 
        lon += (2*PI);  // "wrap" negative modulo

    double r   = sqrt(sqr(eci.x) + sqr(eci.y));

    double e2  = F * (2.0 - F);
    double lat = atan2(eci.z, r);

    const double delta = 1.0e-07;
    double phi;
    double c;

    do   
    {
        phi = lat;
        c   = 1.0 / sqrt(1.0 - e2 * sqr(sin(phi)));
        lat = atan2(eci.z + XKMPER * c * e2 * sin(phi), r);
    }
    while (fabs(lat - phi) > delta);

    double alt = r / cos(lat) - XKMPER * c;

    coord geo;
    geo.x=lon*180/PI;
    if(geo.x>180)
    {
        geo.x=geo.x-360;
    }
    else if(geo.x<-180)
    {
        geo.x+=360;
    }
    geo.y =lat*180/PI;
    geo.z =alt;

    return geo;// degree, degree, kilometers
}

coord SunPos(int year,               // i.e., 2004
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
    meanlong =fmod(meanlong, 360.0);
    ttdb = tut1;
    meananomaly = 357.5277233 + 35999.05034 * ttdb;
    meananomaly = fmod(meananomaly * deg2rad , twopi);
    if (meananomaly < 0.0)
    {
        meananomaly += twopi;
    }
    eclplong = meanlong + 1.914666471 * sin(meananomaly) + 0.019994643 * sin(2.0 * meananomaly);
    obliquity = 23.439291 - 0.0130042 * ttdb;
    meanlong = meanlong * deg2rad;
    if (meanlong < 0.0)
        meanlong = twopi + meanlong;
    eclplong = eclplong * deg2rad;
    obliquity = obliquity * deg2rad;
    magr = 1.000140612 - 0.016708617 * cos(meananomaly) - 0.000139589 * cos(2.0 * meananomaly);
    //计算得出太阳的地心坐标系坐标
    dbi = magr * cos(eclplong);
    dbj = magr * cos(obliquity) * sin(eclplong);
    dbk = magr * sin(obliquity) * sin(eclplong);

    //convert to geo
    coord ecicoord;
    ecicoord.x=dbi*XKMPER;
    ecicoord.y=dbj*XKMPER;
    ecicoord.z=dbk*XKMPER;

    coord geocoord=toGeo(ecicoord,gmst);
    return geocoord;
}

void DayNightLine(coord sunpos)
{
    double LonS, LatS, LonA, LatA, LonB, LatB, ANB, BN, sita;
    LonS = sunpos.x*PI/180;
    LatS = -sunpos.y*PI/180;
    LonA = LonS - PI / 2;
    LatA = 0;
    coord line[360];
    int i=0;
    for (i = 0; i < 360;i++ )
    {
        sita = (i+0.001) * PI / 180;
        BN = acos(sin(sita) * cos(LatS));//(0~pi之间)
        double danb=sin(LatS) * sin(sita) / sin(BN);
        ANB = asin(danb);//(-pi/2 ~ pi/2之间)
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
        coord c;
        c.x=LonB * 180 / PI;
        c.y=LatB * 180 / PI;
        line[i]=c;
    }

    for(i=0;i<360;i++)
    {
        //printf("%f\n",line[i].x);
        printf("%f\n",line[i].y);
        //printf(" x: %f, y: %f\n",line[i].x,line[i].y);
    }
}

main()
{
    coord geocoord=SunPos(2016, 9, 14, 3, 0, 0);//UTC time
    //printf("%f,%f",geocoord.x,geocoord.y);
    DayNightLine(geocoord);
}
