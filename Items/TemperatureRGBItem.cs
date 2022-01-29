using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace StarView
{
    public class TemperatureRGBItem
    {
        public TemperatureRGBItem(string data)
        {
            string[] separatedData = data.Split(',');

            var test = separatedData.Count();

            Temperature = separatedData[0];
            CMF = separatedData[2];
            XY = new Point(Convert.ToDouble(separatedData[3], CultureInfo.InvariantCulture), Convert.ToDouble(separatedData[4], CultureInfo.InvariantCulture));
            P = separatedData[5];
            RGB_01 = separatedData[6] + " " + separatedData[7] + " " + separatedData[8];
            byte stest = Convert.ToByte(separatedData[9]);
            byte stes2t = Convert.ToByte(separatedData[10]);
            byte stes3t = Convert.ToByte(separatedData[11]);
            RGB_0255 = Color.FromRgb(Convert.ToByte(separatedData[9]), Convert.ToByte(separatedData[10]), Convert.ToByte(separatedData[11]));
            RGB_0ff = separatedData[12];
        }

        public string Temperature { get; set; }
        public string CMF { get; set; }
        public Point XY { get; set; }
        public string P { get; set; }
        public string RGB_01 { get; set; }
        public Color RGB_0255 { get; set; }
        public string RGB_0ff { get; set; }

    }

}
