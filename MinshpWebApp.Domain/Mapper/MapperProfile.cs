using MinshpWebApp.Domain.Dtos;
using AutoMapper;
using MinshpWebApp.Domain.Models;
using System.Threading.Tasks.Dataflow;


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
               .ForMember(x => x.Price, dest => dest.MapFrom(x => x.Price))
               .ForMember(x => x.IdCategory, dest => dest.MapFrom(x => x.IdCategory));


            CreateMap<Promotion, PromotionDto>()
             .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
             .ForMember(x => x.StartDate, dest => dest.MapFrom(x => x.StartDate))
             .ForMember(x => x.EndDate, dest => dest.MapFrom(x => x.EndDate))
             .ForMember(x => x.Purcentage, dest => dest.MapFrom(x => x.Purcentage))
             .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct));


            CreateMap<Image, ImageDto>()
               .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
               .ForMember(x => x.Url, dest => dest.MapFrom(x => x.Url))
               .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct));


            CreateMap<Video, VideoDto>()
                .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
                .ForMember(x => x.Url, dest => dest.MapFrom(x => x.Url))
                .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct));


            CreateMap<Feature, FeatureDto>()
                .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
                .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description));



            CreateMap<ProductFeature, ProductFeatureDto>()
                .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
                .ForMember(x => x.IdFeature, dest => dest.MapFrom(x => x.IdFeature))
                .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct));

        }
    }
}
