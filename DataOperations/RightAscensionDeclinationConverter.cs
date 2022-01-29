using System;
using System.Windows.Media.Media3D;

namespace StarView
{
    public class RightAscensionDeclinationConverter
    {
        /// <summary>
        /// Turns right ascension, declination, and distance to xyz coordinates
        /// </summary>
        /// <param name="rightAscension"></param>
        /// <param name="declination"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Point3D RaDecConverter(double rightAscension, double declination, double distance)
        {
            /*int d1 = Convert.ToInt32(rightAscension);
            int m1= Convert.ToInt32((rightAscension - d1) * 60);
            int s1 = Convert.ToInt32(((rightAscension - d1) * 60) - m1);

            int d2 = Convert.ToInt32(declination);
            int m2 = Convert.ToInt32((declination - d2) * 60);
            int s2 = Convert.ToInt32(((declination - d2) * 60) - m2);

            double A = (d1 * 15) + (m1 * 0.25) + (s1 * 0.004166);
            double B = (Math.Abs(d2) + (m2 / 60) + (s2 / 3600)) * Math.Sign(d2);
            double C = distance;
            double C = 500;

            Point3D point = new Point3D();
            point.X = (C * Math.Cos(B)) * Math.Cos(A);
            point.Y = (C * Math.Cos(B)) * Math.Sin(A);
            point.Z = C * Math.Sin(B);*/

            /* Given that the right ascension and declination are already in the correct format (dms converted to decimal)...
            ...there's no need for the above formula, but it might come useful someday */

            // this cool formula from here: http://fmwriters.com/Visionback/Issue14/wbputtingstars.htm
            double C = distance;

            Point3D point = new Point3D();
            point.X = (C * Math.Cos(declination)) * Math.Cos(rightAscension);
            point.Y = (C * Math.Cos(declination)) * Math.Sin(rightAscension);
            point.Z = C * Math.Sin(declination);

            return point;
        }

    }
}
