using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    public partial class DeepSkyChartView : Page
    {
        #region Properties

        public List<string> CheckBoxes = new List<string>();
        private Viewport3D TargetViewport { get; set; }
        private Viewport3D OtherViewport { get; set; }
        private float StarScale { get; set; }
        public static double ObjectDistance { get; set; }
        public static IEnumerable<DeepSkyObjectItem> SelectedItems { get; set; }
        public static List<Point3D> SphereCoordinatePoints_Sphere { get; set; }
        public static List<Point3D> SphereCoordinatePoints_Normal { get; set; }
        public static int SelectedIndex { get; set; }

        #endregion

        #region Constructors

        public DeepSkyChartView()
        {
            InitializeComponent();

            if (FileConverter.deepSkyListComplete.Count == 0)
            {
                FileConverter.FileToList("dso");
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

            if (CheckBoxes == null)
            {
                return;
            }
            else
            {
                CreateChart(DeepSkyObjectSorting.SortItems(CheckBoxes));
            }

            TransformChart();
            GetStarsInfo(-1);
        }

        #endregion

        #region Sliders

        private void StarSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).Value = Math.Round(((Slider)sender).Value, 2);
        }

        #endregion

        #region Checkboxes

        private void Type_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxes.Add((string)((CheckBox)sender).Content);
        }
        private void Type_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBoxes.Remove((string)((CheckBox)sender).Content);
        }

        #endregion

        #region Buttons

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxes.Count == 0)
            {
                return;
            }

            CreateChart(DeepSkyObjectSorting.SortItems(CheckBoxes));
            GetStarsInfo(-1);
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
        }
        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            FileConverter.FileToList("dso");
            DeepSkyObjectSorting.UpdateObjectListBase();
        }

        #endregion

        #region Textboxes

        private void StarDistanceText_TextChanged(object sender, TextChangedEventArgs e)
        {
            ObjectDistance = Convert.ToDouble(StarDistanceText.Text, CultureInfo.InvariantCulture);
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

        public void CreateChart(IEnumerable<DeepSkyObjectItem> list)
        {
            // Clearing and setting viewports
            if (PerspectiveView.IsChecked == true)
            {
                TargetViewport = ChartViewPerspective;
                OtherViewport = ChartViewOrthographic;

                TargetViewport.Children.Clear();
                TargetViewport.Children.Add(new ModelVisual3D() { Content = new AmbientLight(Colors.White) });
                OtherViewport.Children.Clear();
                OtherViewport.Children.Add(new ModelVisual3D() { Content = new AmbientLight(Colors.White) });

                InitializeSelectionRect(TargetViewport);

                StarScale = 0.25f;
            }
            else
            {
                TargetViewport = ChartViewOrthographic;
                OtherViewport = ChartViewPerspective;

                TargetViewport.Children.Clear();
                TargetViewport.Children.Add(new ModelVisual3D() { Content = new AmbientLight(Colors.White) });
                OtherViewport.Children.Clear();
                OtherViewport.Children.Add(new ModelVisual3D() { Content = new AmbientLight(Colors.White) });

                InitializeSelectionRect(TargetViewport);

                StarScale = 1.05f;
            }

            // 1. set the scatter plot size
            m_3dChart = new ScatterChart3D();
            m_3dChart.SetDataNo(list.Count());

            // 2. set the properties of each dot
            int itemNumber = 0;

            foreach (DeepSkyObjectItem item in list)
            {

                ScatterPlotItem chartObject = new ScatterPlotItem();

                chartObject.w = 0.025f;
                chartObject.h = 0.025f;

                chartObject.x = Convert.ToSingle(item.PositionsOnSphere.X, CultureInfo.InvariantCulture);
                chartObject.y = Convert.ToSingle(item.PositionsOnSphere.Y, CultureInfo.InvariantCulture);
                chartObject.z = Convert.ToSingle(item.PositionsOnSphere.Z, CultureInfo.InvariantCulture);

                chartObject.shape = (int)Chart3D.SHAPE.BAR;
                chartObject.color = Colors.White;

                ((ScatterChart3D)m_3dChart).SetVertex(itemNumber, chartObject);

                itemNumber++;
            }

            // get Mesh3D array from scatter plot
            ArrayList meshs = ((ScatterChart3D)m_3dChart).GetMeshes();

            // show 3D scatter plot in Viewport3d
            Model3D model3d = new Model3D();
            m_nChartModelIndex = model3d.UpdateModel(meshs, null, m_nChartModelIndex, TargetViewport, "dso", itemNumber);

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
            Point pt = args.GetPosition(TargetViewport);
            if (args.ChangedButton == MouseButton.Left)         // rotate or drag 3d model
            {
                m_transformMatrix.OnLBtnDown(pt);
            }
            else if (args.ChangedButton == MouseButton.Right)   // select rect
            {
                m_selectRect.OnMouseDown(pt, TargetViewport, m_nRectModelIndex);
            }
        }

        public void OnViewportMouseMove(object sender, System.Windows.Input.MouseEventArgs args)
        {
            Point pt = args.GetPosition(TargetViewport);

            if (args.LeftButton == MouseButtonState.Pressed)                // rotate or drag 3d model
            {
                m_transformMatrix.OnMouseMove(pt, TargetViewport);

                TransformChart();
            }
            else if (args.RightButton == MouseButtonState.Pressed)          // select rect
            {
                m_selectRect.OnMouseMove(pt, TargetViewport, m_nRectModelIndex);
            }

        }

        public void OnViewportMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs args)
        {
            Point pt = args.GetPosition(TargetViewport);
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
                MeshGeometry3D meshGeometry = Model3D.GetGeometry(TargetViewport, m_nChartModelIndex);
                if (meshGeometry == null)
                {
                    return;
                }

                // 2. set selection in 3d chart
                m_3dChart.Select(m_selectRect, m_transformMatrix, TargetViewport);

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

            ModelVisual3D visual3d = (ModelVisual3D)(TargetViewport.Children[m_nChartModelIndex]);
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
        // Helper functions that only serve their purpose in this specific case, thus no need to put them in a different file

        /// <summary>
        /// Gets info about selected elements by comparing xyz coordinates of selected objects to objects in a file
        /// </summary>
        /// <param name="meshGeometry"></param>
        /// 
        private void GetSelectedElements(MeshGeometry3D meshGeometry)
        {
            if (ScatterChart3D.SelectedItems.Count == 0)
            {
                return;
            }

            double range = 0;

            if (ObjectDistance > 80)
            {
                range = 0.05;
            }
            else if (ObjectDistance > 60)
            {
                range = 0.045;
            }
            else if (ObjectDistance > 40)
            {
                range = 0.040;
            }
            else if (ObjectDistance > 20)
            {
                range = 0.035;
            }
            else if (ObjectDistance > 0)
            {
                range = 0.030;
            }

            double desiredX_1 = ScatterChart3D.SelectedItems[0].X - range;
            double desiredY_1 = ScatterChart3D.SelectedItems[0].Y - range;
            double desiredZ_1 = ScatterChart3D.SelectedItems[0].Z - range;

            double desiredX_2 = ScatterChart3D.SelectedItems[0].X + range;
            double desiredY_2 = ScatterChart3D.SelectedItems[0].Y + range;
            double desiredZ_2 = ScatterChart3D.SelectedItems[0].Z + range;

            IEnumerable<DeepSkyObjectItem> listX_1 = DeepSkyObjectSorting.ListIndexes.Where(find => Convert.ToDouble(find.PositionsOnSphere.X, CultureInfo.InvariantCulture) > desiredX_1);
            IEnumerable<DeepSkyObjectItem> listY_1 = DeepSkyObjectSorting.ListIndexes.Where(find => Convert.ToDouble(find.PositionsOnSphere.Y, CultureInfo.InvariantCulture) > desiredY_1);
            IEnumerable<DeepSkyObjectItem> listZ_1 = DeepSkyObjectSorting.ListIndexes.Where(find => Convert.ToDouble(find.PositionsOnSphere.Z, CultureInfo.InvariantCulture) > desiredZ_1);

            IEnumerable<DeepSkyObjectItem> listX_2 = DeepSkyObjectSorting.ListIndexes.Where(find => Convert.ToDouble(find.PositionsOnSphere.X, CultureInfo.InvariantCulture) < desiredX_2);
            IEnumerable<DeepSkyObjectItem> listY_2 = DeepSkyObjectSorting.ListIndexes.Where(find => Convert.ToDouble(find.PositionsOnSphere.Y, CultureInfo.InvariantCulture) < desiredY_2);
            IEnumerable<DeepSkyObjectItem> listZ_2 = DeepSkyObjectSorting.ListIndexes.Where(find => Convert.ToDouble(find.PositionsOnSphere.Z, CultureInfo.InvariantCulture) < desiredZ_2);

            IEnumerable<DeepSkyObjectItem> listX = listX_1.Intersect(listX_2);
            IEnumerable<DeepSkyObjectItem> listY = listY_1.Intersect(listY_2);
            IEnumerable<DeepSkyObjectItem> listZ = listZ_1.Intersect(listZ_2);

            IEnumerable<DeepSkyObjectItem> listFinal = new List<DeepSkyObjectItem>();

            if (listX.Any())
            {
                if (!listFinal.Any())
                {
                    listFinal = FileConverter.deepSkyListComplete;
                }

                listFinal = listFinal.Intersect(listX);
            }
            if (listY.Any())
            {
                if (!listFinal.Any())
                {
                    listFinal = FileConverter.deepSkyListComplete;
                }

                listFinal = listFinal.Intersect(listY);
            }
            if (listZ.Any())
            {
                if (!listFinal.Any())
                {
                    listFinal = FileConverter.deepSkyListComplete;
                }

                listFinal = listFinal.Intersect(listZ);
            }
            int amount = listFinal.Count();
            SelectedItems = listFinal;

            SelectedIndex = 0;

            GetStarsInfo(SelectedIndex);
            if (SelectedItems.Count() - 1 > 0)
            {
                NextButton.IsEnabled = true;
            }
            else
            {
                NextButton.IsEnabled = false;
            }
            //if (SelectedItems.Count() > 0)
            //DetailsButton.IsEnabled = true;
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
            m_nRectModelIndex = model3d_.UpdateModel(meshs_, null, m_nRectModelIndex, viewport, "dso", 0);
        }

        /// <summary>
        /// Gets info about a selected object from SelectedItems list and of SelectedIndex index. Then the information gets passed to textboxes
        /// </summary>
        /// <param name="selectedIndex"></param>
        private void GetStarsInfo(int selectedIndex)
        {
            if (SelectedItems == null || selectedIndex == -1)
            {
                StarInfo_1.Text = "Category: " + "     Id: " + "      Name: " + "      Type:  " + "      Constellation: ";
                StarInfo_2.Text = "Deep sky objects: " + DeepSkyObjectSorting.ListIndexes.Count() + "       Selected DSO ID: " + "     Nearby objects: ";

            }
            else
            {
                string cat;

                if (SelectedItems.ElementAt(selectedIndex).Cat1 != null)
                {
                    cat = SelectedItems.ElementAt(selectedIndex).Cat1;
                }
                else
                {
                    cat = SelectedItems.ElementAt(selectedIndex).Cat2;
                }

                StarInfo_1.Text = "     Id: " + cat + "      Name: " + SelectedItems.ElementAt(selectedIndex).Name + "    Type: " + SelectedItems.ElementAt(selectedIndex).Type +
                    "      Constellation: " + SelectedItems.ElementAt(selectedIndex).Constellation;
                StarInfo_2.Text = "Deep sky objects: " + DeepSkyObjectSorting.ListIndexes.Count() + "       Selected DSO ID: " + SelectedItems.ElementAt(selectedIndex).Id +
                    "     Nearby objects: " + (SelectedItems.Count() - 1);
            }
        }
        #endregion
    }
}
