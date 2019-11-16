using AutoMapper;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.SharedBldgElements;
using XbimFloorPlanGenerator.Data.Entities;

namespace XbimFloorPlanGenerator.Services.DTO.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //to dto
            //todo: extract data from UnitsInContext property
            CreateMap<IfcProject, Project>()
                .ForMember(dest => dest.Sites, opt => opt.Ignore())
                .ForMember(dest => dest.IfcId, opt => opt.MapFrom(src => src.GlobalId.Value.ToString()))
                .ForMember(dest => dest.IfcName, opt => opt.MapFrom(src => src.FriendlyName));

            CreateMap<IfcSite, Site>()
                .ForMember(dest => dest.Buildings, opt => opt.Ignore())
                .ForMember(dest => dest.IfcId, opt => opt.MapFrom(src => src.GlobalId.Value.ToString()))
                .ForMember(dest => dest.IfcName, opt => opt.MapFrom(src => src.FriendlyName));
                //.ForMember(dest => dest.FootprintArea, opt => opt.MapFrom(src => src.FootprintArea.Value));

            //todo: extract data from building address
            CreateMap<IfcBuilding, Building>()
                .ForMember(dest => dest.Floors, opt => opt.Ignore())
    .ForMember(dest => dest.IfcId, opt => opt.MapFrom(src => src.GlobalId.ToString()))
    .ForMember(dest => dest.IfcName, opt => opt.MapFrom(src => src.FriendlyName));

            CreateMap<IfcBuildingStorey, Floor>()
                .ForMember(dest => dest.Walls, opt => opt.Ignore())
                .ForMember(dest => dest.Spaces, opt => opt.Ignore())
    .ForMember(dest => dest.IfcId, opt => opt.MapFrom(src => src.GlobalId.Value.ToString()))
    .ForMember(dest => dest.IfcName, opt => opt.MapFrom(src => src.FriendlyName))
    .ForMember(dest => dest.Elevation, opt => opt.MapFrom(src => (double) (src.Elevation ?? 0)));

            CreateMap<IfcWall, Wall>()
                .ForMember(dest => dest.ProductShapes, opt => opt.Ignore())
.ForMember(dest => dest.IfcId, opt => opt.MapFrom(src => src.GlobalId.Value.ToString()))
.ForMember(dest => dest.IfcName, opt => opt.MapFrom(src => src.FriendlyName))
.ForMember(dest => dest.IsExternal, opt => opt.MapFrom(src => (bool) src.IsExternal))
.ForMember(dest => dest.WallSideArea, opt => opt.MapFrom(src => (double) (src.GetWallSideArea ?? 0)));

            CreateMap<IfcSpace, Space>()
.ForMember(dest => dest.IfcId, opt => opt.MapFrom(src => src.GlobalId.Value.ToString()))
.ForMember(dest => dest.IfcName, opt => opt.MapFrom(src => src.FriendlyName))
.ForMember(dest => dest.GrossFloorArea, opt => opt.MapFrom(src => (double)(src.GrossFloorArea ?? 0)))
.ForMember(dest => dest.NetFloorArea, opt => opt.MapFrom(src => (double)(src.NetFloorArea ?? 0 )));
        }
    }
}
