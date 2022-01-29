using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StarView
{
    public class DeepSkyObjectSorting
    {
        #region Properties

        private static List<DeepSkyObjectItem> ObjectListBase { get; set; }
        public static IEnumerable<DeepSkyObjectItem> ListIndexes { get; set; }

        #endregion

        #region Item Sorting

        public static IEnumerable<DeepSkyObjectItem> SortItems(List<string> catalogues)
        {
            AfterCallSetup();

            FindByCatalogue(catalogues);

            return ListIndexes;
        }

        #endregion

        #region Searching for data

        private static void FindByCatalogue(List<string> types)
        {
            foreach (string item in types)
            {
                switch (item)
                {
                    case "Galaxies":
                        {
                            IEnumerable<DeepSkyObjectItem> Gxy = ObjectListBase.Where(find => find.Type == "Gxy");
                            MergeLists(Gxy);
                            break;
                        }
                    case "Galaxy Clouds":
                        {
                            IEnumerable<DeepSkyObjectItem> GxyCld = ObjectListBase.Where(find => find.Type == "GxyCld");
                            MergeLists(GxyCld);
                            break;
                        }
                    case "Nebulae":
                        {
                            IEnumerable<DeepSkyObjectItem> Neb = ObjectListBase.Where(find => find.Type == "Neb");
                            MergeLists(Neb);
                            break;
                        }
                    case "Planetary Nebulae":
                        {
                            IEnumerable<DeepSkyObjectItem> PN = ObjectListBase.Where(find => find.Type == "PN");
                            MergeLists(PN);
                            break;
                        }
                    case "Globular Clusters":
                        {
                            IEnumerable<DeepSkyObjectItem> GC = ObjectListBase.Where(find => find.Type == "GC");
                            MergeLists(GC);
                            break;
                        }
                    case "Open Clusters":
                        {
                            IEnumerable<DeepSkyObjectItem> OC = ObjectListBase.Where(find => find.Type == "OC");
                            MergeLists(OC);
                            break;
                        }
                    case "Open Clusters with Nebulae":
                        {
                            IEnumerable<DeepSkyObjectItem> OCNeb = ObjectListBase.Where(find => find.Type == "OC+Neb");
                            MergeLists(OCNeb);
                            break;
                        }
                    case "Supernova Remnants":
                        {
                            IEnumerable<DeepSkyObjectItem> SNR = ObjectListBase.Where(find => find.Type == "SNR");
                            MergeLists(SNR);
                            break;
                        }
                    case "Dark Nebulae":
                        {
                            IEnumerable<DeepSkyObjectItem> DN = ObjectListBase.Where(find => find.Type == "DN");
                            MergeLists(DN);
                            break;
                        }
                    default: break;
                }
            }
        }

        #endregion

        #region Helper Functions

        private static void MergeLists(IEnumerable<DeepSkyObjectItem> list)
        {
            if (list == null)
            {
                return;
            }
            else if (ListIndexes == null)
            {
                ListIndexes = list;
            }
            else
            {
                ListIndexes = ListIndexes.Union(list).ToList();
            }
        }

        private static void AfterCallSetup()
        {
            ListIndexes = null;

            if (CultureInfo.CurrentCulture != new CultureInfo("us-US", false))
            {
                CultureInfo.CurrentCulture = new CultureInfo("us-US", false);
            }

            if (CultureInfo.CurrentUICulture != new CultureInfo("us-US", false))
            {
                CultureInfo.CurrentUICulture = new CultureInfo("us-US", false);
            }

            if (FileConverter.deepSkyListComplete == null)
            {
                FileConverter.FileToList("dso");
            }

            if (ObjectListBase == null)
            {
                ObjectListBase = FileConverter.deepSkyListComplete;
            }
        }

        public static void UpdateObjectListBase()
        {
            if (ObjectListBase == FileConverter.deepSkyListComplete)
            {
                return;
            }

            ObjectListBase.Clear();
            ObjectListBase = FileConverter.deepSkyListComplete;
        }

        #endregion
    }
}
