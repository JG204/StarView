using System.Runtime;
using System.Windows;

namespace StarView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            // Avoiding large heap fragmentation that was most likely caused by large lists of StarItem items
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            //System.Diagnostics.Process.GetCurrentProcess().MinWorkingSet = System.Diagnostics.Process.GetCurrentProcess().MinWorkingSet;

            PageContent.Content = new StartupPage();
        }

        #endregion

        #region Buttons
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PageContent.Content = new StarChartView();
            Button1.IsEnabled = false;
            Button2.IsEnabled = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PageContent.Content = new DeepSkyChartView();
            Button1.IsEnabled = true;
            Button2.IsEnabled = false;
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            PageContent.Content = new StarChartView();
            ContinueButton.Visibility = Visibility.Hidden;
            Button1.Visibility = Visibility.Visible;
            Button2.Visibility = Visibility.Visible;
            Button1.IsEnabled = false;
            Button2.IsEnabled = true;
        }

        #endregion
    }
}
