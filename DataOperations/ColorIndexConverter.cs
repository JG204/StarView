using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace StarView
{
    public class ColorIndexConverter
    {
        #region Properties

        public static List<TemperatureRGBItem> ColorTable = new List<TemperatureRGBItem>();

        #endregion

        #region Setup

        public static void SetUpConverter()
        {
            List<string> dataBase = new List<string>(File.ReadLines(Environment.CurrentDirectory + "/ExternalFiles/bbr_color.csv"));

            List<TemperatureRGBItem> colorList = new List<TemperatureRGBItem>();

            for (int i = 19; i < dataBase.Count; i++)
            {
                TemperatureRGBItem obj = new TemperatureRGBItem(dataBase[i]);
                colorList.Add(obj);
            }
            ColorTable = colorList;
        }

        #endregion

        #region Converters

        public static Color ColorIndexToColor(double colorIndex)
        {
            double temperature = 4600 * (1 / ((0.92 * colorIndex) + 1.7) + 1 / ((0.92 * colorIndex) + 0.62));
            double temperatureRounded = Math.Round(temperature / 100) * 100;

            if (temperatureRounded < 1000)
            {
                temperatureRounded = 1000;
            }
            else if (temperatureRounded > 40000)
            {
                temperatureRounded = 40000;
            }

            Color color = new Color();
            int index = ColorTable.FindIndex(find => find.Temperature == Convert.ToString(temperatureRounded));
            color = ColorTable[index].RGB_0255;

            return color;
        }

        public static Point ColorIndexToPoint(double colorIndex)
        {
            if (colorIndex > 0.25)
            {
                colorIndex += 0.6;
            }

            double temperature;
            if (colorIndex < 0 || colorIndex > -1.5)
            {
                temperature = 5601 / Math.Pow(colorIndex + 0.4, 2f / 3);
            }
            else
            {
                temperature = 4600 * (1 / ((0.92 * colorIndex) + 1.7) + 1 / ((0.92 * colorIndex) + 0.62));
            }

            double temperatureRounded = Math.Round(temperature / 100) * 100;

            if (temperatureRounded < 1000)
            {
                temperatureRounded = 1000;
            }
            else if (temperatureRounded > 40000)
            {
                temperatureRounded = 40000;
            }

            Point point = new Point();
            int index = ColorTable.FindIndex(find => find.Temperature == Convert.ToString(temperatureRounded)) + 1;
            point = ColorTable[index].XY;

            var test = ColorTable[index].RGB_0ff;

            return point;
        }

        #endregion
    }
}
