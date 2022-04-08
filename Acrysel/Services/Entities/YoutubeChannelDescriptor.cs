using System;
using System.Text.Json.Serialization;

namespace Acrysel.Services.Entities;

public class YoutubeChannelDescriptor
{
    [JsonPropertyName("channelId")] public string? Id { get; set; }

    [JsonPropertyName("channelTitle")] public string? ChannelTitle { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("publishedAt")] public DateTime CreatedAt { get; set; }
}