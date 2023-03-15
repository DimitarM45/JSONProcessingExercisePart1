namespace ProductShop.DTOs.Export;

using Newtonsoft.Json;

public class ExportCategoryDto
{
    [JsonProperty("category")]

    public string? Name { get; set; }

    [JsonProperty("productsCount")]

    public int ProductsCount { get; set; }

    [JsonProperty("averagePrice")]

    public string? AveragePrice { get; set; }

    [JsonProperty("totalRevenue")]

    public string? TotalRevenue { get; set; }
}
