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
        public static IList<ArbitraryClosedShapeVertices> ConvertMesh3DToPolylineX(XbimShapeTriangulation vMesh3D, bool isClosed)
        {
            if (vMesh3D == null)
                return new List<ArbitraryClosedShapeVertices>();

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
            while(i < uniquePoints.Count)
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
            var currentPoint = edges_h.First();
            var finalPath = new List<ArbitraryClosedShapeVertices>();
            while (edges_h.Count > 0)
            {
                var startPoint = currentPoint.Key;
                var endPoint = currentPoint.Value;
                finalPath.Add(startPoint);
                finalPath.Add(endPoint);
                edges_h.Remove(currentPoint.Key);


                currentPoint.Key
                edges_h.First(e => e.Key == currentPoint || e.Value == currentPoint)
                while (true)
                {
                    var curr = prev;
                    var prev prev;
                }
            }
            return vPoint2DCol;
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
}
