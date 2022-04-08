using System.Text.Json.Serialization;

namespace Acrysel.Services.Entities;

public class YoutubeChannelSearchResponse
{
    [JsonPropertyName("items")] public Item[] Items { get; set; }

    public class Item
    {
        [JsonPropertyName("snippet")] public YoutubeChannelDescriptor Descriptor { get; set; }
    }
}