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
                otp => otp.MapFrom(src => src.ProductsSold));

        CreateMap<Product, ExportProductUserDto>();

        CreateMap<Category, ExportCategoryDto>()
            .ForMember(c => c.ProductsCount,
                otp => otp.MapFrom(src => src.CategoriesProducts.Count))
            .ForMember(c => c.AveragePrice,
                otp => otp.MapFrom(src => $"{src.CategoriesProducts.Average(cp => cp.Product.Price):f2}"))
            .ForMember(c => c.TotalRevenue,
                otp => otp.MapFrom(src => $"{src.CategoriesProducts.Sum(cp => cp.Product.Price):f2}"));
    }
}
