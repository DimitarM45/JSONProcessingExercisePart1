namespace ProductShop;

using Data;
using Models;
using DTOs.Import;
using DTOs.Export;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

public class StartUp
{
    public static void Main()
    {
        using ProductShopContext context = new ProductShopContext();
    }

    //Problem 1

    public static string ImportUsers(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportUserDto[] userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson)!;

        User[] users = mapper.Map<User[]>(userDtos);

        context.Users!.AddRange(users);
        context.SaveChanges();

        return $"Successfully imported {users.Length}";
    }

    //Problem 2

    public static string ImportProducts(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportProductDto[] productDtos = JsonConvert.DeserializeObject<ImportProductDto[]>(inputJson)!;

        Product[] products = mapper.Map<Product[]>(productDtos);

        context.Products!.AddRange(products);
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

        context.Categories!.AddRange(validCategories);
        context.SaveChanges();

        return $"Successfully imported {validCategories.Count}";
    }

    //Problem 4

    public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportCategoryProductDto[] categoryProductDtos = JsonConvert.DeserializeObject<ImportCategoryProductDto[]>(inputJson)!;

        CategoryProduct[] entries = mapper.Map<CategoryProduct[]>(categoryProductDtos);

        context.CategoriesProducts!.AddRange(entries);
        context.SaveChanges();

        return $"Successfully imported {entries.Length}";
    }

    //Problem 5

    public static string GetProductsInRange(ProductShopContext context)
    {
        IConfigurationProvider config = CreateMapperConfig();

        var productDtosInRange = context.Products!
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

    //Problem 7 

    public static string GetCategoriesByProductsCount(ProductShopContext context)
    {
        IConfigurationProvider config = CreateMapperConfig();

        var categoriesDtos = context.Categories!
            .AsNoTracking()
            .OrderByDescending(c => c.CategoriesProducts.Count)
            .ProjectTo<ExportCategoryDto>(config)
            .ToArray();

        string exportCategoriesJson = JsonConvert.SerializeObject(categoriesDtos, Formatting.Indented);

        return exportCategoriesJson;
    }

    //Problem 8

    public static string GetUsersWithProducts(ProductShopContext context)
    {
        var users = context.Users!
            .AsNoTracking()
            .Include(u => u.ProductsSold)
            .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
            .ToArray();

        var jsonTemplate = new
        {
            usersCount = users.Length,
            users = users
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Age,
                    SoldProducts = new
                    {
                        Count = u.ProductsSold
                            .Count(ps => ps.BuyerId != null),
                        Products = u.ProductsSold
                            .Where(ps => ps.BuyerId != null)
                            .Select(ps => new
                            {
                                ps.Name,
                                ps.Price
                            })
                            .ToArray()
                    }
                })
                .OrderByDescending(u => u.SoldProducts.Count)
        };

        string usersProductsJson = JsonConvert.SerializeObject(jsonTemplate, new JsonSerializerSettings()
        {
            ContractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() },
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        });

        return usersProductsJson;
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