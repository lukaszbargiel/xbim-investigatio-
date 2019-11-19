﻿using System.Collections.Generic;
using Xbim.Ifc;
using Xbim.Ifc2x3.Kernel;
using XbimFloorPlanGenerator.Data.Entities;
using XbimFloorPlanGenerator.Services.DTO;

namespace XbimFloorPlanGenerator.Services.Interfaces
{
    public interface IIfcGeometryService
    {
        void InitializeService(IfcStore model);
        List<ShapeGeometry> GetShapeGeometry(IfcProduct ifcProduct);
        List<ShapeGeometry> GetShape2DGeometryFromMeshTriangles(IfcProduct product);
    }
}