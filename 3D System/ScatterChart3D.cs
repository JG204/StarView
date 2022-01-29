//------------------------------------------------------------------
// (c) Copywrite Jianzhong Zhang
// This code is under The Code Project Open License
// Please read the attached license document before using this class
//------------------------------------------------------------------

// class of 3d scatter plot .
// version 0.1

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace StarView
{
    class ScatterChart3D : Chart3D
    {
        public static List<Point3D> SelectedItems { get; set; }

        public ScatterPlotItem Get(int n)
        {
            return (ScatterPlotItem)m_vertices[n];
        }

        public void SetVertex(int n, ScatterPlotItem value)
        {
            m_vertices[n] = value;
        }

        // convert the 3D scatter plot into a array of Mesh3D object
        public ArrayList GetMeshes()
        {
            int nDotNo = GetDataNo();
            if (nDotNo == 0)
            {
                return null;
            }

            ArrayList meshs = new ArrayList();

            int nVertIndex = 0;
            for (int i = 0; i < nDotNo; i++)
            {
                ScatterPlotItem plotItem = Get(i);
                int nType = plotItem.shape % Chart3D.SHAPE_NO;
                float w = plotItem.w;
                //float h = plotItem.h;
                Mesh3D dot;
                if (nType == 0)
                {
                    dot = new Bar3D(0, 0, 0, w, w, w);
                }
                else
                {
                    dot = new Ellipse3D(w / 1.5, w / 1.5, w / 1.5, 7);
                }

                m_vertices[i].nMinI = nVertIndex;
                nVertIndex += dot.GetVertexNo();
                m_vertices[i].nMaxI = nVertIndex - 1;

                TransformMatrix.Transform(dot, new Point3D(plotItem.x, plotItem.y, plotItem.z), 0, 0);
                dot.SetColor(plotItem.color);
                meshs.Add(dot);
            }
            AddAxesMeshes(meshs);

            return meshs;
        }
        // selection
        public override void Select(ViewportRect rect, TransformMatrix matrix, Viewport3D viewport3d)
        {
            int nDotNo = GetDataNo();
            if (nDotNo == 0)
            {
                return;
            }

            double xMin = rect.XMin();
            double xMax = rect.XMax();
            double yMin = rect.YMin();
            double yMax = rect.YMax();

            SelectedItems = new List<Point3D>();

            for (int i = 0; i < nDotNo; i++)
            {
                ScatterPlotItem plotItem = Get(i);
                Point pt = matrix.VertexToViewportPt(new Point3D(plotItem.x, plotItem.y, plotItem.z),
                    viewport3d);

                if ((pt.X > xMin) && (pt.X < xMax) && (pt.Y > yMin) && (pt.Y < yMax))
                {
                    m_vertices[i].selected = true;
                    SelectedItems.Add(new Point3D(plotItem.x, plotItem.y, plotItem.z));
                }
                else
                {
                    m_vertices[i].selected = false;
                }
            }
        }

        // highlight the selection
        public override void HighlightSelection(System.Windows.Media.Media3D.MeshGeometry3D meshGeometry, System.Windows.Media.Color selectColor)
        {
            int nDotNo = GetDataNo();
            if (nDotNo == 0)
            {
                return;
            }

            Point mapPt;
            for (int i = 0; i < nDotNo; i++)
            {
                if (m_vertices[i].selected)
                {
                    mapPt = TextureMapping.GetMappingPosition(selectColor, false);
                }
                else
                {
                    mapPt = TextureMapping.GetMappingPosition(m_vertices[i].color, false);
                }
                int nMin = m_vertices[i].nMinI;
                int nMax = m_vertices[i].nMaxI;
                for (int j = nMin; j <= nMax; j++)
                {
                    meshGeometry.TextureCoordinates[j] = mapPt;
                }
            }
        }

    }
}
