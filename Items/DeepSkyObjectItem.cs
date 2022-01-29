using System;
using System.Globalization;
using System.Windows.Media.Media3D;

namespace StarView
{
    public class DeepSkyObjectItem
    {
        public DeepSkyObjectItem(string data)
        {
            string[] separatedData = data.Split(',');
            //RightAscension = separatedData[0];
            //Declination = separatedData[1];
            Type = separatedData[2];
            Constellation = separatedData[3];
            //VisualMagnitude = separatedData[4];
            Name = separatedData[5];
            RightAscension_Radians = separatedData[6];
            Declination_Radians = separatedData[7];
            Id = separatedData[8];
            //R1 = separatedData[9];
            //R2 = separatedData[10];
            //Dso_Source = separatedData[11];
            //Id1 = separatedData[12];
            Cat1 = separatedData[13];
            //Id2 = separatedData[14];
             Cat2 = separatedData[15];
            //DupId = separatedData[16];
            //DuCat = separatedData[17];
            //Display_Mag = separatedData[18];
            PositionsOnSphere = RightAscensionDeclinationConverter.RaDecConverter(Convert.ToDouble(RightAscension_Radians,CultureInfo.InvariantCulture), Convert.ToDouble(Declination_Radians, CultureInfo.InvariantCulture), DeepSkyChartView.ObjectDistance);
        }

        public string RightAscension { get; set; }
        public string Declination { get; set; }
        public string Type { get; set; }
        public string Constellation { get; set; }
        //public string VisualMagnitude { get; set; }
        public string Name { get; set; }
        public string RightAscension_Radians { get; set; }
        public string Declination_Radians { get; set; }
        public string Id { get; set; }
        //public string R1 { get; set; } //Semi-major axes of the object, in arcminutes
        //public string R2 { get; set; } //Semi-minor axes of the object, in arcminutes
        //public string Dso_Source { get; set; }
        //public string Id1 { get; set; } //Primary (most commonly used) ID number/designation for this object.
        public string Cat1 { get; set; } //Primary (most commonly used) catalog name for this object.
        //public string Id2 { get; set; }
        public string Cat2 { get; set; }
        //public string DupId { get; set; } //Duplicate ID number+catalog name. Unlike id2 and cat2, a duplicate ID normally means this object is better known by the duplicate ID, and should be excluded from display.
        //public string DuCat { get; set; } //Duplicate ID number+catalog name. Unlike id2 and cat2, a duplicate ID normally means this object is better known by the duplicate ID, and should be excluded from display.
        //public string Display_Mag { get; set; } //For objects whose actual magnitude is either not known or is not representative of their visibility (such as very large diffuse nebulas like the Veil or North America Nebula), this is a suggested magnitude cutoff for chart drawing software. This field can be safely ignored for other purposes.
        public Point3D PositionsOnSphere { get; set; }
    }

}
