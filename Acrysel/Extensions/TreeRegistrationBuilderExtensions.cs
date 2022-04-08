using System.Linq;
using System.Reflection;
using Remora.Commands.DependencyInjection;
using Remora.Commands.Groups;

namespace Acrysel.Extensions;

public static class TreeRegistrationBuilderExtensions
{
    public static TreeRegistrationBuilder AddCommandsFromAssembly(this TreeRegistrationBuilder builder)
    {
        var allCommands = Assembly.GetExecutingAssembly()
            .ExportedTypes
            .Where(type => type.IsClass && type.IsAssignableTo(typeof(CommandGroup)))
            .ToArray();

        if (allCommands.Length > 0)
        {
            foreach (var command in allCommands)
            {
                builder.WithCommandGroup(command);
            }
        }

        return builder;
    }
}