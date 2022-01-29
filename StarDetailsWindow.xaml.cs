using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace StarView
{
    /// <summary>
    /// Interaction logic for StarDetailsWindow.xaml
    /// </summary>
    public partial class StarDetailsWindow : Window
    {

        private string Designation { get; set; }
        private int StarIndex { get; set; }
        private IEnumerable<StarItem> SelectedStars { get; set; }
        private StarItem SelectedStar { get; set; }
        private bool OriginatesFromSelection { get; set; }

        public StarDetailsWindow(IEnumerable<StarItem> selectedStars)
        {
            InitializeComponent();

            OriginatesFromSelection = false;
            StarIndex = StarChartView.SelectedIndex;
            SelectedStars = selectedStars;
            DataContext = SelectedStars.ElementAt(StarIndex);

            DesignationComboBox.SelectedIndex = 0;

            DesignationText.Text = SelectedStars.ElementAt(StarIndex).HIP;

            if (SelectedStars.ElementAt(StarIndex).SpectralType == "")
            {
                return;
            }

            switch (SelectedStars.ElementAt(StarIndex).SpectralType[0])
            {
                case 'O': StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassO.jpg")); break;
                case 'A': StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassA.jpg")); break;
                case 'B': StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassB.jpg")); break;
                case 'F': StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassF.jpg")); break;
                case 'G': StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassG.jpg")); break;
                case 'K': StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassK.jpg")); break;
                case 'M': StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassM.jpg")); break;
                default: StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassNotAvailable.jpg")); break;
            }
        }
        public StarDetailsWindow(StarItem selectedStar)
        {
            InitializeComponent();

            OriginatesFromSelection = true;
            SelectedStar = selectedStar;
            DataContext = SelectedStar;

            DesignationComboBox.SelectedIndex = 0;

            DesignationText.Text = SelectedStar.HIP;

            switch (SelectedStar.SpectralType[0])
            {
                case 'O': StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassO.jpg")); break;
                case 'A': StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassA.jpg")); break;
                case 'B': StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassB.jpg")); break;
                case 'F': StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassF.jpg")); break;
                case 'G': StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassG.jpg")); break;
                case 'K': StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassK.jpg")); break;
                case 'M': StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/ExternalFiles/ClassM.jpg")); break;
                default: StarImage.Source = new BitmapImage(new Uri("/ExternalFiles/ClassNotAvailable.jpg")); break;
            }
        }

        private void DesignationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if ((SelectedStars == null && OriginatesFromSelection == false) || (SelectedStar == null && OriginatesFromSelection == true))
            {
                return;
            }

            if (OriginatesFromSelection == false)
            {
                switch (DesignationComboBox.SelectedValue.ToString().Substring(38))
                {
                    case "HIP": Designation = SelectedStars.ElementAt(StarIndex).HIP; break;
                    case "HD": Designation = SelectedStars.ElementAt(StarIndex).HD; break;
                    case "HR": Designation = SelectedStars.ElementAt(StarIndex).HR; break;
                    case "GL": Designation = SelectedStars.ElementAt(StarIndex).GL; break;
                    case "BF": Designation = SelectedStars.ElementAt(StarIndex).BF; break;
                    case "Proper": Designation = SelectedStars.ElementAt(StarIndex).Proper; break;
                    default: Designation = "error"; break;
                }
            }
            else
            {
                switch (DesignationComboBox.SelectedValue.ToString().Substring(38))
                {
                    case "HIP": Designation = SelectedStar.HIP; break;
                    case "HD": Designation = SelectedStar.HD; break;
                    case "HR": Designation = SelectedStar.HR; break;
                    case "GL": Designation = SelectedStar.GL; break;
                    case "BF": Designation = SelectedStar.BF; break;
                    case "Proper": Designation = SelectedStar.Proper; break;
                    default: Designation = "error"; break;
                }
            }

            DesignationText.Text = Designation;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
