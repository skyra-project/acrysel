using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Acrysel.Services.Entities;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Remora.Rest.Results;
using Remora.Results;

namespace Acrysel.Services;

public class YoutubeApiClient : IYoutubeApiClient
{
    private const string BaseUrl = "https://www.googleapis.com/youtube/v3/search?part=id&part=snippet&type=channel";
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public YoutubeApiClient(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = configuration.GetValue<string?>("YOUTUBE_API_KEY") ??
                  throw new InvalidOperationException("YOUTUBE_API_KEY environment variable not set.");
    }

    public async Task<Result<IEnumerable<YoutubeChannelDescriptor>>> SearchForChannelAsync(string nameQuery)
    {
        var fullQueryUrl = QueryHelpers.AddQueryString(BaseUrl, "q", nameQuery);

        var withToken = QueryHelpers.AddQueryString(fullQueryUrl, "key", _apiKey);

        var request = await _httpClient.GetAsync(withToken);

        if (!request.IsSuccessStatusCode)
        {
            var body = await request.Content.ReadAsStringAsync();
            return Result<IEnumerable<YoutubeChannelDescriptor>>.FromError(
                new HttpResultError(request.StatusCode, body));
        }

        var stream = await request.Content.ReadAsStreamAsync();

        var jsonBody = await JsonSerializer.DeserializeAsync<YoutubeChannelSearchResponse>(stream);

        return Result<IEnumerable<YoutubeChannelDescriptor>>.FromSuccess(jsonBody.Items.Select(item => item.Descriptor));
    }
}