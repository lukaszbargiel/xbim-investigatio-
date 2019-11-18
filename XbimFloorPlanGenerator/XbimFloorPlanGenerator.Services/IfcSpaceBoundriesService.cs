using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.TopologyResource;
using XbimFloorPlanGenerator.Services.Interfaces;

namespace XbimFloorPlanGenerator.Services
{
    class SpaceBoundry
    {
        public bool IsPhysical { get; set; }

        public bool IsExternal { get; set; }
    }
    public class IfcSpaceBoundriesService : IIfcSpaceBoundriesService
    {
        public void ExtractSpaceBoundries(IfcSpace space)
        {
            foreach (var bound in space.BoundedBy)
            {
                var spaceBound = new SpaceBoundry();
                spaceBound.IsPhysical = bound.PhysicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.PHYSICAL;
                spaceBound.IsExternal = bound.InternalOrExternalBoundary == IfcInternalOrExternalEnum.EXTERNAL;
                if (bound.RelatedBuildingElement == null)
                {
                    continue;
                }
                var buildingElement = bound.RelatedBuildingElement;
                var boundLabel = $"{buildingElement.Name} - {buildingElement.Description} - {buildingElement.GlobalId}";
                var geometry = bound.ConnectionGeometry as IfcConnectionSurfaceGeometry;
                IfcSurfaceOrFaceSurface surface = geometry.SurfaceOnRelatingElement as IfcSurfaceOrFaceSurface;

                if (surface is IfcSurfaceOfLinearExtrusion)
                {

                }
                else if (surface is IfcCurveBoundedPlane)
                {
                    var curvedBoundedSurface = surface as IfcCurveBoundedPlane;
                    var boundShape = curvedBoundedSurface.OuterBoundary as Xbim.Ifc2x3.GeometryResource.IfcPolyline;
                    var vertices = boundShape.Points.Select(p => p.Coordinates).ToList();
                    decimal xFactor = 0;
                    decimal yFactor = 0;

                    for (var i = 0; i < vertices.Count - 1; i++)
                    {
                        var x1 = Convert.ToDecimal(vertices[i][0].Value);
                        var y1 = Convert.ToDecimal(vertices[i][1].Value);
                        var x2 = Convert.ToDecimal(vertices[(i + 1)][0].Value);
                        var y2 = Convert.ToDecimal(vertices[(i + 1)][1].Value);

                        xFactor += x1 * y2;
                        yFactor += y1 * x2;
                    }
                    var polygonArea = (xFactor - yFactor) / 2; // result in square milimiters
                    var polygonAreaInSquareMeters = polygonArea / 1000000;

                    // or Trace.Listeners.Add(new ConsoleTraceListener());

                    //INV 3 - Gipsvägg - 3G4e1R8Jn17QL8Z0aCQCCe - 9.342 -> this should be 13.148m2
                    //Trace.WriteLine(boundLabel + " - " + polygonAreaInSquareMeters.ToString());

                }
                else if (surface is IfcSurface)
                {

                }
                else if (surface is IfcFaceSurface)
                {

                }
                else if (surface is IfcFaceBasedSurfaceModel)
                {

                }
            }

            //return string.Empty;
        }
    }
}

