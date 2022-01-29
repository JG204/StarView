using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace StarView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class StarChartView : Page
    {
        #region Properties
        public bool MoreOptionsWindowVisible { get; set; }
        public bool IsStarView { get; set; }
        public bool IsPerspectiveView { get; set; }
        public static bool VisibleAxes { get; set; }
        public List<string> CheckBoxes = new List<string>();
        private Viewport3D TargetViewPort { get; set; }
        private Viewport3D OtherViewPort { get; set; }
        public static double StarDistance { get; set; }
        public static float StarSize { get; set; }
        public static IEnumerable<StarItem> SelectedItems { get; set; }
        public static int SelectedIndex { get; set; }
        private static StarItem DesiredItem { get; set; }
        #endregion

        #region Constructors
        public StarChartView()
        {
            InitializeComponent();

            if (FileConverter.starListComplete.Count == 0 || FileConverter.starListComplete == null)
            {
                FileConverter.FileToList("hyg");
            }

            if (ColorIndexConverter.ColorTable.Count == 0)
            {
                ColorIndexConverter.SetUpConverter();
            }
        }
        #endregion

        #region Window Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PreviousButton.IsEnabled = false;
            NextButton.IsEnabled = false;
            DetailsButton.IsEnabled = false;
            IsStarView = true;
            VisibleAxes = false;

            MoreOptionsWindow.Visibility = Visibility.Hidden;
            MoreOptionsWindowVisible = false;

            StarDistance = StarDistanceSlider.Value;
            StarSize = 0.025f;

            CreateChart(StarSorting.SortItems(CheckBoxes, Convert.ToDouble(DistanceText.Text, CultureInfo.InvariantCulture), SpectralType.Text, Convert.ToDouble(LuminosityMinText.Text,
                                              CultureInfo.InvariantCulture), Convert.ToDouble(LuminosityMaxText.Text, CultureInfo.InvariantCulture), Convert.ToDouble(BrightnessMinText.Text, CultureInfo.InvariantCulture),
                                              Convert.ToDouble(BrightnessMaxText.Text, CultureInfo.InvariantCulture)));
            TransformChart();
            GetStarsInfo(-1);
        }
        #endregion

        #region Sliders
        private void Slider_RoundTo3(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(((Slider)sender).Value, 3);

        }
        private void Slider_RoundTo2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(((Slider)sender).Value, 2);
        }
        #endregion

        #region Checkboxes
        private void Catalogue_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxes.Add((string)((CheckBox)sender).Content);
        }
        private void Catalogue_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBoxes.Remove((string)((CheckBox)sender).Content);
        }
        private void VisibleAxes_Checked(object sender, RoutedEventArgs e)
        {
            VisibleAxes = true;
        }
        private void VisibleAxes_Unchecked(object sender, RoutedEventArgs e)
        {
            VisibleAxes = false;
        }
        private void PerspectiveView_Checked(object sender, RoutedEventArgs e)
        {
            IsPerspectiveView = true;
        }
        private void PerspectiveView_Unchecked(object sender, RoutedEventArgs e)
        {
            IsPerspectiveView = false;
        }
        #endregion

        #region Buttons
        private void GetInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (GetTypeTextBox.Text == "")
            {
                return;
            }

            switch (GetTypeComboBox.SelectedIndex)
            {
                case 0: DesiredItem = FileConverter.starListComplete.Find(find => find.HIP == GetTypeTextBox.Text); break;
                case 1: DesiredItem = FileConverter.starListComplete.Find(find => find.HD == GetTypeTextBox.Text); break;
                case 2: DesiredItem = FileConverter.starListComplete.Find(find => find.HR == GetTypeTextBox.Text); break;
                case 3: DesiredItem = FileConverter.starListComplete.Find(find => find.GL == GetTypeTextBox.Text); break;
                case 4: DesiredItem = FileConverter.starListComplete.Find(find => find.BF == GetTypeTextBox.Text); break;
                case 5: DesiredItem = FileConverter.starListComplete.Find(find => find.Proper == HelperFunctions.ConvertToCorrectCase(GetTypeTextBox.Text, GetTypeTextBox)); break;
                default: break;
            }

            if (DesiredItem == null)
            {
                HelperFunctions.ChangeTextAndBack(GetTypeTextBox.Text, "No such item in file", GetTypeTextBox, GetInfoButton, 1000);
                return;
            }

            double temperature = 0;
            if (DesiredItem.ColorIndex != "")
            {
                double colorIndex = Convert.ToDouble(DesiredItem.ColorIndex, CultureInfo.InvariantCulture);
                temperature = 4600 * (1 / ((0.92 * colorIndex) + 1.7) + 1 / ((0.92 * colorIndex) + 0.62));
                StarInfo_1.Text = "Temperature:  ~" + Math.Round(temperature) + "K      Distance: " + DesiredItem.Distance + "pc      Color Index: " + DesiredItem.ColorIndex + "      Brightness: " +
                                  DesiredItem.VisualMagnitude + "      Luminosity: " + Math.Round(DesiredItem.Luminosity, 7) + "      Spectral Type: " + DesiredItem.SpectralType + "      Constellation: " +
                                  DesiredItem.Constellation;
            }
            else
            {
                StarInfo_1.Text = "Temperature:  -" + "Distance: " + DesiredItem.Distance + "pc      Color Index: " + DesiredItem.ColorIndex + "      Brightness: " + DesiredItem.VisualMagnitude +
                                  "      Luminosity: " + Math.Round(DesiredItem.Luminosity, 7) + "      Spectral Type: " + DesiredItem.SpectralType + "      Constellation: " + DesiredItem.Constellation;
            }

            StarDetailsWindow starDetailsWindow = new StarDetailsWindow(DesiredItem);
            starDetailsWindow.Show();

        }
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxes.Count == 0)
            {
                return;
            }

            CreateChart(StarSorting.SortItems(CheckBoxes, Convert.ToDouble(DistanceText.Text, CultureInfo.InvariantCulture), SpectralType.Text, Convert.ToDouble(LuminosityMinText.Text,
                                              CultureInfo.InvariantCulture), Convert.ToDouble(LuminosityMaxText.Text, CultureInfo.InvariantCulture), Convert.ToDouble(BrightnessMinText.Text, CultureInfo.InvariantCulture),
                                              Convert.ToDouble(BrightnessMaxText.Text, CultureInfo.InvariantCulture)));
            GetStarsInfo(-1);
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
        }
        private void SelectedStarAction(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == PreviousButton.Name)
            {

                SelectedIndex--;
                if (SelectedIndex == 0)
                {
                    PreviousButton.IsEnabled = false;
                }

                GetStarsInfo(SelectedIndex);

                if (NextButton.IsEnabled == false)
                {
                    NextButton.IsEnabled = true;
                }
            }
            else if (((Button)sender).Name == NextButton.Name)
            {
                SelectedIndex++;
                if (SelectedIndex == SelectedItems.Count() - 1)
                {
                    NextButton.IsEnabled = false;
                }

                GetStarsInfo(SelectedIndex);

                if (PreviousButton.IsEnabled == false)
                {
                    PreviousButton.IsEnabled = true;
                }
            }
            else if (((Button)sender).Name == DetailsButton.Name)
            {
                if (!SelectedItems.Any())
                {
                    return;
                }

                StarDetailsWindow starDetailsWindow = new StarDetailsWindow(SelectedItems);
                //starDetailsWindow.Owner = this;
                starDetailsWindow.Show();
            }
        }
        private void ShowMore_Click(object sender, RoutedEventArgs e)
        {
            if (MoreOptionsWindow.Visibility == Visibility.Hidden)
            {
                MoreOptionsWindow.Visibility = Visibility.Visible;
                ShowMoreButton.Content = "> Show Less";
                MoreOptionsWindowVisible = true;
            }
            else
            {
                MoreOptionsWindow.Visibility = Visibility.Hidden;
                ShowMoreButton.Content = "< Show More";
                MoreOptionsWindowVisible = false;
            }
        }
        private void ChartGraph_Toggle(object sender, RoutedEventArgs e)
        {
            if ((string)((Button)sender).Content == "Go to: GraphView")
            {
                ((Button)sender).Content = "Go to: StarView";
                MoreOptionsWindow.Visibility = Visibility.Hidden;
                StarViewControlPanel.Visibility = Visibility.Collapsed;
                StarViewControlPanelTooltips.Visibility = Visibility.Hidden;
                GraphViewControlPanel.Visibility = Visibility.Visible;
                VisibleAxes = (bool)VisibleAxes_GraphView.IsChecked;
                IsPerspectiveView = (bool)PerspectiveGraphView.IsChecked;
                IsStarView = false;
            }
            else
            {
                ((Button)sender).Content = "Go to: GraphView";
                if (MoreOptionsWindowVisible == true)
                {
                    MoreOptionsWindow.Visibility = Visibility.Visible;
                }

                StarViewControlPanel.Visibility = Visibility.Visible;
                StarViewControlPanelTooltips.Visibility = Visibility.Visible;
                GraphViewControlPanel.Visibility = Visibility.Collapsed;
                VisibleAxes = (bool)VisibleAxes_StarView.IsChecked;
                IsPerspectiveView = (bool)PerspectiveStarView.IsChecked;
                IsStarView = true;
            }
        }
        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            FileConverter.FileToList("hyg");
            StarSorting.UpdateStarListBase();
        }
        #endregion

        #region Textboxes
        private void StarDistanceText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (StarDistanceText == null)
            {
                return;
            }

            StarDistance = Convert.ToDouble(StarDistanceText.Text, CultureInfo.InvariantCulture);
        }

        #endregion

        #region 3D View

        // transform class object for rotate the 3d model
        public TransformMatrix m_transformMatrix = new TransformMatrix();

        // ***************************** 3d chart ***************************
        private Chart3D m_3dChart;       // data for 3d chart
        public int m_nChartModelIndex = -1;         // model index in the Viewport3d
        public int m_nSurfaceChartGridNo = 100;     // surface chart grid no. in each axis
        public int m_nScatterPlotDataNo = 5000;     // total data number of the scatter plot

        // ***************************** selection rect ***************************
        ViewportRect m_selectRect = new ViewportRect();
        public int m_nRectModelIndex = -1;

        public void CreateChart(IEnumerable<StarItem> list)
        {
            // Clearing and setting viewports
            if (IsPerspectiveView == true)
            {
                TargetViewPort = ChartViewPerspective;
                OtherViewPort = ChartViewOrthographic;

                TargetViewPort.Children.Clear();
                TargetViewPort.Children.Add(new ModelVisual3D() { Content = new AmbientLight(Colors.White) });
                OtherViewPort.Children.Clear();
                OtherViewPort.Children.Add(new ModelVisual3D() { Content = new AmbientLight(Colors.White) });

                InitializeSelectionRect(TargetViewPort);
            }
            else
            {
                TargetViewPort = ChartViewOrthographic;
                OtherViewPort = ChartViewPerspective;

                TargetViewPort.Children.Clear();
                TargetViewPort.Children.Add(new ModelVisual3D() { Content = new AmbientLight(Colors.White) });
                OtherViewPort.Children.Clear();
                OtherViewPort.Children.Add(new ModelVisual3D() { Content = new AmbientLight(Colors.White) });

                InitializeSelectionRect(TargetViewPort);
            }

            // 1. set the scatter plot size
            m_3dChart = new ScatterChart3D();
            m_3dChart.SetDataNo(list.Count());

            // 2. set the properties of each dot
            int itemNumber = 0;

            if (IsStarView == true)
            {
                foreach (StarItem item in list)
                {
                    ScatterPlotItem chartObject = new ScatterPlotItem();

                    chartObject.w = Convert.ToSingle(StarSizeSlider.Value, CultureInfo.InvariantCulture);
                    chartObject.h = Convert.ToSingle(StarSizeSlider.Value, CultureInfo.InvariantCulture);

                    if (StarsOnSphere.IsChecked == false)
                    {
                        chartObject.x = Convert.ToSingle(item.X, CultureInfo.InvariantCulture) * Convert.ToSingle(StarDistanceMultiplierSlider.Value, CultureInfo.InvariantCulture) / 5;
                        chartObject.y = Convert.ToSingle(item.Y, CultureInfo.InvariantCulture) * Convert.ToSingle(StarDistanceMultiplierSlider.Value, CultureInfo.InvariantCulture) / 5;
                        chartObject.z = Convert.ToSingle(item.Z, CultureInfo.InvariantCulture) * Convert.ToSingle(StarDistanceMultiplierSlider.Value, CultureInfo.InvariantCulture) / 5;
                    }
                    else
                    {

                        chartObject.x = Convert.ToSingle(item.SpareProp1.X, CultureInfo.InvariantCulture);
                        chartObject.y = Convert.ToSingle(item.SpareProp1.Y, CultureInfo.InvariantCulture);
                        chartObject.z = Convert.ToSingle(item.SpareProp1.Z, CultureInfo.InvariantCulture);
                    }
                    if (ViewSphere.IsChecked == false)
                    {
                        chartObject.shape = (int)Chart3D.SHAPE.BAR;
                    }
                    else
                    {
                        chartObject.shape = (int)Chart3D.SHAPE.ELLIPSE;
                    } ((ScatterChart3D)m_3dChart).SetVertex(itemNumber, chartObject);

                    itemNumber++;
                }
            }
            else if (IsStarView == false)
            {
                foreach (StarItem item in list)
                {
                    ScatterPlotItem chartObject = new ScatterPlotItem();

                    chartObject.w = Convert.ToSingle(StarSizeSlider.Value, CultureInfo.InvariantCulture);
                    chartObject.h = Convert.ToSingle(StarSizeSlider.Value, CultureInfo.InvariantCulture);

                    string x = HelperFunctions.StringToPropertyValue(item, PropertyXComboBox.Text);
                    string y = HelperFunctions.StringToPropertyValue(item, PropertyYComboBox.Text);
                    string z = HelperFunctions.StringToPropertyValue(item, PropertyZComboBox.Text);


                    chartObject.x = Convert.ToSingle(x, CultureInfo.InvariantCulture);
                    chartObject.y = Convert.ToSingle(y, CultureInfo.InvariantCulture);
                    chartObject.z = Convert.ToSingle(z, CultureInfo.InvariantCulture);

                    if (ViewSphere.IsChecked == false)
                    {
                        chartObject.shape = (int)Chart3D.SHAPE.BAR;
                    }
                    else
                    {
                        chartObject.shape = (int)Chart3D.SHAPE.ELLIPSE;
                    } ((ScatterChart3D)m_3dChart).SetVertex(itemNumber, chartObject);

                    itemNumber++;
                }
            }

            // set axes
            if (VisibleAxes == true)
            {
                m_3dChart.GetDataRange();
                m_3dChart.SetAxes();
            }

            // get Mesh3D array from scatter plot
            ArrayList meshs = ((ScatterChart3D)m_3dChart).GetMeshes();

            // show 3D scatter plot in Viewport3d
            Model3D model3d = new Model3D();
            m_nChartModelIndex = model3d.UpdateModel(meshs, null, m_nChartModelIndex, TargetViewPort, "hyg", itemNumber);

            // set projection matrix
            float viewRange = 0f;
            m_transformMatrix.CalculateProjectionMatrix(0, viewRange, 0, viewRange, 0, viewRange, 0.5);
            TransformChart();
        }

        public void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs args)
        {
            m_transformMatrix.OnKeyDown(args);
            TransformChart();
        }

        public void OnViewportMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs args)
        {
            Point pt = args.GetPosition(TargetViewPort);
            if (args.ChangedButton == MouseButton.Left)         // rotate or drag 3d model
            {
                m_transformMatrix.OnLBtnDown(pt);
            }
            else if (args.ChangedButton == MouseButton.Right)   // select rect
            {
                m_selectRect.OnMouseDown(pt, TargetViewPort, m_nRectModelIndex);
            }
        }

        public void OnViewportMouseMove(object sender, System.Windows.Input.MouseEventArgs args)
        {
            Point pt = args.GetPosition(TargetViewPort);

            if (args.LeftButton == MouseButtonState.Pressed)                // rotate or drag 3d model
            {
                m_transformMatrix.OnMouseMove(pt, TargetViewPort);

                TransformChart();
            }
            else if (args.RightButton == MouseButtonState.Pressed)          // select rect
            {
                m_selectRect.OnMouseMove(pt, TargetViewPort, m_nRectModelIndex);
            }

        }

        public void OnViewportMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs args)
        {
            Point pt = args.GetPosition(TargetViewPort);
            if (args.ChangedButton == MouseButton.Left)
            {
                m_transformMatrix.OnLBtnUp();
            }
            else if (args.ChangedButton == MouseButton.Right)
            {
                if (m_nChartModelIndex == -1)
                {
                    return;
                }
                // 1. get the mesh structure related to the selection rect
                MeshGeometry3D meshGeometry = Model3D.GetGeometry(TargetViewPort, m_nChartModelIndex);
                if (meshGeometry == null)
                {
                    return;
                }

                // 2. set selection in 3d chart
                m_3dChart.Select(m_selectRect, m_transformMatrix, TargetViewPort);

                GetSelectedElements(meshGeometry);

                // 3. update selection display (not working) (need to make a ColorToPoint converter)
                //m_3dChart.HighlightSelection(meshGeometry, Color.FromRgb(200, 200, 200));

            }
        }

        // this function is used to rotate, drag and zoom the 3d chart
        private void TransformChart()
        {
            if (m_nChartModelIndex == -1)
            {
                return;
            }

            ModelVisual3D visual3d = (ModelVisual3D)(TargetViewPort.Children[m_nChartModelIndex]);
            if (visual3d.Content == null)
            {
                return;
            }

            Transform3DGroup group1 = visual3d.Content.Transform as Transform3DGroup;
            group1.Children.Clear();
            group1.Children.Add(new MatrixTransform3D(m_transformMatrix.m_totalMatrix));
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void OnScroll(object sender, MouseWheelEventArgs e)
        {
            m_transformMatrix.OnMouseScroll(e);
            TransformChart();
        }
        #endregion

        #region Helper Functions 
        // Helper functions that only serve their purpose in this specific case, thus I didn't put them in a different file

        /// <summary>
        /// Gets info about selected elements by comparing xyz coordinates of selected objects to objects in a file
        /// </summary>
        /// <param name="meshGeometry"></param>
        private void GetSelectedElements(MeshGeometry3D meshGeometry)
        {
            if (ScatterChart3D.SelectedItems.Count == 0)
            {
                return;
            }

            if (StarsOnSphere.IsChecked == false && IsStarView == true)
            {
                double range = 0;

                if (StarDistanceMultiplierSlider.Value > 8)
                {
                    range = 0.1;
                }
                else if (StarDistanceMultiplierSlider.Value > 6)
                {
                    range = 0.15;
                }
                else if (StarDistanceMultiplierSlider.Value > 4)
                {
                    range = 0.2;
                }
                else if (StarDistanceMultiplierSlider.Value > 2)
                {
                    range = 0.25;
                }
                else if (StarDistanceMultiplierSlider.Value > 0)
                {
                    range = 0.3;
                }

                double desiredX_1 = ScatterChart3D.SelectedItems[0].X * 5 / Convert.ToSingle(StarDistanceMultiplierSlider.Value, CultureInfo.InvariantCulture) - range;
                double desiredY_1 = ScatterChart3D.SelectedItems[0].Y * 5 / Convert.ToSingle(StarDistanceMultiplierSlider.Value, CultureInfo.InvariantCulture) - range;
                double desiredZ_1 = ScatterChart3D.SelectedItems[0].Z * 5 / Convert.ToSingle(StarDistanceMultiplierSlider.Value, CultureInfo.InvariantCulture) - range;

                double desiredX_2 = ScatterChart3D.SelectedItems[0].X * 5 / Convert.ToSingle(StarDistanceMultiplierSlider.Value, CultureInfo.InvariantCulture) + range;
                double desiredY_2 = ScatterChart3D.SelectedItems[0].Y * 5 / Convert.ToSingle(StarDistanceMultiplierSlider.Value, CultureInfo.InvariantCulture) + range;
                double desiredZ_2 = ScatterChart3D.SelectedItems[0].Z * 5 / Convert.ToSingle(StarDistanceMultiplierSlider.Value, CultureInfo.InvariantCulture) + range;

                IEnumerable<StarItem> listX_1 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(find.X, CultureInfo.InvariantCulture) > desiredX_1);
                IEnumerable<StarItem> listY_1 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(find.Y, CultureInfo.InvariantCulture) > desiredY_1);
                IEnumerable<StarItem> listZ_1 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(find.Z, CultureInfo.InvariantCulture) > desiredZ_1);

                IEnumerable<StarItem> listX_2 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(find.X, CultureInfo.InvariantCulture) < desiredX_2);
                IEnumerable<StarItem> listY_2 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(find.Y, CultureInfo.InvariantCulture) < desiredY_2);
                IEnumerable<StarItem> listZ_2 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(find.Z, CultureInfo.InvariantCulture) < desiredZ_2);

                IEnumerable<StarItem> listX = listX_1.Intersect(listX_2);
                IEnumerable<StarItem> listY = listY_1.Intersect(listY_2);
                IEnumerable<StarItem> listZ = listZ_1.Intersect(listZ_2);

                IEnumerable<StarItem> listFinal = new List<StarItem>();

                if (listX.Any())
                {
                    if (!listFinal.Any())
                    {
                        listFinal = StarSorting.ListIndexes;
                    }

                    listFinal = listFinal.Intersect(listX);
                }
                if (listY.Any())
                {
                    if (!listFinal.Any())
                    {
                        listFinal = StarSorting.ListIndexes;
                    }

                    listFinal = listFinal.Intersect(listY);
                }
                if (listZ.Any())
                {
                    if (!listFinal.Any())
                    {
                        listFinal = StarSorting.ListIndexes;
                    }

                    listFinal = listFinal.Intersect(listZ);
                }
                SelectedItems = listFinal;
            }
            else if (StarsOnSphere.IsChecked == true && IsStarView == true)
            {
                double range = 1;

                if (StarDistance > 80)
                {
                    range = 0.05;
                }
                else if (StarDistance > 60)
                {
                    range = 0.045;
                }
                else if (StarDistance > 40)
                {
                    range = 0.040;
                }
                else if (StarDistance > 20)
                {
                    range = 0.035;
                }
                else if (StarDistance > 0)
                {
                    range = 0.030;
                }

                double desiredX_1 = ScatterChart3D.SelectedItems[0].X - range;
                double desiredY_1 = ScatterChart3D.SelectedItems[0].Y - range;
                double desiredZ_1 = ScatterChart3D.SelectedItems[0].Z - range;

                double desiredX_2 = ScatterChart3D.SelectedItems[0].X + range;
                double desiredY_2 = ScatterChart3D.SelectedItems[0].Y + range;
                double desiredZ_2 = ScatterChart3D.SelectedItems[0].Z + range;

                IEnumerable<StarItem> listX_1 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(find.SpareProp1.X, CultureInfo.InvariantCulture) > desiredX_1);
                IEnumerable<StarItem> listY_1 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(find.SpareProp1.Y, CultureInfo.InvariantCulture) > desiredY_1);
                IEnumerable<StarItem> listZ_1 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(find.SpareProp1.Z, CultureInfo.InvariantCulture) > desiredZ_1);

                IEnumerable<StarItem> listX_2 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(find.SpareProp1.X, CultureInfo.InvariantCulture) < desiredX_2);
                IEnumerable<StarItem> listY_2 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(find.SpareProp1.Y, CultureInfo.InvariantCulture) < desiredY_2);
                IEnumerable<StarItem> listZ_2 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(find.SpareProp1.Z, CultureInfo.InvariantCulture) < desiredZ_2);

                IEnumerable<StarItem> listX = listX_1.Intersect(listX_2);
                IEnumerable<StarItem> listY = listY_1.Intersect(listY_2);
                IEnumerable<StarItem> listZ = listZ_1.Intersect(listZ_2);

                IEnumerable<StarItem> listFinal = new List<StarItem>();

                if (listX.Any())
                {
                    if (!listFinal.Any())
                    {
                        listFinal = StarSorting.ListIndexes;
                    }

                    listFinal = listFinal.Intersect(listX);
                }
                if (listY.Any())
                {
                    if (!listFinal.Any())
                    {
                        listFinal = StarSorting.ListIndexes;
                    }

                    listFinal = listFinal.Intersect(listY);
                }
                if (listZ.Any())
                {
                    if (!listFinal.Any())
                    {
                        listFinal = StarSorting.ListIndexes;
                    }

                    listFinal = listFinal.Intersect(listZ);
                }
                SelectedItems = listFinal;

            }
            else if (StarsOnSphere.IsChecked == false && IsStarView == false)
            {
                double range = 0;

                if (StarDistanceMultiplierSlider.Value > 8)
                {
                    range = 0.1;
                }
                else if (StarDistanceMultiplierSlider.Value > 6)
                {
                    range = 0.15;
                }
                else if (StarDistanceMultiplierSlider.Value > 4)
                {
                    range = 0.2;
                }
                else if (StarDistanceMultiplierSlider.Value > 2)
                {
                    range = 0.25;
                }
                else if (StarDistanceMultiplierSlider.Value > 0)
                {
                    range = 0.3;
                }

                double desiredX_1 = ScatterChart3D.SelectedItems[0].X - range;
                double desiredY_1 = ScatterChart3D.SelectedItems[0].Y - range;
                double desiredZ_1 = ScatterChart3D.SelectedItems[0].Z - range;

                double desiredX_2 = ScatterChart3D.SelectedItems[0].X + range;
                double desiredY_2 = ScatterChart3D.SelectedItems[0].Y + range;
                double desiredZ_2 = ScatterChart3D.SelectedItems[0].Z + range;


                IEnumerable<StarItem> listX_1 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(HelperFunctions.StringToPropertyValue(find, PropertyXComboBox.Text), CultureInfo.InvariantCulture) > desiredX_1);
                IEnumerable<StarItem> listY_1 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(HelperFunctions.StringToPropertyValue(find, PropertyYComboBox.Text), CultureInfo.InvariantCulture) > desiredY_1);
                IEnumerable<StarItem> listZ_1 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(HelperFunctions.StringToPropertyValue(find, PropertyZComboBox.Text), CultureInfo.InvariantCulture) > desiredZ_1);

                IEnumerable<StarItem> listX_2 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(HelperFunctions.StringToPropertyValue(find, PropertyXComboBox.Text), CultureInfo.InvariantCulture) < desiredX_2);
                IEnumerable<StarItem> listY_2 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(HelperFunctions.StringToPropertyValue(find, PropertyYComboBox.Text), CultureInfo.InvariantCulture) < desiredY_2);
                IEnumerable<StarItem> listZ_2 = StarSorting.ListIndexes.Where(find => Convert.ToDouble(HelperFunctions.StringToPropertyValue(find, PropertyZComboBox.Text), CultureInfo.InvariantCulture) < desiredZ_2);

                IEnumerable<StarItem> listX = listX_1.Intersect(listX_2);
                IEnumerable<StarItem> listY = listY_1.Intersect(listY_2);
                IEnumerable<StarItem> listZ = listZ_1.Intersect(listZ_2);

                IEnumerable<StarItem> listFinal = new List<StarItem>();

                if (listX.Any())
                {
                    if (!listFinal.Any())
                    {
                        listFinal = StarSorting.ListIndexes;
                    }

                    listFinal = listFinal.Intersect(listX);
                }
                if (listY.Any())
                {
                    if (!listFinal.Any())
                    {
                        listFinal = StarSorting.ListIndexes;
                    }

                    listFinal = listFinal.Intersect(listY);
                }
                if (listZ.Any())
                {
                    if (!listFinal.Any())
                    {
                        listFinal = StarSorting.ListIndexes;
                    }

                    listFinal = listFinal.Intersect(listZ);
                }
                SelectedItems = listFinal;

            }

            SelectedIndex = 0;

            GetStarsInfo(SelectedIndex);
            if (SelectedItems.Count() > 1)
            {
                NextButton.IsEnabled = true;
            }

            if (SelectedItems.Any())
            {
                DetailsButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Draws a selection rectangle on the screen
        /// </summary>
        /// <param name="viewport"></param>
        private void InitializeSelectionRect(Viewport3D viewport)
        {
            m_selectRect.SetRect(new Point(-0.5, -0.5), new Point(-0.5, -0.5));
            Model3D model3d_ = new Model3D();
            ArrayList meshs_ = m_selectRect.GetMeshes();
            m_nRectModelIndex = model3d_.UpdateModel(meshs_, null, m_nRectModelIndex, viewport, "hyg", 0);
        }

        /// <summary>
        /// Gets info about a selected object from SelectedItems list and of SelectedIndex index. Then the information gets passed to textboxes
        /// </summary>
        /// <param name="selectedIndex"></param>
        private void GetStarsInfo(int selectedIndex)
        {
            if (SelectedItems == null || selectedIndex == -1)
            {
                StarInfo_1.Text = "Temperature:  -" + "K     Distance: -" + "pc      Color Index: -" + "    Brightness: -" + "      Luminosity: -" + "       Spectral Type: -" + "       Constellation: -";
                StarInfo_2.Text = "Stars: " + StarSorting.ListIndexes.Count() + "       Selected Star ID: -" + "     Nearby Stars: -";
            }
            else
            {
                if (!SelectedItems.Any())
                {
                    return;
                }

                if (SelectedItems.ElementAt(SelectedIndex).ColorIndex == "")
                {
                    return;
                }

                double colorIndex = Convert.ToDouble(SelectedItems.ElementAt(selectedIndex).ColorIndex, CultureInfo.InvariantCulture);
                double temperature = 4600 * (1 / ((0.92 * colorIndex) + 1.7) + 1 / ((0.92 * colorIndex) + 0.62));
                StarInfo_1.Text = "Temperature:  ~" + Math.Round(temperature) + "K      Distance: " + SelectedItems.ElementAt(selectedIndex).Distance + "pc      Color Index: " + SelectedItems.ElementAt(selectedIndex).ColorIndex +
                                  "      Brightness: " + SelectedItems.ElementAt(selectedIndex).VisualMagnitude + "      Luminosity: " + Math.Round(SelectedItems.ElementAt(selectedIndex).Luminosity, 7) + "      Spectral Type: " +
                                  SelectedItems.ElementAt(selectedIndex).SpectralType + "      Constellation: " + SelectedItems.ElementAt(selectedIndex).Constellation;
                StarInfo_2.Text = "Stars: " + StarSorting.ListIndexes.Count() + "      Selected Star ID: " + SelectedItems.FirstOrDefault().Id + "      Nearby Stars: " + (selectedIndex + 1) + "/" + SelectedItems.Count();
            }
        }

        #endregion
    }
}
