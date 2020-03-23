using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Common.Geometry;
using XbimFloorPlanGenerator.Services.DTO;
using System.Linq;

namespace XbimFloorPlanGenerator.Services
{
    public class BiDimensionalHelper
    {
        public static List<Face> ConvertMesh3DTo2DPolygons(XbimShapeTriangulation vMesh3D, bool isClosed)
        {
            if (vMesh3D == null)
                return new List<Face>();

            var shapeFaces = new List<Face>();
            var vPoint2DCol = new List<Point2D>();

            for (var x = 0; x <= vMesh3D.Vertices.Count - 1; x++)
                vPoint2DCol.Add(new Point2D(vMesh3D.Vertices[x].X, vMesh3D.Vertices[x].Y));

            int vIndex1;
            int vIndex2;
            int vIndex3;
            Triangle vTriangle;
            List<Triangle> vListTriangles = new List<Triangle>();
            Point2D p1, p2, p3;

            foreach (var meshFace in vMesh3D.Faces)
            {
                var face = new Face();
                var vNewIndices = new List<int>();
                var vNewPos = new List<Point2D>();
                for (var x = 0; x <= meshFace.Indices.Count - 1; x += 3)
                {
                    p1 = vPoint2DCol[meshFace.Indices[x]];
                    p2 = vPoint2DCol[meshFace.Indices[x + 1]];
                    p3 = vPoint2DCol[meshFace.Indices[x + 2]];
                    vTriangle = new Triangle(p1, p2, p3);
                    if (!vTriangle.IsValid)
                        continue;
                    vIndex1 = vNewPos.IndexOf(p1);
                    vIndex2 = vNewPos.IndexOf(p2);
                    vIndex3 = vNewPos.IndexOf(p3);
                    if (!vListTriangles.Contains(vTriangle))
                    {
                        vListTriangles.Add(vTriangle);
                        if (vIndex1 != -1)
                            vNewIndices.Add(vIndex1);
                        else
                        {
                            vNewIndices.Add(vNewPos.Count);
                            vNewPos.Add(p1);
                        }

                        if (vIndex2 != -1)
                            vNewIndices.Add(vIndex2);
                        else
                        {
                            vNewIndices.Add(vNewPos.Count);
                            vNewPos.Add(p2);
                        }

                        if (vIndex3 != -1)
                            vNewIndices.Add(vIndex3);
                        else
                        {
                            vNewIndices.Add(vNewPos.Count);
                            vNewPos.Add(p3);
                        }
                    }
                }
                if (vNewIndices.Count == 0 || vNewPos.Count == 0)
                {
                    continue;
                }

                for (int i = 0; i < vNewIndices.Count; i += 3)
                {
                    var shapePolygon = new Polygon();
                    int v1 = vNewIndices[i];
                    int v2 = vNewIndices[i + 1];
                    int v3 = vNewIndices[i + 2];
                    shapePolygon.PolygonVertices.Add(vNewPos[v1]);
                    shapePolygon.PolygonVertices.Add(vNewPos[v2]);
                    shapePolygon.PolygonVertices.Add(vNewPos[v3]);
                    face.Polygons.Add(shapePolygon);
                }
                shapeFaces.Add(face);
            }

            /// lets try this feature

            return shapeFaces;
        }


    }
}
