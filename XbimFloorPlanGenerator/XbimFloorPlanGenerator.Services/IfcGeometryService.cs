﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.Common.Geometry;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc;
using Xbim.Ifc2x3.Kernel;
using Xbim.ModelGeometry.Scene;
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

        public List<PolygonSet> GetShape2DGeometryFromMeshTriangles(IfcProduct product)
        {
            var productPolygonSets = new List<PolygonSet>();

            // https://github.com/xBimTeam/XbimEssentials/issues/121
            var productShape =
                context.ShapeInstancesOf(product)
                    .Where(p => p.RepresentationType != XbimGeometryRepresentationType.OpeningsAndAdditionsExcluded)
                .Distinct();

            if (productShape.Any())
            {
                foreach (var shapeInstance in productShape)
                {
                    var shapePolygonSet = new PolygonSet();
                    //var polygonGeometry = new List<Polygon>();
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

                            var meshPolygons = BiDimensionalHelper.ConvertMesh3DTo2DPolygons(transformedMesh, true);
                            shapePolygonSet.Polygons = meshPolygons;

                            productPolygonSets.Add(shapePolygonSet);
                        }
                    }

                }
            }

            return productPolygonSets;
        }



    }
}
