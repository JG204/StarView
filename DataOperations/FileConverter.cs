using System;
using System.Collections.Generic;
using System.IO;

namespace StarView
{
    public class FileConverter
    {
        #region Properties

        public static List<StarItem> starListComplete = new List<StarItem>();
        public static List<DeepSkyObjectItem> deepSkyListComplete = new List<DeepSkyObjectItem>();

        #endregion

        #region Converters

        public static void FileToList(string type)
        {
            if (type == "hyg")
            {
                if (starListComplete != null)
                {
                    starListComplete.Clear();
                }

                List<string> dataBase = new List<string>(File.ReadLines(Environment.CurrentDirectory + "/ExternalFiles/hygdata_v3.csv"));

                List<StarItem> starList = new List<StarItem>();

                for (int i = 1; i < dataBase.Count; i++)
                {
                    StarItem star = new StarItem(dataBase[i]);
                    if (star.Distance < 1000) // stars further than 1000 may not have accurate data and would have been be located on a sphere 100000 parsecs away from a center point
                    {
                        starList.Add(star);
                    }
                }

                starListComplete = starList;

                //GC.Collect();

            }
            else if (type == "dso")
            {
                if (deepSkyListComplete != null)
                {
                    deepSkyListComplete.Clear();
                }

                List<string> dataBase = new List<string>(File.ReadLines(Environment.CurrentDirectory + "/ExternalFiles/dso.csv"));

                List<DeepSkyObjectItem> dsoList = new List<DeepSkyObjectItem>();

                for (int i = 1; i < dataBase.Count; i++)
                {
                    DeepSkyObjectItem obj = new DeepSkyObjectItem(dataBase[i]);
                    dsoList.Add(obj);
                }
                deepSkyListComplete = dsoList;
            }
        }
        #endregion
    }
}
