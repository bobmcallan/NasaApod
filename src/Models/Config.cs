namespace Models;

using Newtonsoft.Json;

public class Config
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("source")]
    public string Source { get; set; }


    [JsonProperty("category")]
    public string Category { get; set; }


    [JsonProperty("name")]
    public string Name { get; set; }


    [JsonProperty("value")]
    public string Value { get; set; }

}