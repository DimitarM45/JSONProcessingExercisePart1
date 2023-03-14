namespace ProductShop.DTOs.Export;

using Newtonsoft.Json;

public class ExportProductDto
{
    [JsonProperty("name")]

    public string Name { get; set; } = null!;

    [JsonProperty("price")]

    public decimal Price { get; set; }

    [JsonIgnore]

    public string? SellerFirstName { get; set; }

    [JsonIgnore]

    public string? SellerLastName { get; set; } 

    [JsonProperty("seller")]

    public string SellerFullName => $"{SellerFirstName} {SellerLastName}";
}
