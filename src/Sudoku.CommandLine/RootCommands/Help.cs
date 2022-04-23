namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Defines the type that stores the help options.
/// </summary>
[RootCommand("help", "Displays all possible root commands provided.", IsSpecial = true)]
[SupportedArguments(new[] { "help", "?" })]
[Usage("help", IsFact = true)]
public sealed class Help : IExecutable
{
	/// <inheritdoc/>
	public void Execute()
	{
		if (typeof(Version).Assembly.GetName() is not { Name: { } realName, Version: var version })
		{
			// Returns an error that cannot fetch the assembly name correctly.
			throw new CommandLineRuntimeException((int)ErrorCode.AssemblyNameIsNull);
		}

		// Iterates on each command type, to get the maximum length of the command, in order to display
		// the commands alignedly.
		var commandTypes = typeof(Help).Assembly.GetDerivedTypes(typeof(IExecutable)).ToArray();
		int maxWidth = 0;
		var listOfDescriptionParts = new List<(string CommandName, IEnumerable<string> DescriptionRawParts)>();
		foreach (var commandType in commandTypes)
		{
			var commandAttribute = commandType.GetCustomAttribute<RootCommandAttribute>()!;

			string commandName = commandAttribute.Name;
			maxWidth = Max(commandName.Length, maxWidth);

			var parts = commandAttribute.Description.SplitByLength(Console.LargestWindowWidth);

			listOfDescriptionParts.Add((commandName, parts));
		}

		// Defines a builder that stores the content in order to output.
		var helpTextContentBuilder = new StringBuilder($"Project {realName}\r\nVersion {version}")
			.AppendLine()
			.AppendLine();

		// TODO: Output usage.

		// Build the content.
		helpTextContentBuilder.AppendLine("Commands:").AppendLine();
		foreach (var (commandName, parts) in listOfDescriptionParts)
		{
			helpTextContentBuilder
				.Append(commandName.PadLeft(maxWidth + 1))
				.Append(new string(' ', 3))
				.AppendLine(string.Join($"\r\n{new string(' ', 3 + maxWidth)}", parts))
				.AppendLine();
		}

		// Output the value.
		Terminal.Write(helpTextContentBuilder.ToString());
	}
}
