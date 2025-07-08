using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Domain.Dtos;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Mapper
{
    public class MapperProfile : Profile
    {

        public MapperProfile()
        {
            CreateMap<ProductRequest, Product>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
              .ForMember(x => x.IdCategory, dest => dest.MapFrom(x => x.IdCategory));
        }
    }
}
