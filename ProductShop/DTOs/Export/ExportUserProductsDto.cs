namespace ProductShop.DTOs.Export;

using Newtonsoft.Json;

public class ExportUserProductsDto
{
    [JsonProperty("firstName")]

    public string? FirstName { get; set; }

    [JsonProperty("lastName")]

    public string LastName { get; set; } = null!;

    [JsonProperty(Order = 1, PropertyName = "soldProducts")]

    public ExportProductUserDto[] SoldProducts = null!;
}
