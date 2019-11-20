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
        public static List<ArbitraryClosedShapeVertices> ConvertMesh3DToPolylineX(XbimShapeTriangulation vMesh3D, bool isClosed)
        {
            if (vMesh3D == null)
                return new List<ArbitraryClosedShapeVertices>();

            var response = new List<ArbitraryClosedShapeVertices>();
            var vPoint2DCol = new List<ArbitraryClosedShapeVertices>();
            var positions = new List<float[]>();
            var indices = new List<int>();
            vMesh3D.ToPointsWithNormalsAndIndices(out positions, out indices);

            for (var x = 0; x <= positions.Count - 1; x++)
                vPoint2DCol.Add(new ArbitraryClosedShapeVertices(positions[x][0], positions[x][1]));

            var vNewIndices = new List<int>();
            var vNewPos = new List<ArbitraryClosedShapeVertices>();

            int vIndex1;
            int vIndex2;
            int vIndex3;
            Triangle vTriangle;
            List<Triangle> vListTriangles = new List<Triangle>();
            ArbitraryClosedShapeVertices p1, p2, p3;

            for (var x = 0; x <= indices.Count - 1; x += 3)
            {
                p1 = vPoint2DCol[indices[x]];
                p2 = vPoint2DCol[indices[x + 1]];
                p3 = vPoint2DCol[indices[x + 2]];
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
            /// lets try this feature

            var boundaryPath = EdgeHelpers.GetEdges(vNewIndices.ToArray()).FindBoundary().SortEdges();
            
            foreach (var pathStep in boundaryPath)
            {
                response.Add(vNewPos[pathStep.v1]);
            }
            return response;
            var uniquePoints = new List<ArbitraryClosedShapeVertices>();
            foreach (var indice in vNewIndices)
            {
                var uniquePt = vNewPos[indice];
                if (!uniquePoints.Contains(uniquePt))
                {
                    uniquePoints.Add(uniquePt);
                }
                else
                {
                    uniquePoints.Remove(uniquePt);
                }

            }
            var newPosSortedByX = uniquePoints.OrderBy(s => s.X).ToList();
            var newPosSortedByY = uniquePoints.OrderBy(s => s.Y).ToList();

            var i = 0;
            var edges_h = new Dictionary<ArbitraryClosedShapeVertices, ArbitraryClosedShapeVertices>();
            while (i < uniquePoints.Count)
            {
                var currentY = newPosSortedByY[i].Y;
                while (i < uniquePoints.Count && newPosSortedByY[i].Y == currentY)
                {
                    edges_h[newPosSortedByY[i]] = newPosSortedByY[i + 1];
                    edges_h[newPosSortedByY[i + 1]] = newPosSortedByY[i];
                    i += 2;
                }
            }

            i = 0;
            var edges_v = new Dictionary<ArbitraryClosedShapeVertices, ArbitraryClosedShapeVertices>();

            while (i < uniquePoints.Count)
            {
                var currentX = newPosSortedByX[i].X;
                while (i < uniquePoints.Count && newPosSortedByX[i].X == currentX)
                {
                    edges_v[newPosSortedByX[i]] = newPosSortedByX[i + 1];
                    edges_v[newPosSortedByX[i + 1]] = newPosSortedByX[i];
                    i += 2;
                }
            }

            // https://stackoverflow.com/questions/13746284/merging-multiple-adjacent-rectangles-into-one-polygon
            var result = new[] { edges_v, edges_h }.SelectMany(dict => dict)
                         .ToDictionary(pair => pair.Key, pair => pair.Value);
            var currentEdge = result.First();
            var previousEdge = result.First();
            var finalPath = new List<ArbitraryClosedShapeVertices>();


            //            while (edges_h.Count > 0)
            //            {
            //                finalPath.Insert(0, currentEdge.Key);
            //                //while (true)
            //                //{
            //                //    currentEdge
            //                //}
            //                var startPoint = currentPoint.Key;
            //                var endPoint = currentPoint.Value;
            //                finalPath.Add(startPoint);
            //                finalPath.Add(endPoint);
            //                result.Remove(currentPoint.Key);

            //                currentPoint = result.First(e => e.Key == endPoint);
            //            }


            //    while True:
            //        curr, e = polygon[-1]
            //        if e == 0:
            //            next_vertex = edges_v.pop(curr)
            //            polygon.append((next_vertex, 1))
            //        else:
            //            next_vertex = edges_h.pop(curr)
            //            polygon.append((next_vertex, 0))
            //        if polygon[-1] == polygon[0]:
            //            # Closed polygon
            //            polygon.pop()
            //            break
            //    # Remove implementation-markers from the polygon.
            //    poly = [point for point, _ in polygon]
            //    for vertex in poly:
            //        if vertex in edges_h: edges_h.pop(vertex)
            //        if vertex in edges_v: edges_v.pop(vertex)

            //    p.append(poly)


            //for poly in p:
            //    print poly
            //return vPoint2DCol;
            //return Get2DOutline(vNewIndices, vNewPos, isClosed);
        }


        //private List<ArbitraryClosedShapeVertices> Get2DOutline(List<int> vIndices, List<ArbitraryClosedShapeVertices> vPositions, bool isClosed)
        //{
        //    PathGeometry vPathResult = new PathGeometry();
        //    Point3D p1, p2, p3;
        //    for (var i = 0; i <= vIndices.Count - 1; i += 3)
        //    {
        //        p1 = vPositions[vIndices[i]];
        //        p2 = vPositions[vIndices[i + 1]];
        //        p3 = vPositions[vIndices[i + 2]];
        //        StreamGeometry geo = new StreamGeometry();
        //        using (StreamGeometryContext ctx = geo.Open())
        //        {
        //            ctx.BeginFigure(new System.Windows.Point(p1.X, p1.Y), true, true);
        //            ctx.LineTo(new System.Windows.Point(p2.X, p2.Y), true, true);
        //            ctx.LineTo(new System.Windows.Point(p3.X, p3.Y), true, true);
        //            ctx.LineTo(new System.Windows.Point(p1.X, p1.Y), true, true);
        //            ctx.Close();
        //        }

        //        vPathResult = System.Windows.Media.Geometry.Combine(vPathResult, geo, GeometryCombineMode.Union, null /* TODO Change to default(_) if this is not a reference type */);
        //    }

        //    vPathResult = vPathResult.GetWidenedPathGeometry(new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 2E-06), 1E-06, System.Windows.Media.ToleranceType.Absolute);

        //    Point3DCollection vPoly = new Point3DCollection();
        //    System.Windows.Point p;
        //    System.Windows.Point tg;
        //    for (var i = 600; i <= 1200 - 1; i++)
        //    {
        //        vPathResult.GetPointAtFractionLength(i / 1200, out p, out tg);
        //        vPoly.Add(new Point3D(p.X, p.Y, 0));
        //    }

        //    vPoly.Add(vPoly[0]);
        //    SimplifyPoints(ref vPoly);
        //    return vPoly;
        //}

        public void SimplifyPoints(ref List<ArbitraryClosedShapeVertices> vPoints)
        {
            float angulo;
            List<ArbitraryClosedShapeVertices> listRemove = new List<ArbitraryClosedShapeVertices>();
            bool doRecursion = false;
            for (int i = 0; i <= vPoints.Count - 3; i += 2)
            {
                angulo = (float)angleByVectors(
                    new ArbitraryClosedShapeVertices(vPoints[i].X, vPoints[i].Y),
                    new ArbitraryClosedShapeVertices(vPoints[i + 1].X, vPoints[i + 1].Y),
                    new ArbitraryClosedShapeVertices(vPoints[i + 2].X, vPoints[i + 2].Y)
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

        public static double angleByVectors(ArbitraryClosedShapeVertices ponto1, ArbitraryClosedShapeVertices ponto2, ArbitraryClosedShapeVertices ponto3)
        {
            ArbitraryClosedShapeVertices vet1;
            ArbitraryClosedShapeVertices vet2;
            float prodEscalar;
            float multNorma;
            vet1 = new ArbitraryClosedShapeVertices(ponto2.X - ponto1.X, ponto2.Y - ponto1.Y);
            vet2 = new ArbitraryClosedShapeVertices(ponto3.X - ponto2.X, ponto3.Y - ponto2.Y);
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
