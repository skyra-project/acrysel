using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Acrysel.Services;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Results;

namespace Acrysel.Commands;

public class YoutubeCommandGroup : CommandGroup
{
    private readonly ICommandContext _context;
    private readonly FeedbackService _feedbackService;
    private readonly IYoutubeApiClient _youtubeApiClient;

    public YoutubeCommandGroup(IYoutubeApiClient youtubeApiClient, FeedbackService feedbackService,
        ICommandContext context)
    {
        _youtubeApiClient = youtubeApiClient;
        _feedbackService = feedbackService;
        _context = context;
    }

    [Command("subscribe")]
    [Description("subscribe to a channel")]
    [Ephemeral]
    public async Task<IResult> SubscribeAsync([Description("The channel you would like to search for.")] string query)
    {
        var searchResult = await _youtubeApiClient.SearchForChannelAsync(query);

        if (!searchResult.IsSuccess)
        {
            return Result.FromError(searchResult.Error);
        }

        var embed = new Embed(Description: "Select which channel to subscribe to below.");

        var options = new FeedbackMessageOptions(MessageComponents: new IMessageComponent[]
            {
                new ActionRowComponent(new[]
                {
                    new SelectMenuComponent("ChannelSelector",
                        searchResult
                            .Entity
                            .Select(result =>
                            {
                                var createdAt = $" (Created At: {result.CreatedAt})";

                                var description = $"{result.Description}";

                                if (description.Length + createdAt.Length >= 100)
                                {
                                    description = description[..(99 - createdAt.Length)];
                                }

                                description += createdAt;

                                var length = description.Length;

                                return new SelectOption(result.ChannelTitle, result.Id, description);
                            }).ToArray())
                })
            }
        );

        var embedResult = await _feedbackService.SendContextualEmbedAsync(embed, options);

        if (!embedResult.IsSuccess)
        {
            return Result.FromError(embedResult.Error);
        }

        return Result.FromSuccess();
    }
}