using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Results;

namespace Acrysel.Commands;

public class TestCommandGroup : CommandGroup
{
    private readonly ICommandContext _context;
    private readonly FeedbackService _feedbackService;

    public TestCommandGroup(ICommandContext context, FeedbackService feedbackService)
    {
        _context = context;
        _feedbackService = feedbackService;
    }

    [Command("hello")]
    [Description("Say hello!")]
    [Ephemeral]
    public async Task<IResult> SayHelloAsync([Description("Your name.")] string name)
    {
        var message = $"Hello, {name}! How are you?";

        var feedbackMessage = new FeedbackMessage(message, Color.Firebrick);

        var reply = await _feedbackService.SendContextualMessageAsync(feedbackMessage);

        return reply.IsSuccess
            ? Result.FromSuccess()
            : Result.FromError(reply);
    }
}