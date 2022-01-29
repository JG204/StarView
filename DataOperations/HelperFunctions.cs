using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StarView
{
    public class HelperFunctions
    {
        #region Functions

        /// <summary>
        /// Sets the the case of a string to the correct case
        /// </summary>
        /// <param name="stringToConvert"></param>
        /// <returns></returns>
        public static string ConvertToCorrectCase(string stringToConvert, TextBox textBox)
        {
            string correctedString = "";
            string[] separatedData = textBox.Text.Split(' ');
            for (int i = 0; i < separatedData.Length; i++)
            {
                if (separatedData[i] == "")
                {
                    continue;
                }

                correctedString += separatedData[i][0].ToString().ToUpper();
                correctedString += separatedData[i][1..].ToLower();
                if (i < separatedData.Length - 1)
                {
                    correctedString += " ";
                }
            }
            return correctedString;
        }

        /// <summary>
        /// Changes the text in a textbox to a different text for a specified time
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textTemp"></param>
        /// <param name="textBox"></param>
        /// <param name="enabledButton"></param>
        /// <param name="timeSpan"></param>
        public static async void ChangeTextAndBack(string text, string textTemp, TextBox textBox, Button enabledButton, int timeSpan)
        {
            enabledButton.IsEnabled = false;
            textBox.IsReadOnly = true;
            textBox.Text = textTemp;
            await Task.Delay(timeSpan);
            textBox.Text = text;
            textBox.IsReadOnly = false;
            enabledButton.IsEnabled = true;
        }

        /// <summary>
        /// Accesses the property of an object from a string
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string StringToPropertyValue(object obj, string property)
        {
            if (property == "-")
            {
                return "0";
            }

            string propData = Convert.ToString(obj.GetType().GetProperty(property)?.GetValue(obj, null), CultureInfo.InvariantCulture);
            if (propData == null || propData == "")
            {
                return "0";
            }

            return propData;
        }

        public static double DegreesToRadians(double degrees)
        {
            return (Math.PI / 180) * Convert.ToDouble(degrees);
        }
        public static double DegreesToRadians(string degrees)
        {
            return (Math.PI / 180) * Convert.ToDouble(degrees, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
