using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xbim.Common.Geometry;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.ModelGeometry.Scene;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Services.DTO;
using XbimFloorPlanGenerator.Services.Interfaces;

namespace XbimFloorPlanGenerator.Services
{
    public class IfcGeometryService : IIfcGeometryService
    {
        public Xbim3DModelContext context { get; private set; }

        public void InitializeService(IfcStore model)
        {
            context = new Xbim3DModelContext(model);
            context.CreateContext();
        }
        public List<ShapeGeometry> GetShapeGeometry(IfcProduct ifcProduct)
        {
            if (ifcProduct.ObjectPlacement is IfcLocalPlacement)
            {
                IfcLocalPlacement localPlacement = (IfcLocalPlacement)ifcProduct.ObjectPlacement;
                IfcProductRepresentation representation = (IfcProductRepresentation)ifcProduct.Representation;
                if (representation is IfcProductDefinitionShape)
                {
                    IfcProductDefinitionShape productDefinitionShape =
                            (IfcProductDefinitionShape)representation;
                    return doStuffWithPlacementAndShape(localPlacement, productDefinitionShape);
                }
            }
            return null;
        }

        public List<ShapeGeometry> GetShape2DGeometryFromMeshTriangles(IfcProduct product)
        {
            var productShapesGeometry = new List<ShapeGeometry>();

            // https://github.com/xBimTeam/XbimEssentials/issues/121
            var productShape =
                context.ShapeInstancesOf(product)
                    .Where(p => p.RepresentationType != XbimGeometryRepresentationType.OpeningsAndAdditionsExcluded)
                .Distinct();

            if (productShape.Any())
            {
                foreach (var shapeInstance in productShape)
                {
                    var polygonGeometry = new ArbitraryClosedGeometry();
                    var shapeGeometry = context.ShapeGeometry(shapeInstance.ShapeGeometryLabel);
                    if (shapeGeometry == null) continue;

                    byte[] data = ((IXbimShapeGeometryData)shapeGeometry).ShapeData;

                    //If you want to get all the faces and trinagulation use this
                    using (var stream = new MemoryStream(data))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            var mesh = reader.ReadShapeTriangulation();

                            List<XbimFaceTriangulation> faces = mesh.Faces as List<XbimFaceTriangulation>;
                            List<XbimPoint3D> vertices = mesh.Vertices as List<XbimPoint3D>;
                            var transformedMesh = mesh.Transform(shapeInstance.Transformation);
                            var positions = new List<float[]>();
                            var indices = new List<int>();
                            var xxx = BiDimensionalHelper.ConvertMesh3DToPolylineX(transformedMesh, true);

                            transformedMesh.ToPointsWithNormalsAndIndices(out positions, out indices);
                            polygonGeometry.ShapeVertices = xxx;
                            //foreach(var position in positions)
                            //{
                            //    polygonGeometry.ShapeVertices.Add(new ArbitraryClosedShapeVertices()
                            //    {
                            //        X = position[0],
                            //        Y = position[1]
                            //    });
                            //}
                        }
                    }

                    productShapesGeometry.Add(polygonGeometry);

                    //var transformedBoundry = shapeGeometry.BoundingBox.Transform(shapeInstance.Transformation);
                    //shapes.Add(new ProductShape()
                    //{
                    //    BoundingBoxX = transformedBoundry.X,
                    //    BoundingBoxY = transformedBoundry.Y,
                    //    BoundingBoxSizeX = transformedBoundry.SizeX,
                    //    BoundingBoxSizeY = transformedBoundry.SizeY
                    //});
                }
            }

            return productShapesGeometry;
        }
        private static List<ShapeGeometry> doStuffWithPlacementAndShape(IfcLocalPlacement localPlacement, IfcProductDefinitionShape productDefinitionShape)
        {
            var coordinates = new List<ShapeGeometry>();
            foreach (IfcRepresentation representation in productDefinitionShape.Representations)
            {
                if (representation is IfcShapeRepresentation)
                {
                    IfcShapeRepresentation shapeRepresentation = (IfcShapeRepresentation)representation;
                    foreach (IfcRepresentationItem representationItem in shapeRepresentation.Items)
                    {
                        if (representationItem is IfcExtrudedAreaSolid)
                        {
                            IfcExtrudedAreaSolid extrudedAreaSolid =
                                    (IfcExtrudedAreaSolid)representationItem;
                            coordinates.Add(doStuffWithExtrudedAreaSolid(localPlacement, extrudedAreaSolid));
                        }
                    }
                }
            }
            return coordinates;
        }

        private static ShapeGeometry doStuffWithExtrudedAreaSolid(IfcLocalPlacement localPlacement, IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            IfcAxis2Placement3D placement = extrudedAreaSolid.Position;
            IfcProfileDef profile = extrudedAreaSolid.SweptArea;
            return doStuffWithProfileObjectPlacementAndLocalPlacement(profile, placement, localPlacement);
        }

        private static ShapeGeometry doStuffWithProfileObjectPlacementAndLocalPlacement(
                IfcProfileDef profile,
                IfcAxis2Placement3D placement,
                IfcLocalPlacement localPlacement)
        {
            if (profile is IfcArbitraryClosedProfileDef)
            {
                return handleArbitraryClosedProfileDef((IfcArbitraryClosedProfileDef)profile, placement, localPlacement);
            }
            if (profile is IfcRectangleProfileDef)
            {
                return handleRectangleProfileDef((IfcRectangleProfileDef)profile, placement, localPlacement);
            }
            return null;
        }

        private static ArbitraryClosedGeometry handleArbitraryClosedProfileDef(
        IfcArbitraryClosedProfileDef profile,
        IfcAxis2Placement3D placement,
        IfcLocalPlacement localPlacement)
        {
            //todo: not sure why we have placement and localplacement here 
            // possibly we will need to perform transformation
            var coordinates = new ArbitraryClosedGeometry();
            if (profile.ProfileType == IfcProfileTypeEnum.AREA && profile.OuterCurve is IfcPolyline)
            {
                IfcPolyline polyLine =
                        (IfcPolyline)profile.OuterCurve;

                foreach (IfcCartesianPoint cartesianPoint in polyLine.Points)
                {
                    var vertice = new ArbitraryClosedShapeVertices()
                    {
                        X = cartesianPoint.X,
                        Y = cartesianPoint.Y
                    };

                    if (localPlacement.RelativePlacement is IfcAxis2Placement3D)
                    {
                        IfcAxis2Placement3D axisPlacement = (IfcAxis2Placement3D)localPlacement.RelativePlacement;

                        var xAdd = axisPlacement.Location.X + placement.Location.X;
                        var yAdd = axisPlacement.Location.Y + placement.Location.Y;

                        if (axisPlacement.RefDirection == null || axisPlacement.RefDirection.X != 0)
                        {
                            vertice.X = (axisPlacement.RefDirection != null ?
                                axisPlacement.RefDirection.X :
                                1)
                                * cartesianPoint.X +
                                 xAdd;
                            vertice.Y = cartesianPoint.Y + yAdd;
                        }
                        else if (axisPlacement.RefDirection.Y != 0)
                        {
                            vertice.X = cartesianPoint.Y + xAdd;
                            vertice.Y = axisPlacement.RefDirection.Y * cartesianPoint.X + yAdd;
                        }
                        else if (axisPlacement.RefDirection.Z != 0)
                        {
                            vertice.X = cartesianPoint.X + axisPlacement.RefDirection.Z * xAdd;
                            vertice.Y = cartesianPoint.Y + yAdd;
                        }

                    }
                    coordinates.ShapeVertices.Add(vertice);
                }
            }
            return coordinates;
        }

        private static RectangleProfileGeometry handleRectangleProfileDef(
        IfcRectangleProfileDef profile,
        IfcAxis2Placement3D placement,
        IfcLocalPlacement localPlacement)
        {

            //todo: not sure why we have placement and localplacement here 
            // possibly we will need to perform transformation

            var coordinates = new RectangleProfileGeometry();

            if (placement.RefDirection != null)
            {
                //var transformedPosition = profile.Position.ToMatrix3D().Transform(placement.RefDirection.XbimVector3D());
            }

            if (profile.ProfileType != IfcProfileTypeEnum.AREA)
            {
                return coordinates;
            }

            if (localPlacement.RelativePlacement is IfcAxis2Placement3D)
            {
                IfcAxis2Placement3D axisPlacement = (IfcAxis2Placement3D)localPlacement.RelativePlacement;

                var xAdd = axisPlacement.Location.X + placement.Location.X;
                var yAdd = axisPlacement.Location.Y + placement.Location.Y;

                if (axisPlacement.RefDirection == null || axisPlacement.RefDirection.X != 0)
                {
                    coordinates.X = (axisPlacement.RefDirection != null ?
                        axisPlacement.RefDirection.X :
                        1)
                        * profile.Position.Location.X +
                         xAdd;
                    coordinates.Y = profile.Position.Location.Y + yAdd;
                    coordinates.XDim = profile.XDim;
                    coordinates.YDim = profile.YDim;
                }
                else if (axisPlacement.RefDirection.Y != 0)
                {
                    coordinates.X = profile.Position.Location.Y + xAdd;
                    coordinates.Y = axisPlacement.RefDirection.Y * profile.Position.Location.X + yAdd;
                    coordinates.YDim = profile.XDim;
                    coordinates.XDim = profile.YDim;
                }
                else if (axisPlacement.RefDirection.Z != 0)
                {
                    coordinates.X = profile.Position.Location.X + axisPlacement.RefDirection.Z * xAdd;
                    coordinates.Y = profile.Position.Location.Y + yAdd;
                    coordinates.XDim = profile.XDim;
                    coordinates.YDim = profile.YDim;
                }

            }


            return coordinates;
        }
    }
}
