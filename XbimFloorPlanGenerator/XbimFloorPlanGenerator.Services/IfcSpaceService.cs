using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.RepresentationResource;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Data.Interfaces;
using XbimFloorPlanGenerator.Services.DTO;
using XbimFloorPlanGenerator.Services.Interfaces;

namespace XbimFloorPlanGenerator.Services
{
    public class IfcSpaceService : IIfcSpaceService
    {
        private readonly IMapper _mapper;

        public IfcSpaceService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Space CreateSpace(IfcSpace ifcSpace, int floorId)
        {
            var dbSpace = _mapper.Map<Space>(ifcSpace);
            dbSpace.FloorId = floorId;
            dbSpace.SpaceCoordinates = JsonConvert.SerializeObject(GetShapeGeometry(ifcSpace));
            return dbSpace;
        }

        public List<SpacePosition> GetShapeGeometry(IfcSpace ifcSpace)
        {
            if (ifcSpace.ObjectPlacement is IfcLocalPlacement)
            {
                IfcLocalPlacement localPlacement = (IfcLocalPlacement)ifcSpace.ObjectPlacement;
                IfcProductRepresentation representation = (IfcProductRepresentation)ifcSpace.Representation;
                if (representation is IfcProductDefinitionShape)
                {
                    IfcProductDefinitionShape productDefinitionShape =
                            (IfcProductDefinitionShape)representation;
                    return doStuffWithPlacementAndShape(localPlacement, productDefinitionShape);
                }
            }
            return null;
        }
        private static List<SpacePosition> doStuffWithPlacementAndShape(IfcLocalPlacement localPlacement, IfcProductDefinitionShape productDefinitionShape)
        {
            var coordinates = new List<SpacePosition>();
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

        private static SpacePosition doStuffWithExtrudedAreaSolid(IfcLocalPlacement localPlacement, IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            IfcAxis2Placement3D placement = extrudedAreaSolid.Position;
            IfcProfileDef profile = extrudedAreaSolid.SweptArea;
            return doStuffWithProfileObjectPlacementAndLocalPlacement(profile, placement, localPlacement);
        }

        private static SpacePosition doStuffWithProfileObjectPlacementAndLocalPlacement(
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

        private static ArbitraryClosedSpaceCoordinates handleArbitraryClosedProfileDef(
        IfcArbitraryClosedProfileDef profile,
        IfcAxis2Placement3D placement,
        IfcLocalPlacement localPlacement)
        {
            //todo: not sure why we have placement and localplacement here 
            // possibly we will need to perform transformation
            var coordinates = new ArbitraryClosedSpaceCoordinates();
            if (profile.ProfileType == IfcProfileTypeEnum.AREA && profile.OuterCurve is IfcPolyline)
            {
                IfcPolyline polyLine =
                        (IfcPolyline)profile.OuterCurve;
                foreach (IfcCartesianPoint cartesianPoint in polyLine.Points)
                {
                    coordinates.SweptAreaCoordinates.Add(new ArbitraryClosedSpaceSweptAreaCoordinates() { X = cartesianPoint.X, Y = cartesianPoint.Y });
                }
            }
            return coordinates;
        }

        private static RectangleProfileDefSpaceCoordinates handleRectangleProfileDef(
        IfcRectangleProfileDef profile,
        IfcAxis2Placement3D placement,
        IfcLocalPlacement localPlacement)
        {
            
            //todo: not sure why we have placement and localplacement here 
            // possibly we will need to perform transformation
            var coordinates = new RectangleProfileDefSpaceCoordinates();
            if (profile.ProfileType == IfcProfileTypeEnum.AREA )
            {
                coordinates.X = profile.Position.Location.X;
                coordinates.Y = profile.Position.Location.Y;
                coordinates.XDim = profile.XDim;
                coordinates.YDim = profile.YDim;
            }
            return coordinates;
        }
    }
}
