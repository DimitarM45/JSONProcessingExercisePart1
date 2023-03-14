namespace ProductShop;

using Data;
using Models;
using DTOs.Import;

using AutoMapper;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using ProductShop.DTOs.Export;
using Newtonsoft.Json.Serialization;

public class StartUp
{
    public static void Main()
    {
        using ProductShopContext context = new ProductShopContext();

        //context.Database.EnsureDeleted();
        //context.Database.EnsureCreated();

        string result = GetSoldProducts(context);

        File.WriteAllText(@"..\..\..\Results\users-sold-products.json", result);

        Console.WriteLine(result);
    }

    //Problem 1

    public static string ImportUsers(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportUserDto[] userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson)!;

        User[] users = mapper.Map<User[]>(userDtos);

        context.Users.AddRange(users);
        context.SaveChanges();

        return $"Successfully imported {users.Length}";
    }

    //Problem 2

    public static string ImportProducts(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportProductDto[] productDtos = JsonConvert.DeserializeObject<ImportProductDto[]>(inputJson)!;

        Product[] products = mapper.Map<Product[]>(productDtos);

        context.Products.AddRange(products);
        context.SaveChanges();

        return $"Successfully imported {products.Length}";
    }

    //Prolem 3 

    public static string ImportCategories(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportCategoryDto[] categoryDtos = JsonConvert.DeserializeObject<ImportCategoryDto[]>(inputJson)!;

        ICollection<Category> validCategories = new HashSet<Category>();

        foreach (ImportCategoryDto categoryDto in categoryDtos)
        {
            if (!String.IsNullOrEmpty(categoryDto.Name))
            {
                Category category = mapper.Map<Category>(categoryDto);

                validCategories.Add(category);
            }
        }

        context.Categories.AddRange(validCategories);
        context.SaveChanges();

        return $"Successfully imported {validCategories.Count}";
    }

    //Problem 4

    public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportCategoryProductDto[] categoryProductDtos = JsonConvert.DeserializeObject<ImportCategoryProductDto[]>(inputJson)!;

        ICollection<CategoryProduct> validEntries = new HashSet<CategoryProduct>();

        foreach (ImportCategoryProductDto cpDto in categoryProductDtos)
        {
            if (!context.Categories.Any(c => c.Id == cpDto.CategoryId) ||
                !context.Products.Any(p => p.Id == cpDto.ProductId))
                continue;

            CategoryProduct categoryProduct = mapper.Map<CategoryProduct>(cpDto);

            validEntries.Add(categoryProduct);
        }

        context.CategoriesProducts.AddRange(validEntries);
        context.SaveChanges();

        return $"Successfully imported {validEntries.Count}";
    }

    //Problem 5

    public static string GetProductsInRange(ProductShopContext context)
    {
        IConfigurationProvider config = CreateMapperConfig();

        var productDtosInRange = context.Products
            .AsNoTracking()
            .Where(p => p.Price >= 500 && p.Price <= 1000)
            .OrderBy(p => p.Price)
            .ProjectTo<ExportProductDto>(config)
            .ToArray();

        string exportProductJson = JsonConvert.SerializeObject(productDtosInRange, Formatting.Indented);

        return exportProductJson;
    }

    //Problem 6

    public static string GetSoldProducts(ProductShopContext context)
    {
        IConfigurationProvider config = CreateMapperConfig();

        var userDtos = context.Users!
            .AsNoTracking()
            .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ProjectTo<ExportUserProductsDto>(config)
            .ToArray();

        string exportUserDtosJson = JsonConvert.SerializeObject(userDtos, Formatting.Indented);

        return exportUserDtosJson;
    }

    //Problem 7 Add properly made Dtos from this point on

    public static string GetCategoriesByProductsCount(ProductShopContext context)
    {
        var categoriesDtos = context.Categories
            .AsNoTracking()
            .OrderByDescending(c => c.CategoriesProducts.Count)
            .Select(c => new ExportCategoryDto()
            {
                Name = c.Name,
                ProductsCount = c.CategoriesProducts.Count,
                AveragePrice = c.CategoriesProducts.Average(cp => cp.Product.Price),
                TotalRevenue = c.CategoriesProducts.Sum(cp => cp.Product.Price)
            })
            .ToArray();

        string exportCategoriesJson = JsonConvert.SerializeObject(categoriesDtos, Formatting.Indented);

        return exportCategoriesJson;
    }

    //Problem 8 SOLVE

    public static string GetUsersWithProducts(ProductShopContext context)
    {
        var users = context.Users
            .AsNoTracking()
            .Where(u => u.ProductsSold.Count > 0 && u.ProductsSold.Any(ps => ps.BuyerId != null))
            .OrderByDescending(u => u.ProductsSold.Count);

        return null;
    }

    private static IMapper CreateMapper()
    {
        IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductShopProfile>();
        }));

        return mapper;
    }

    private static IConfigurationProvider CreateMapperConfig()
    {
        IConfigurationProvider config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductShopProfile>();
        });

        return config;
    }
}