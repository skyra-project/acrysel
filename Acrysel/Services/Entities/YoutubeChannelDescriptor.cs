using System;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Acrysel.Services.Entities;

[PublicAPI]
public class YoutubeChannelDescriptor
{
    [JsonPropertyName("channelId")] public string? Id { get; set; }

    [JsonPropertyName("channelTitle")] public string? ChannelTitle { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("publishedAt")] public DateTime CreatedAt { get; set; }

    [JsonPropertyName("thumbnails")] public Thumbnails? Thumbnails { get; set; }
}

[PublicAPI]
public class Thumbnails
{
    [JsonPropertyName("high")] public High? High { get; set; }
}

[PublicAPI]
public class High
{
    [JsonPropertyName("url")] public Uri? Url { get; set; }
}