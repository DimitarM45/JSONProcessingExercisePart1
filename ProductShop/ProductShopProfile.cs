namespace ProductShop;

using Models;
using DTOs.Import;
using DTOs.Export;

using AutoMapper;

public class ProductShopProfile : Profile
{
    public ProductShopProfile()
    {
        CreateMap<ImportUserDto, User>();

        CreateMap<ImportProductDto, Product>();

        CreateMap<ImportCategoryDto, Category>();

        CreateMap<ImportCategoryProductDto, CategoryProduct>();

        CreateMap<Product, ExportProductDto>();

        CreateMap<User, ExportUserProductsDto>()
            .ForMember(u => u.SoldProducts,
                opt => opt.MapFrom(src => src.ProductsSold));

        CreateMap<Product, ExportProductUserDto>();
    }
}
