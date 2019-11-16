using System;
using System.Collections.Generic;
using System.Text;

namespace XbimFloorPlanGenerator.Services.DTO
{
    public class RectangleProfileDefSpaceCoordinates : SpacePosition
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double XDim { get; set; }
        public double YDim { get; set; }
        public RectangleProfileDefSpaceCoordinates()
        {
            this.SpacePositionType = SpacePositionType.RectangleProfile;
        }
    }
}
