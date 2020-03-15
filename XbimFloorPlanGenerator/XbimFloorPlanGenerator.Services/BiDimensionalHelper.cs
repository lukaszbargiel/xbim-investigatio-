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
        public static List<Polygon> ConvertMesh3DTo2DPolygons(XbimShapeTriangulation vMesh3D, bool isClosed)
        {
            if (vMesh3D == null)
                return new List<Polygon>();

            var finalPolygons = new List<Polygon>();
            var vPoint2DCol = new List<Point2D>();
            var positions = new List<float[]>();
            var indices = new List<int>();
            vMesh3D.ToPointsWithNormalsAndIndices(out positions, out indices);

            //var sameSurfaceIndices = new Dictionary<double, List<int>>();

            //for (var x = 0; x <= indices.Count - 1; x += 3)
            //{
            //    var pos1 = positions[indices[x]];
            //    var pos2 = positions[indices[x + 1]];
            //    var pos3 = positions[indices[x + 2]];
            //    if (pos1[2] == pos2[2] && pos2[2] == pos3[2])
            //    {
            //        if (!sameSurfaceIndices.ContainsKey(pos1[2]))
            //        {
            //            sameSurfaceIndices.Add(pos1[2], new List<int>());
            //        }
            //        sameSurfaceIndices[pos1[2]].Add(indices[x]);
            //        sameSurfaceIndices[pos1[2]].Add(indices[x + 1]);
            //        sameSurfaceIndices[pos1[2]].Add(indices[x + 2]);
            //    }
            //}

            for (var x = 0; x <= vMesh3D.Vertices.Count - 1; x++)
                vPoint2DCol.Add(new Point2D(vMesh3D.Vertices[x].X, vMesh3D.Vertices[x].Y));

            int vIndex1;
            int vIndex2;
            int vIndex3;
            Triangle vTriangle;
            List<Triangle> vListTriangles = new List<Triangle>();
            Point2D p1, p2, p3;

            //if (!sameSurfaceIndices.Any())
            //    return finalPolygons;
            foreach(var face in vMesh3D.Faces)
            {
                var vNewIndices = new List<int>();
                var vNewPos = new List<Point2D>();                
                for (var x = 0; x <= face.Indices.Count - 1; x += 3)
                {
                    p1 = vPoint2DCol[face.Indices[x]];
                    p2 = vPoint2DCol[face.Indices[x + 1]];
                    p3 = vPoint2DCol[face.Indices[x + 2]];
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
                if(vNewIndices.Count == 0 || vNewPos.Count == 0)
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
                    finalPolygons.Add(shapePolygon);
                }
                //var boundaryPath = EdgeHelpers.GetEdges(vNewIndices.ToArray()).FindBoundary().SortEdges();

                //foreach (var pathStep in boundaryPath)
                //{
                //    shapePolygon.PolygonVertices.Add(vNewPos[pathStep.v1]);
                //}
                //shapePolygon.PolygonVertices.Add(vNewPos[boundaryPath[boundaryPath.Count - 1].v2]);
                
            }

            /// lets try this feature

            return finalPolygons;
        }
        public void SimplifyPoints(ref List<Point2D> vPoints)
        {
            float angulo;
            List<Point2D> listRemove = new List<Point2D>();
            bool doRecursion = false;
            for (int i = 0; i <= vPoints.Count - 3; i += 2)
            {
                angulo = (float)angleByVectors(
                    new Point2D(vPoints[i].X, vPoints[i].Y),
                    new Point2D(vPoints[i + 1].X, vPoints[i + 1].Y),
                    new Point2D(vPoints[i + 2].X, vPoints[i + 2].Y)
                    );
                if (angulo >= -0.05 & angulo <= 0.05)
                    listRemove.Add(vPoints[i + 1]);
            }

            for (int i = 0; i <= listRemove.Count - 1; i++)
            {
                vPoints.Remove(listRemove[i]);
                doRecursion = true;
            }

            if (doRecursion)
                SimplifyPoints(ref vPoints);
        }

        public static double angleByVectors(Point2D ponto1, Point2D ponto2, Point2D ponto3)
        {
            Point2D vet1;
            Point2D vet2;
            float prodEscalar;
            float multNorma;
            vet1 = new Point2D(ponto2.X - ponto1.X, ponto2.Y - ponto1.Y);
            vet2 = new Point2D(ponto3.X - ponto2.X, ponto3.Y - ponto2.Y);
            prodEscalar = (float)(vet1.X * vet2.X + vet1.Y * vet2.Y);

            multNorma = (float)(
                Math.Sqrt(Math.Pow(vet1.X, 2) + Math.Pow(vet1.Y, 2)) *
                Math.Sqrt(Math.Pow(vet2.X, 2) + Math.Pow(vet2.Y, 2))
                );

            return Math.Acos(prodEscalar / multNorma);
        }
    }

    public static class EdgeHelpers
    {
        public struct Edge
        {
            public int v1;
            public int v2;
            public int triangleIndex;
            public Edge(int aV1, int aV2, int aIndex)
            {
                v1 = aV1;
                v2 = aV2;
                triangleIndex = aIndex;
            }
        }

        public static List<Edge> GetEdges(int[] aIndices)
        {
            List<Edge> result = new List<Edge>();
            for (int i = 0; i < aIndices.Length; i += 3)
            {
                int v1 = aIndices[i];
                int v2 = aIndices[i + 1];
                int v3 = aIndices[i + 2];
                result.Add(new Edge(v1, v2, i));
                result.Add(new Edge(v2, v3, i));
                result.Add(new Edge(v3, v1, i));
            }
            return result;
        }

        public static List<Edge> FindBoundary(this List<Edge> aEdges)
        {
            List<Edge> result = new List<Edge>(aEdges);
            for (int i = result.Count - 1; i > 0; i--)
            {
                for (int n = i - 1; n >= 0; n--)
                {
                    if (result[i].v1 == result[n].v2 && result[i].v2 == result[n].v1)
                    {
                        // shared edge so remove both
                        result.RemoveAt(i);
                        result.RemoveAt(n);
                        i--;
                        break;
                    }
                }
            }
            return result;
        }
        public static List<Edge> SortEdges(this List<Edge> aEdges)
        {
            List<Edge> result = new List<Edge>(aEdges);
            for (int i = 0; i < result.Count - 2; i++)
            {
                Edge E = result[i];
                for (int n = i + 1; n < result.Count; n++)
                {
                    Edge a = result[n];
                    if (E.v2 == a.v1)
                    {
                        // in this case they are already in order so just continoue with the next one
                        if (n == i + 1)
                            break;
                        // if we found a match, swap them with the next one after "i"
                        result[n] = result[i + 1];
                        result[i + 1] = a;
                        break;
                    }
                }
            }
            return result;
        }
    }
}
