using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
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
              .ForMember(x => x.Stock, dest => dest.MapFrom(x => x.Stock))
              .ForMember(x => x.Main, dest => dest.MapFrom(x => x.Main))
              .ForMember(x => x.IdCategory, dest => dest.MapFrom(x => x.IdCategory));

            CreateMap<ProductDto, Product>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
              .ForMember(x => x.Stock, dest => dest.MapFrom(x => x.Stock))
              .ForMember(x => x.Main, dest => dest.MapFrom(x => x.Main))
              .ForMember(x => x.IdCategory, dest => dest.MapFrom(x => x.IdCategory));

            CreateMap<PromotionRequest, Promotion>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.StartDate, dest => dest.MapFrom(x => x.StartDate))
              .ForMember(x => x.EndDate, dest => dest.MapFrom(x => x.EndDate))
              .ForMember(x => x.Purcentage, dest => dest.MapFrom(x => x.Purcentage))
              .ForMember(x => x.DateCreation, dest => dest.MapFrom(x => x.DateCreation))
              .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct));



            CreateMap<Promotion, PromotionViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.StartDate, dest => dest.MapFrom(x => x.StartDate))
              .ForMember(x => x.EndDate, dest => dest.MapFrom(x => x.EndDate))
              .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct))
              .ForMember(x => x.Purcentage, dest => dest.MapFrom(x => x.Purcentage));


            CreateMap<CategoryRequest, Category>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name));

            CreateMap<Category, CategoryViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name));


            CreateMap<FeatureRequest, Feature>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description));


            CreateMap<ImageRequest, Image>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Url, dest => dest.MapFrom(x => x.Url))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
              .ForMember(x => x.Title, dest => dest.MapFrom(x => x.Title))
              .ForMember(x => x.Position, dest => dest.MapFrom(x => x.Position))
              .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct));


            CreateMap<Image, ImageViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Url, dest => dest.MapFrom(x => x.Url))
              .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct))
              .ForMember(x => x.Title, dest => dest.MapFrom(x => x.Title))
              .ForMember(x => x.Position, dest => dest.MapFrom(x => x.Position))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description));

            CreateMap<VideoRequest, Video>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Url, dest => dest.MapFrom(x => x.Url))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
              .ForMember(x => x.Title, dest => dest.MapFrom(x => x.Title))
              .ForMember(x => x.Position, dest => dest.MapFrom(x => x.Position))
              .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct));



            CreateMap<Video, VideoViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Url, dest => dest.MapFrom(x => x.Url))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
              .ForMember(x => x.Title, dest => dest.MapFrom(x => x.Title))
              .ForMember(x => x.Position, dest => dest.MapFrom(x => x.Position))
              .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct));

            CreateMap<CustomerRequest, Customer>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.FirstName, dest => dest.MapFrom(x => x.FirstName))
              .ForMember(x => x.LastName, dest => dest.MapFrom(x => x.LastName))
              .ForMember(x => x.PhoneNumber, dest => dest.MapFrom(x => x.PhoneNumber))
              .ForMember(x => x.DeliveryAddress, dest => dest.MapFrom(x => x.DeliveryAddress))
              .ForMember(x => x.BillingAddress, dest => dest.MapFrom(x => x.BillingAddress))
              .ForMember(x => x.Password, dest => dest.MapFrom(x => x.Password));


            CreateMap<OrderRequest, Order>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.OrderNumber, dest => dest.MapFrom(x => x.OrderNumber))
              .ForMember(x => x.Quantity, dest => dest.MapFrom(x => x.Quantity))
              .ForMember(x => x.Date, dest => dest.MapFrom(x => x.Date))
              .ForMember(x => x.IdCustomer, dest => dest.MapFrom(x => x.IdCustomer))
              .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.Id_product))
              .ForMember(x => x.Status, dest => dest.MapFrom(x => x.Status));



            CreateMap<Feature,FeatureViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.IdCategory, dest => dest.MapFrom(x => x.IdCategory))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description));


            CreateMap<Stock, StockViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Quantity, dest => dest.MapFrom(x => x.Quantity));


            CreateMap<Stock, StockViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct))
              .ForMember(x => x.Quantity, dest => dest.MapFrom(x => x.Quantity));

            CreateMap<StockRequest, Stock>()
               .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
               .ForMember(x => x.Quantity, dest => dest.MapFrom(x => x.Quantity))
               .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct));


            CreateMap<ProductFeatureRequest, ProductFeature>()
               .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
               .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct))
               .ForMember(x => x.IdFeature, dest => dest.MapFrom(x => x.Id_feature));



            CreateMap<ProductFeature, ProductFeatureViewModel>()
               .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
               .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct))
               .ForMember(x => x.IdFeature, dest => dest.MapFrom(x => x.IdFeature));
        }
    }
}
