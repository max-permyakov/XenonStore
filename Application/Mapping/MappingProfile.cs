//using AutoMapper;
//using Xenon.Application.DTOs;
//using Xenon.Domain.Models;

//namespace Xenon.Application.Mapping
//{
//    public class MappingProfile : Profile
//    {
//        public MappingProfile()
//        {
//            // Product → ProductDto
//            CreateMap<Product, ProductDto>()
//                .ForMember(dest => dest.CategoryName,
//                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
//                .ForMember(dest => dest.SupplierName,
//                    opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null));

//            // CreateProductDto → Product
//            //CreateMap<CreateProductDto, Product>();

//            // UpdateProductDto → Product
//            CreateMap<UpdateProductDto, Product>();

//            // Category
//            CreateMap<Category, CategoryDto>();
//            CreateMap<CreateCategoryDto, Category>();

//            // Supplier
//            CreateMap<Supplier, SupplierDto>();
//            CreateMap<CreateSupplierDto, Supplier>();

//            // Order
//            CreateMap<Order, OrderDto>();
//            CreateMap<CreateOrderDto, Order>();
//        }
//    }
//}