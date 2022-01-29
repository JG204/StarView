using System.Collections.Generic;
using System.Linq;

namespace StarView
{
    public class StarSorting
    {
        #region Properties

        private static List<StarItem> StarListBase { get; set; }
        public static IEnumerable<StarItem> ListIndexes { get; set; }
        private static StarItem DesiredItem { get; set; }

        #endregion

        #region Item Sorting and Searching

        public static IEnumerable<StarItem> SortItems(List<string> catalogues, double distance, string spectralType, double luminosityMin, double luminosityMax, double apparentMagnitudeMin, double apparentMagnitudeMax)
        {
            AfterCallSetup();

            FindByCatalogue(catalogues);
            FindByDistance(distance);
            FindByLuminosity(luminosityMin, luminosityMax);
            FindByBrightness(apparentMagnitudeMin, apparentMagnitudeMax);
            FindBySpectralType(spectralType);

            return ListIndexes;
        }

        public static string LocateItem(string nameType, string name)
        {
            AfterCallSetup();

            FindSpecificStar(nameType, name);

            return DesiredItem.Id;
        }

        #endregion

        #region Searching for Data

        private static void FindSpecificStar(string nameType, string name)
        {
            switch (nameType)
            {
                case "HIP": DesiredItem = StarListBase.Find(find => find.HIP == name); break;
                case "HD": DesiredItem = StarListBase.Find(find => find.HD == name); break;
                case "HR": DesiredItem = StarListBase.Find(find => find.HR == name); break;
                case "GL": DesiredItem = StarListBase.Find(find => find.GL == name); break;
                case "BF(NN)": DesiredItem = StarListBase.Find(find => find.BF == "NN" + name); break;
                case "BF(Gl)": DesiredItem = StarListBase.Find(find => find.BF == "Gl" + name); break;
                case "BF(GJ)": DesiredItem = StarListBase.Find(find => find.BF == "GJ" + name); break;
                case "BF(Wo)": DesiredItem = StarListBase.Find(find => find.BF == "Wo" + name); break;
                case "Proper": DesiredItem = StarListBase.Find(find => find.Proper == name); break;
                default: break;
            }
        }

        private static void FindByCatalogue(List<string> catalogues)
        {
            foreach (string item in catalogues)
            {
                switch (item)
                {
                    case "Hippacros catalog":
                        {
                            IEnumerable<StarItem> HIP = StarListBase.Where(find => find.HIP != "");
                            MergeLists(HIP);
                            break;
                        }
                    case "Henry Draper catalog":
                        {
                            IEnumerable<StarItem> HD = StarListBase.Where(find => find.HD != "");
                            MergeLists(HD);
                            break;
                        }
                    case "Harvard Revised catalog":
                        {
                            IEnumerable<StarItem> HR = StarListBase.Where(find => find.HR != "");
                            MergeLists(HR);
                            break;
                        }
                    case "Gliese Catalog of Nearby Stars":
                        {
                            IEnumerable<StarItem> GL = StarListBase.Where(find => find.GL != "");
                            MergeLists(GL);
                            break;
                        }
                    case "Bayer / Flamsteed designation":
                        {
                            IEnumerable<StarItem> BF = StarListBase.Where(find => find.BF != "");
                            MergeLists(BF);
                            break;
                        }
                    case "Proper Name":
                        {
                            IEnumerable<StarItem> Proper = StarListBase.Where(find => find.Proper != "");
                            MergeLists(Proper);
                            break;
                        }
                    default: break;
                }
            }
        }

        private static void FindByDistance(double distance)
        {
            IEnumerable<StarItem> distanceFound = StarListBase.Where(find => find.Distance <= distance);
            ListIndexes = ListIndexes == null ? distanceFound : ListIndexes = ListIndexes.Intersect(distanceFound).ToList();
        }

        private static void FindBySpectralType(string spectralType)
        {
            if (spectralType == "All")
            {
                return;
            }

            IEnumerable<StarItem> spectralTypeFound = StarListBase.Where(findMin => findMin.SpectralType.FirstOrDefault('M') == spectralType.Last());
            ListIndexes = ListIndexes == null ? spectralTypeFound : ListIndexes = ListIndexes.Intersect(spectralTypeFound).ToList();
        }

        private static void FindByLuminosity(double luminosityMin, double luminosityMax)
        {
            IEnumerable<StarItem> luminosityMinFound = StarListBase.Where(findMin => findMin.Luminosity >= luminosityMin);
            IEnumerable<StarItem> luminosityMaxFound = StarListBase.Where(findMax => findMax.Luminosity <= luminosityMax);
            IEnumerable<StarItem> luminosityFound = luminosityMinFound.Intersect(luminosityMaxFound);
            ListIndexes = ListIndexes == null ? luminosityFound : ListIndexes = ListIndexes.Intersect(luminosityFound).ToList();
        }

        private static void FindByBrightness(double apparentMagnitudeMin, double apparentMagnitudeMax)
        {
            IEnumerable<StarItem> apparentMagnitudeMinFound = StarListBase.Where(findMin => findMin.VisualMagnitude >= apparentMagnitudeMin);
            IEnumerable<StarItem> apparentMagnitudeMaxFound = StarListBase.Where(findMax => findMax.VisualMagnitude <= apparentMagnitudeMax);
            IEnumerable<StarItem> apparentMagnitudeFound = apparentMagnitudeMinFound.Intersect(apparentMagnitudeMaxFound);
            ListIndexes = ListIndexes == null ? apparentMagnitudeFound : ListIndexes = ListIndexes.Intersect(apparentMagnitudeFound).ToList();
        }

        #endregion

        #region Helper Functions

        private static void MergeLists(IEnumerable<StarItem> list)
        {
            if (list == null)
            {
                return;
            }
            else if (ListIndexes == null)
            {
                ListIndexes = list.Where(find => find.ColorIndex != "");
            }
            else
            {
                ListIndexes = ListIndexes.Union(list.Where(find => find.ColorIndex != "")).ToList();
            }
        }

        private static void AfterCallSetup()
        {
            //GC.Collect();

            ListIndexes = Enumerable.Empty<StarItem>();

            if (FileConverter.starListComplete == null)
            {
                FileConverter.FileToList("hyg");
            }

            if (StarListBase == null)
            {
                StarListBase = FileConverter.starListComplete;
            }
        }

        public static void UpdateStarListBase()
        {
            if (StarListBase == FileConverter.starListComplete)
            {
                return;
            }

            StarListBase.Clear();
            StarListBase = FileConverter.starListComplete;
        }

        #endregion
    }

}
