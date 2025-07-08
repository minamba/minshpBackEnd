using MinshpWebApp.Domain.Dtos;
using AutoMapper;
using MinshpWebApp.Domain.Models;


namespace MinshpWebApp.Domain.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Product, ProductDto>()
               .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
               .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name))
               .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
               .ForMember(x => x.IdCategory, dest => dest.MapFrom(x => x.IdCategory));
        }
    }
}
