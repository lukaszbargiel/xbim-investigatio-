using System;
using System.Collections.Generic;
using System.Text;

namespace XbimFloorPlanGenerator.Services.DTO
{
    public enum SpacePositionType
    {
        ArbitraryClosedSpace = 0,
        RectangleProfile = 1
    }
    public abstract class SpacePosition
    {
        public SpacePositionType SpacePositionType { get; set; }
    }
}
