using System.Windows.Media;

namespace StarView
{
    public class Materials
    {
        public static Color ClassO = Colors.Blue;
        public static Color ClassB = Colors.LightBlue;
        public static Color ClassA = Colors.White;
        public static Color ClassF = Colors.LightYellow;
        public static Color ClassG = Colors.Yellow;
        public static Color ClassK = Colors.PeachPuff;
        public static Color ClassM = Colors.OrangeRed;


        public static Color GetMaterial(string spectralTypeProperty)
        {
            Color material;

            if (spectralTypeProperty == "")
            {
                material = Materials.ClassO;
            }
            else
            {
                switch (spectralTypeProperty[..1])
                {
                    case "O": material = Materials.ClassO; break;
                    case "B": material = Materials.ClassB; break;
                    case "A": material = Materials.ClassA; break;
                    case "F": material = Materials.ClassF; break;
                    case "G": material = Materials.ClassG; break;
                    case "K": material = Materials.ClassK; break;
                    case "M": material = Materials.ClassM; break;
                    default: material = Materials.ClassG; break;
                }
            }

            return material;
        }
    }
}
