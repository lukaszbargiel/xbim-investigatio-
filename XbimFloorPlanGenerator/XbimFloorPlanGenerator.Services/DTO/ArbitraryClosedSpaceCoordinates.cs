using System;
using System.Collections.Generic;
using System.Text;

namespace XbimFloorPlanGenerator.Services.DTO
{
    public class ArbitraryClosedSpaceCoordinates : SpacePosition
    {
        public ArbitraryClosedSpaceCoordinates()
        {
            SweptAreaCoordinates = new List<ArbitraryClosedSpaceSweptAreaCoordinates>();
            SpacePositionType = SpacePositionType.ArbitraryClosedSpace;
        }
        public List<ArbitraryClosedSpaceSweptAreaCoordinates> SweptAreaCoordinates { get; set; }
    }
}
