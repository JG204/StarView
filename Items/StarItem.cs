using System;
using System.Globalization;
using System.Windows.Media.Media3D;

namespace StarView
{
    /// <summary>
    /// A ridiculously long list of properties available in "ExternalFiles/hygdata_v3.csv" file
    /// </summary>
    public class StarItem
    {
        #region Constructor
        public StarItem(string data)
        {
            string[] separatedData = data.Split(',');
            Id = separatedData[0];
            HIP = separatedData[1];
            HD = separatedData[2];
            HR = separatedData[3];
            GL = separatedData[4];
            BF = separatedData[5];
            Proper = separatedData[6];
            RightAscension = separatedData[7];
            Declination = separatedData[8];
            Distance = Convert.ToDouble(separatedData[9], CultureInfo.InvariantCulture);
            //PM_RightAscension = separatedData[10];
            //PM_Declination = separatedData[11];
            //RadialVelocity = separatedData[12];
            VisualMagnitude = Convert.ToDouble(separatedData[13], CultureInfo.InvariantCulture);
            AbsoluteVisualMagnitude = Convert.ToDouble(separatedData[14], CultureInfo.InvariantCulture);
            SpectralType = separatedData[15];
            ColorIndex = separatedData[16];
            X = separatedData[17];
            Y = separatedData[18];
            Z = separatedData[19];
            //VX = separatedData[20];
            //VY = separatedData[21];
            //VZ = separatedData[22];
            RightAscension_Radians = separatedData[23];
            Declination_Radians = separatedData[24];
            //PM_RightAscension_Radians = separatedData[25];
            //PM_Declination_Radians = separatedData[26];
            //BayerDesignation = separatedData[27];
            //FlamsteedNumber = separatedData[28];
            Constellation = separatedData[29];
            //Companion = separatedData[30];
            //Companion_Primary = separatedData[31];
            //Base = separatedData[32];
            Luminosity = Convert.ToDouble(separatedData[33], CultureInfo.InvariantCulture);
            //VariableDesignation = separatedData[34];
            //var_min = separatedData[35];
            //var_max = separatedData[36];
            SpareProp1 = RightAscensionDeclinationConverter.RaDecConverter(Convert.ToDouble(RightAscension_Radians, CultureInfo.InvariantCulture), Convert.ToDouble(Declination_Radians, CultureInfo.InvariantCulture), StarChartView.StarDistance);



        }
        #endregion

        #region Properties
        public string Id { get; set; } // Primary key
        public string HIP { get; set; }  // The star's ID in the Hipparcos catalog, if known.
        public string HD { get; set; } // The star's ID in the Henry Draper catalog, if known.
        public string HR { get; set; } // The star's ID in the Harvard Revised catalog, which is the same as its number in the Yale Bright Star Catalog.
        public string GL { get; set; } // The star's ID in the third edition of the Gliese Catalog of Nearby Stars.
        public string BF { get; set; } // The Bayer / Flamsteed designation
        public string Proper { get; set; } // name
        public string RightAscension { get; set; } // for epoch and equinox 2000.0.
        //public string RadialVelocity { get; set; }
        public string Declination { get; set; } // for epoch and equinox 2000.0.
        public double Distance { get; set; } // In Parsecs
        //public string PM_RightAscension { get; set; } // proper motion in ...
        //public string PM_Declination { get; set; } // proper motion in ...
        public double VisualMagnitude { get; set; }
        public double AbsoluteVisualMagnitude { get; set; }
        public string SpectralType { get; set; } // if known
        public string ColorIndex { get; set; }
        public string X { get; set; } // The Cartesian coordinates of the star, in a system based on the equatorial coordinates as seen from Earth.
        public string Y { get; set; } // ^
        public string Z { get; set; } // ^
        //public string VX { get; set; } // The Cartesian velocity components of the star
        //public string VY { get; set; } // ^
        //public string VZ { get; set; } // ^
        public string RightAscension_Radians { get; set; }
        public string Declination_Radians { get; set; }
        //public string PM_RightAscension_Radians { get; set; }
        //public string PM_Declination_Radians { get; set; }
        //public string BayerDesignation { get; set; }
        //public string FlamsteedNumber { get; set; }
        public string Constellation { get; set; } //abbrevation
        //public string Companion { get; set; } //id of companion star if in a multiple star system
        //public string Companion_Primary { get; set; } // id of primary star
        //public string Base { get; set; } // catalog ID or name for this multi-star system.
        public double Luminosity { get; set; } // as a multiple of Solar luminosity
        //public string VariableDesignation { get; set; } // Star's standard variable star designation, when known
        //public string var_min { get; set; } // min magnitude for variables
        //public string var_max { get; set; } // max magnitude for variables
        public Point3D SpareProp1 { get; set; }

        #endregion

    }
}
