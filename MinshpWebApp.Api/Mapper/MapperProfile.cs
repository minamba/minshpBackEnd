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
              .ForMember(x => x.Model, dest => dest.MapFrom(x => x.Model))
              .ForMember(x => x.Brand, dest => dest.MapFrom(x => x.Brand))
              .ForMember(x => x.CreationDate, dest => dest.MapFrom(x => x.CreationDate))
              .ForMember(x => x.ModificationDate, dest => dest.MapFrom(x => x.ModificationDate))
              .ForMember(x => x.IdPromotionCode, dest => dest.MapFrom(x => x.IdPromotionCode))
              .ForMember(x => x.IdPackageProfil, dest => dest.MapFrom(x => x.IdPackageProfil))
              .ForMember(x => x.IdSubCategory, dest => dest.MapFrom(x => x.IdSubCategory))
              .ForMember(x => x.Display, dest => dest.MapFrom(x => x.Display))
              .ForMember(x => x.IdCategory, dest => dest.MapFrom(x => x.IdCategory));

            CreateMap<ProductDto, Product>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
              .ForMember(x => x.Stock, dest => dest.MapFrom(x => x.Stock))
              .ForMember(x => x.Main, dest => dest.MapFrom(x => x.Main))
              .ForMember(x => x.Model, dest => dest.MapFrom(x => x.Model))
              .ForMember(x => x.Brand, dest => dest.MapFrom(x => x.Brand))
              .ForMember(x => x.CreationDate, dest => dest.MapFrom(x => x.CreationDate))
              .ForMember(x => x.ModificationDate, dest => dest.MapFrom(x => x.ModificationDate))
              .ForMember(x => x.IdPromotionCode, dest => dest.MapFrom(x => x.IdPromotionCode))
              .ForMember(x => x.IdPackageProfil, dest => dest.MapFrom(x => x.IdPackageProfil))
              .ForMember(x => x.IdSubCategory, dest => dest.MapFrom(x => x.IdSubCategory))
              .ForMember(x => x.Display, dest => dest.MapFrom(x => x.Display))
              .ForMember(x => x.IdCategory, dest => dest.MapFrom(x => x.IdCategory));


            CreateMap<ProductDto, ProductVIewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
              .ForMember(x => x.Main, dest => dest.MapFrom(x => x.Main))
              .ForMember(x => x.Model, dest => dest.MapFrom(x => x.Model))
              .ForMember(x => x.Brand, dest => dest.MapFrom(x => x.Brand))
              .ForMember(x => x.CreationDate, dest => dest.MapFrom(x => x.CreationDate))
              .ForMember(x => x.ModificationDate, dest => dest.MapFrom(x => x.ModificationDate))
              .ForMember(x => x.IdPromotionCode, dest => dest.MapFrom(x => x.IdPromotionCode));

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
              .ForMember(x => x.IdTaxe, dest => dest.MapFrom(x => x.IdTaxe))
              .ForMember(x => x.IdPromotionCode, dest => dest.MapFrom(x => x.IdPromotionCode))
              .ForMember(x => x.IdPackageProfil, dest => dest.MapFrom(x => x.IdPackageProfil))
              .ForMember(x => x.ContentCode, dest => dest.MapFrom(x => x.ContentCode))
              .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name));

            CreateMap<Category, CategoryViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.TaxeId, dest => dest.MapFrom(x => x.IdTaxe))
              .ForMember(x => x.IdPromotionCode, dest => dest.MapFrom(x => x.IdPromotionCode))
              .ForMember(x => x.ContentCode, dest => dest.MapFrom(x => x.ContentCode))
              .ForMember(x => x.IdPackageProfil, dest => dest.MapFrom(x => x.IdPackageProfil))
              .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name));


            CreateMap<FeatureRequest, Feature>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
              .ForMember(x => x.IdFeatureCategory, dest => dest.MapFrom(x => x.IdFeatureCategory));


            CreateMap<ImageRequest, Image>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Url, dest => dest.MapFrom(x => x.Url))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
              .ForMember(x => x.Title, dest => dest.MapFrom(x => x.Title))
              .ForMember(x => x.Position, dest => dest.MapFrom(x => x.Position))
              .ForMember(x => x.IdCategory, dest => dest.MapFrom(x => x.IdCategory))
              .ForMember(x => x.IdSubCategory, dest => dest.MapFrom(x => x.IdSubCategory))
              .ForMember(x => x.Display, dest => dest.MapFrom(x => x.Display))
              .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct));


            CreateMap<Image, ImageViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Url, dest => dest.MapFrom(x => x.Url))
              .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct))
              .ForMember(x => x.Title, dest => dest.MapFrom(x => x.Title))
              .ForMember(x => x.Position, dest => dest.MapFrom(x => x.Position))
              .ForMember(x => x.IdCategory, dest => dest.MapFrom(x => x.IdCategory))
              .ForMember(x => x.IdSubCategory, dest => dest.MapFrom(x => x.IdSubCategory))
              .ForMember(x => x.Display, dest => dest.MapFrom(x => x.Display))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description));

            CreateMap<VideoRequest, Video>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Url, dest => dest.MapFrom(x => x.Url))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
              .ForMember(x => x.Title, dest => dest.MapFrom(x => x.Title))
              .ForMember(x => x.Position, dest => dest.MapFrom(x => x.Position))
              .ForMember(x => x.Display, dest => dest.MapFrom(x => x.Display))
              .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct));



            CreateMap<Video, VideoViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Url, dest => dest.MapFrom(x => x.Url))
              .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
              .ForMember(x => x.Title, dest => dest.MapFrom(x => x.Title))
              .ForMember(x => x.Position, dest => dest.MapFrom(x => x.Position))
              .ForMember(x => x.Display, dest => dest.MapFrom(x => x.Display))
              .ForMember(x => x.IdProduct, dest => dest.MapFrom(x => x.IdProduct));

            CreateMap<CustomerRequest, Customer>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.FirstName, dest => dest.MapFrom(x => x.FirstName))
              .ForMember(x => x.LastName, dest => dest.MapFrom(x => x.LastName))
              .ForMember(x => x.ClientNumber, dest => dest.MapFrom(x => x.ClientNumber))
              .ForMember(x => x.Email, dest => dest.MapFrom(x => x.Email))
              .ForMember(x => x.Pseudo, dest => dest.MapFrom(x => x.Pseudo))
              .ForMember(x => x.Civilite, dest => dest.MapFrom(x => x.Civilite))
              .ForMember(x => x.PhoneNumber, dest => dest.MapFrom(x => x.PhoneNumber))
              .ForMember(x => x.Actif, dest => dest.MapFrom(x => x.Actif))
              .ForMember(x => x.IdAspNetUser, dest => dest.MapFrom(x => x.IdAspNetUser))
              .ForMember(x => x.BirthDate, dest => dest.MapFrom(x => x.BirthDate));

            CreateMap<Customer, CustomerViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.FirstName, dest => dest.MapFrom(x => x.FirstName))
              .ForMember(x => x.LastName, dest => dest.MapFrom(x => x.LastName))
              .ForMember(x => x.ClientNumber, dest => dest.MapFrom(x => x.ClientNumber))
              .ForMember(x => x.Email, dest => dest.MapFrom(x => x.Email))
              .ForMember(x => x.Pseudo, dest => dest.MapFrom(x => x.Pseudo))
              .ForMember(x => x.Civilite, dest => dest.MapFrom(x => x.Civilite))
              .ForMember(x => x.PhoneNumber, dest => dest.MapFrom(x => x.PhoneNumber))
              .ForMember(x => x.Actif, dest => dest.MapFrom(x => x.Actif))
              .ForMember(x => x.IdAspNetUser, dest => dest.MapFrom(x => x.IdAspNetUser))
              .ForMember(x => x.Roles, dest => dest.MapFrom(x => x.Roles))
              .ForMember(x => x.BirthDate, dest => dest.MapFrom(x => x.BirthDate));


            CreateMap<OrderRequest, Order>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Date, dest => dest.MapFrom(x => x.Date))
              .ForMember(x => x.Amount, dest => dest.MapFrom(x => x.Amount))
              .ForMember(x => x.CustomerId, dest => dest.MapFrom(x => x.CustomerId))
              .ForMember(x => x.PaymentMethod, dest => dest.MapFrom(x => x.PaymentMethod))
              .ForMember(x => x.DeliveryAmount, dest => dest.MapFrom(x => x.DeliveryAmount))

              .ForMember(x => x.DeliveryMode, dest => dest.MapFrom(x => x.DeliveryMode))
              .ForMember(x => x.Carrier, dest => dest.MapFrom(x => x.Carrier))
              .ForMember(x => x.ServiceCode, dest => dest.MapFrom(x => x.ServiceCode))
              .ForMember(x => x.RelayId, dest => dest.MapFrom(x => x.RelayId))
              .ForMember(x => x.RelayLabel, dest => dest.MapFrom(x => x.RelayLabel))
              .ForMember(x => x.BoxtalShipmentId, dest => dest.MapFrom(x => x.BoxtalShipmentId))
              .ForMember(x => x.TrackingNumber, dest => dest.MapFrom(x => x.TrackingNumber))
              .ForMember(x => x.TrackingLink, dest => dest.MapFrom(x => x.TrackingLink))
              .ForMember(x => x.LabelUrl, dest => dest.MapFrom(x => x.LabelUrl))
              .ForMember(x => x.Status, dest => dest.MapFrom(x => x.Status));


            CreateMap<Order, OrderViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.OrderNumber, dest => dest.MapFrom(x => x.OrderNumber))
              .ForMember(x => x.Date, dest => dest.MapFrom(x => x.Date))
              .ForMember(x => x.Amount, dest => dest.MapFrom(x => x.Amount))
              .ForMember(x => x.PaymentMethod, dest => dest.MapFrom(x => x.PaymentMethod))
              .ForMember(x => x.DeliveryAmount, dest => dest.MapFrom(x => x.DeliveryAmount))

              .ForMember(x => x.DeliveryMode, dest => dest.MapFrom(x => x.DeliveryMode))
              .ForMember(x => x.Carrier, dest => dest.MapFrom(x => x.Carrier))               
              .ForMember(x => x.ServiceCode, dest => dest.MapFrom(x => x.ServiceCode))                      
              .ForMember(x => x.RelayId, dest => dest.MapFrom(x => x.RelayId))                          
              .ForMember(x => x.RelayLabel, dest => dest.MapFrom(x => x.RelayLabel))                    
              .ForMember(x => x.BoxtalShipmentId, dest => dest.MapFrom(x => x.BoxtalShipmentId))                          
              .ForMember(x => x.TrackingNumber, dest => dest.MapFrom(x => x.TrackingNumber))
              .ForMember(x => x.TrackingLink, dest => dest.MapFrom(x => x.TrackingLink))
              .ForMember(x => x.LabelUrl, dest => dest.MapFrom(x => x.LabelUrl))
              .ForMember(x => x.Status, dest => dest.MapFrom(x => x.Status));


            CreateMap<Invoice, InvoiceViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.CustomerId, dest => dest.MapFrom(x => x.CustomerId))
              .ForMember(x => x.OrderId, dest => dest.MapFrom(x => x.OrderId))
              .ForMember(x => x.DateCreation, dest => dest.MapFrom(x => x.DateCreation))
              .ForMember(x => x.Representative, dest => dest.MapFrom(x => x.Representative))
              .ForMember(x => x.InvoiceLink, dest => dest.MapFrom(x => x.InvoiceLink))
              .ForMember(x => x.InvoiceNumber, dest => dest.MapFrom(x => x.InvoiceNumber));


            CreateMap<InvoiceRequest, Invoice>()
             .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
             .ForMember(x => x.CustomerId, dest => dest.MapFrom(x => x.CustomerId))
             .ForMember(x => x.OrderId, dest => dest.MapFrom(x => x.OrderId))
             .ForMember(x => x.InvoiceLink, dest => dest.MapFrom(x => x.InvoiceLink))
             .ForMember(x => x.HardDelete, dest => dest.MapFrom(x => x.HardDelete))
             .ForMember(x => x.Representative, dest => dest.MapFrom(x => x.Representative));



            CreateMap<Feature,FeatureViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.IdCategory, dest => dest.MapFrom(x => x.IdCategory))
              .ForMember(x => x.IdFeatureCategory, dest => dest.MapFrom(x => x.IdFeatureCategory))
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


            CreateMap<FeatureCategoryRequest, FeatureCategory>()
               .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
               .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name));


            CreateMap<FeatureCategory, FeatureCategoryViewModel>()
               .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
               .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name));



            CreateMap<TaxeRequest, Taxe>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name))
              .ForMember(x => x.Purcentage, dest => dest.MapFrom(x => x.Purcentage))
              .ForMember(x => x.Amount, dest => dest.MapFrom(x => x.Amount));


            CreateMap<Taxe, TaxeViewModel>()
               .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
               .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name))
               .ForMember(x => x.Purcentage, dest => dest.MapFrom(x => x.Purcentage))
               .ForMember(x => x.Amount, dest => dest.MapFrom(x => x.Amount));



            CreateMap<PromotionCodeRequest, PromotionCode>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name))
              .ForMember(x => x.Purcentage, dest => dest.MapFrom(x => x.Purcentage))
              .ForMember(x => x.StartDate, dest => dest.MapFrom(x => x.StartDate))
              .ForMember(x => x.EndDate, dest => dest.MapFrom(x => x.EndDate))
              .ForMember(x => x.DateCreation, dest => dest.MapFrom(x => x.DateCreation));


            CreateMap<PromotionCode, PromotionCodeViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name))
              .ForMember(x => x.Purcentage, dest => dest.MapFrom(x => x.Purcentage))
              .ForMember(x => x.StartDate, dest => dest.MapFrom(x => x.StartDate))
              .ForMember(x => x.EndDate, dest => dest.MapFrom(x => x.EndDate))
              .ForMember(x => x.DateCreation, dest => dest.MapFrom(x => x.DateCreation));


            CreateMap<ApplicationRequest, Application>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.DisplayNewProductNumber, dest => dest.MapFrom(x => x.DisplayNewProductNumber))
              .ForMember(x => x.StartDate, dest => dest.MapFrom(x => x.StartDate))
              .ForMember(x => x.DefaultDropOffChronoPost, dest => dest.MapFrom(x => x.DefaultDropOffChronoPost))
              .ForMember(x => x.DefaultDropOffUps, dest => dest.MapFrom(x => x.DefaultDropOffUps))
              .ForMember(x => x.DefaultDropOffMondialRelay, dest => dest.MapFrom(x => x.DefaultDropOffMondialRelay))
              .ForMember(x => x.DefaultDropLaposte, dest => dest.MapFrom(x => x.DefaultDropLaposte))
              .ForMember(x => x.SocietyName, dest => dest.MapFrom(x => x.SocietyName))
              .ForMember(x => x.SocietyAddress, dest => dest.MapFrom(x => x.SocietyAddress))
              .ForMember(x => x.SocietyZipCode, dest => dest.MapFrom(x => x.SocietyZipCode))
              .ForMember(x => x.SocietyCity, dest => dest.MapFrom(x => x.SocietyCity))
              .ForMember(x => x.IsMaintenance, dest => dest.MapFrom(x => x.IsMaintenance))
              .ForMember(x => x.EndDate, dest => dest.MapFrom(x => x.EndDate));


            CreateMap<Application, ApplicationViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.DisplayNewProductNumber, dest => dest.MapFrom(x => x.DisplayNewProductNumber))
              .ForMember(x => x.StartDate, dest => dest.MapFrom(x => x.StartDate))
              .ForMember(x => x.DefaultDropOffChronoPost, dest => dest.MapFrom(x => x.DefaultDropOffChronoPost))
              .ForMember(x => x.DefaultDropOffUps, dest => dest.MapFrom(x => x.DefaultDropOffUps))
              .ForMember(x => x.DefaultDropOffMondialRelay, dest => dest.MapFrom(x => x.DefaultDropOffMondialRelay))
              .ForMember(x => x.SocietyName, dest => dest.MapFrom(x => x.SocietyName))
              .ForMember(x => x.SocietyAddress, dest => dest.MapFrom(x => x.SocietyAddress))
              .ForMember(x => x.SocietyZipCode, dest => dest.MapFrom(x => x.SocietyZipCode))
              .ForMember(x => x.SocietyCity, dest => dest.MapFrom(x => x.SocietyCity))
              .ForMember(x => x.IsMaintenance, dest => dest.MapFrom(x => x.IsMaintenance))
              .ForMember(x => x.EndDate, dest => dest.MapFrom(x => x.EndDate));



            CreateMap<BillingAddress, BillingAddressViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Address, dest => dest.MapFrom(x => x.Address))
              .ForMember(x => x.IdCustomer, dest => dest.MapFrom(x => x.IdCustomer))
              .ForMember(x => x.ComplementaryAddress, dest => dest.MapFrom(x => x.ComplementaryAddress))
              .ForMember(x => x.City, dest => dest.MapFrom(x => x.City))
              .ForMember(x => x.Country, dest => dest.MapFrom(x => x.Country))
              .ForMember(x => x.PostalCode, dest => dest.MapFrom(x => x.PostalCode));


            CreateMap<BillingAddressRequest, BillingAddress>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Address, dest => dest.MapFrom(x => x.Address))
              .ForMember(x => x.IdCustomer, dest => dest.MapFrom(x => x.IdCustomer))
              .ForMember(x => x.ComplementaryAddress, dest => dest.MapFrom(x => x.ComplementaryAddress))
              .ForMember(x => x.City, dest => dest.MapFrom(x => x.City))
              .ForMember(x => x.Country, dest => dest.MapFrom(x => x.Country))
              .ForMember(x => x.PostalCode, dest => dest.MapFrom(x => x.PostalCode));


            CreateMap<DeliveryAddress, DeliveryAddressViewModel>()
                .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
                .ForMember(x => x.Address, dest => dest.MapFrom(x => x.Address))
                .ForMember(x => x.IdCustomer, dest => dest.MapFrom(x => x.IdCustomer))
                .ForMember(x => x.ComplementaryAddress, dest => dest.MapFrom(x => x.ComplementaryAddress))
                .ForMember(x => x.City, dest => dest.MapFrom(x => x.City))
                .ForMember(x => x.Country, dest => dest.MapFrom(x => x.Country))
                .ForMember(x => x.Favorite, dest => dest.MapFrom(x => x.Favorite))
                .ForMember(x => x.LastName, dest => dest.MapFrom(x => x.LastName))
                .ForMember(x => x.FirstName, dest => dest.MapFrom(x => x.FirstName))
                .ForMember(x => x.Phone, dest => dest.MapFrom(x => x.Phone))
                .ForMember(x => x.Civilite, dest => dest.MapFrom(x => x.Civilite))
                .ForMember(x => x.PostalCode, dest => dest.MapFrom(x => x.PostalCode));


            CreateMap<DeliveryAddressRequest, DeliveryAddress>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Address, dest => dest.MapFrom(x => x.Address))
              .ForMember(x => x.IdCustomer, dest => dest.MapFrom(x => x.IdCustomer))
              .ForMember(x => x.ComplementaryAddress, dest => dest.MapFrom(x => x.ComplementaryAddress))
              .ForMember(x => x.City, dest => dest.MapFrom(x => x.City))
              .ForMember(x => x.Country, dest => dest.MapFrom(x => x.Country))
              .ForMember(x => x.Favorite, dest => dest.MapFrom(x => x.Favorite))
              .ForMember(x => x.LastName, dest => dest.MapFrom(x => x.LastName))
              .ForMember(x => x.FirstName, dest => dest.MapFrom(x => x.FirstName))
              .ForMember(x => x.Phone, dest => dest.MapFrom(x => x.Phone))
              .ForMember(x => x.Civilite, dest => dest.MapFrom(x => x.Civilite))
              .ForMember(x => x.PostalCode, dest => dest.MapFrom(x => x.PostalCode));




            CreateMap<OrderCustomerProductRequest, OrderCustomerProduct>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.OrderId, dest => dest.MapFrom(x => x.OrderId))
              .ForMember(x => x.CustomerId, dest => dest.MapFrom(x => x.CustomerId))
              .ForMember(x => x.Quantity, dest => dest.MapFrom(x => x.Quantity))
              .ForMember(x => x.ProductUnitPrice, dest => dest.MapFrom(x => x.ProductUnitPrice))
              .ForMember(x => x.ProductId, dest => dest.MapFrom(x => x.ProductId));


            CreateMap<OrderCustomerProduct, OrderCustomerProductViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.OrderId, dest => dest.MapFrom(x => x.OrderId))
              .ForMember(x => x.CustomerId, dest => dest.MapFrom(x => x.CustomerId))
              .ForMember(x => x.Quantity, dest => dest.MapFrom(x => x.Quantity))
              .ForMember(x => x.ProductUnitPrice, dest => dest.MapFrom(x => x.ProductUnitPrice))
              .ForMember(x => x.ProductId, dest => dest.MapFrom(x => x.ProductId));



            CreateMap<RelaysAddressRequest, RelaysAddress>()
               .ForMember(x => x.Number, dest => dest.MapFrom(x => x.Number))
               .ForMember(x => x.PostalCode, dest => dest.MapFrom(x => x.PostalCode))
               .ForMember(x => x.State, dest => dest.MapFrom(x => x.State))
               .ForMember(x => x.Street, dest => dest.MapFrom(x => x.Street))
               .ForMember(x => x.SearchNetworks, dest => dest.MapFrom(x => x.SearchNetworks))
               .ForMember(x => x.CountryIsoCode, dest => dest.MapFrom(x => x.CountryIsoCode))
               .ForMember(x => x.Limit, dest => dest.MapFrom(x => x.Limit))
               .ForMember(x => x.City, dest => dest.MapFrom(x => x.City));



            CreateMap<RelaysAddress,RelaysAddressRequest>()
               .ForMember(x => x.Number, dest => dest.MapFrom(x => x.Number))
               .ForMember(x => x.PostalCode, dest => dest.MapFrom(x => x.PostalCode))
               .ForMember(x => x.State, dest => dest.MapFrom(x => x.State))
               .ForMember(x => x.Street, dest => dest.MapFrom(x => x.Street))
               .ForMember(x => x.SearchNetworks, dest => dest.MapFrom(x => x.SearchNetworks))
               .ForMember(x => x.CountryIsoCode, dest => dest.MapFrom(x => x.CountryIsoCode))
               .ForMember(x => x.Limit, dest => dest.MapFrom(x => x.Limit))
               .ForMember(x => x.City, dest => dest.MapFrom(x => x.City));



            CreateMap<Rate, RateViewModel>()
               .ForMember(x => x.Code, dest => dest.MapFrom(x => x.Code))
               .ForMember(x => x.Carrier, dest => dest.MapFrom(x => x.Carrier))
               .ForMember(x => x.PriceTtc, dest => dest.MapFrom(x => x.PriceTtc))
               .ForMember(x => x.IsRelay, dest => dest.MapFrom(x => x.IsRelay))
               .ForMember(x => x.DropOffPointCodes, dest => dest.MapFrom(x => x.DropOffPointCodes))
               .ForMember(x => x.PickupPointCodes, dest => dest.MapFrom(x => x.PickupPointCodes))
               .ForMember(x => x.Label, dest => dest.MapFrom(x => x.Label));


            CreateMap<OrderDetailsRequest, OrderDetails>()
               .ForMember(x => x.SenderZipCode, dest => dest.MapFrom(x => x.SenderZipCode))
               .ForMember(x => x.SenderType, dest => dest.MapFrom(x => x.SenderType))
               .ForMember(x => x.SenderCity, dest => dest.MapFrom(x => x.SenderCity))

               .ForMember(x => x.RecipientCity, dest => dest.MapFrom(x => x.RecipientCity))
               .ForMember(x => x.RecipientCountry, dest => dest.MapFrom(x => x.RecipientCountry))
               .ForMember(x => x.RecipientType, dest => dest.MapFrom(x => x.RecipientType))
               .ForMember(x => x.RecipientZipCode, dest => dest.MapFrom(x => x.RecipientZipCode))
               .ForMember(x => x.Packages, dest => dest.MapFrom(x => x.Packages))
               .ForMember(x => x.ContainedCode, dest => dest.MapFrom(x => x.ContainedCode));


            CreateMap<PackageRequest, Package>()
               .ForMember(x => x.PackageId, dest => dest.MapFrom(x => x.Id))
               .ForMember(x => x.ContentCodeDefault, dest => dest.MapFrom(x => x.ContainedCode))
               .ForMember(x => x.PackageValue, dest => dest.MapFrom(x => x.PackageValue))
               .ForMember(x => x.PackageWidth, dest => dest.MapFrom(x => x.PackageWidth))
               .ForMember(x => x.PackageWeight, dest => dest.MapFrom(x => x.PackageWeight))
               .ForMember(x => x.PackageHeight, dest => dest.MapFrom(x => x.PackageHeight))
               .ForMember(x => x.PackageLonger, dest => dest.MapFrom(x => x.PackageLonger));

            CreateMap<RateViewModel, Rate>()
               .ForMember(x => x.Code, dest => dest.MapFrom(x => x.Code))
               .ForMember(x => x.Carrier, dest => dest.MapFrom(x => x.Carrier))
               .ForMember(x => x.PriceTtc, dest => dest.MapFrom(x => x.PriceTtc))
               .ForMember(x => x.IsRelay, dest => dest.MapFrom(x => x.IsRelay))
               .ForMember(x => x.Label, dest => dest.MapFrom(x => x.Label));


            CreateMap<Rate, RateViewModel >()
               .ForMember(x => x.Code, dest => dest.MapFrom(x => x.Code))
               .ForMember(x => x.Carrier, dest => dest.MapFrom(x => x.Carrier))
               .ForMember(x => x.PriceTtc, dest => dest.MapFrom(x => x.PriceTtc))
               .ForMember(x => x.IsRelay, dest => dest.MapFrom(x => x.IsRelay))
               .ForMember(x => x.Label, dest => dest.MapFrom(x => x.Label));



            CreateMap<PackageProfil, PackageProfilViewModel>()
               .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
               .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name))
               .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
               .ForMember(x => x.Width, dest => dest.MapFrom(x => x.Width))
               .ForMember(x => x.Weight, dest => dest.MapFrom(x => x.Weight))
               .ForMember(x => x.Longer, dest => dest.MapFrom(x => x.Longer))
               .ForMember(x => x.Height, dest => dest.MapFrom(x => x.Height));


            CreateMap<PackageProfil, PackageProfilRequest>()
               .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
               .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name))
               .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
               .ForMember(x => x.Width, dest => dest.MapFrom(x => x.Width))
               .ForMember(x => x.Weight, dest => dest.MapFrom(x => x.Weight))
               .ForMember(x => x.Longer, dest => dest.MapFrom(x => x.Longer))
               .ForMember(x => x.Height, dest => dest.MapFrom(x => x.Height));


            CreateMap<PackageProfilRequest, PackageProfil>()
               .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
               .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name))
               .ForMember(x => x.Description, dest => dest.MapFrom(x => x.Description))
               .ForMember(x => x.Width, dest => dest.MapFrom(x => x.Width))
               .ForMember(x => x.Weight, dest => dest.MapFrom(x => x.Weight))
               .ForMember(x => x.Longer, dest => dest.MapFrom(x => x.Longer))
               .ForMember(x => x.Height, dest => dest.MapFrom(x => x.Height));


            CreateMap<CodeCategories, CodeCategoriesViewModel>()
               .ForMember(x => x.AllCodeCategories, dest => dest.MapFrom(x => x.AllCodeCategories));



            CreateMap<CodeCategory, CodeCategoryViewModel>()
               .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
               .ForMember(x => x.Label, dest => dest.MapFrom(x => x.Label));


            CreateMap<SubCategoryRequest, SubCategory>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.IdTaxe, dest => dest.MapFrom(x => x.IdTaxe))
              .ForMember(x => x.IdPromotionCode, dest => dest.MapFrom(x => x.IdPromotionCode))
              .ForMember(x => x.ContentCode, dest => dest.MapFrom(x => x.ContentCode))
              .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name));



            CreateMap<SubCategory, SubCategoryViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.IdTaxe, dest => dest.MapFrom(x => x.IdTaxe))
              .ForMember(x => x.IdPromotionCode, dest => dest.MapFrom(x => x.IdPromotionCode))
              .ForMember(x => x.ContentCode, dest => dest.MapFrom(x => x.ContentCode))
              .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name));


            CreateMap<AspNetRole, AspNetRoleViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Name, dest => dest.MapFrom(x => x.Name))
              .ForMember(x => x.NormalizedName, dest => dest.MapFrom(x => x.NormalizedName));


            CreateMap<CustomerPromotionCode, CustomerPromotionCodeViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.IdCutomer, dest => dest.MapFrom(x => x.IdCutomer))
              .ForMember(x => x.IsUsed, dest => dest.MapFrom(x => x.IsUsed));

            CreateMap<CustomerPromotionCodeRequest, CustomerPromotionCode>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.IdCutomer, dest => dest.MapFrom(x => x.IdCutomer))
              .ForMember(x => x.IsUsed, dest => dest.MapFrom(x => x.IsUsed));


            CreateMap<NewLetter, NewLetterViewModel>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Mail, dest => dest.MapFrom(x => x.Mail))
              .ForMember(x => x.Suscribe, dest => dest.MapFrom(x => x.Suscribe));


            CreateMap<NewLetterRequest, NewLetter>()
              .ForMember(x => x.Id, dest => dest.MapFrom(x => x.Id))
              .ForMember(x => x.Mail, dest => dest.MapFrom(x => x.Mail))
              .ForMember(x => x.OldMAil, dest => dest.MapFrom(x => x.OldMAil))
              .ForMember(x => x.Suscribe, dest => dest.MapFrom(x => x.Suscribe));

        }
    }
}
