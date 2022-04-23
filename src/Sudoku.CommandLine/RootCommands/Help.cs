namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Defines the type that stores the help options.
/// </summary>
[Usage("help", IsFact = true)]
public sealed class Help : IHelpCommand
{
	/// <inheritdoc/>
	public static string Name => "help";

	/// <inheritdoc/>
	public static string Description => "Displays all possible root commands provided.";

	/// <inheritdoc/>
	public static string[] SupportedCommands => new[] { "help", "?" };


	/// <inheritdoc/>
	public void Execute()
	{
		const BindingFlags staticProp = BindingFlags.Static | BindingFlags.Public;

		if (typeof(Version).Assembly.GetName() is not { Name: { } realName, Version: var version })
		{
			// Returns an error that cannot fetch the assembly name correctly.
			throw new CommandLineRuntimeException((int)ErrorCode.AssemblyNameIsNull);
		}

		// Iterates on each command type, to get the maximum length of the command, in order to display
		// the commands alignedly.
		var commandTypes = typeof(Help).Assembly.GetDerivedTypes(typeof(IRootCommand)).ToArray();
		int maxWidth = 0;
		var listOfDescriptionParts = new List<(string CommandName, IEnumerable<string> DescriptionRawParts)>();
		foreach (var commandType in commandTypes)
		{
			string commandName = (string)commandType.GetProperty(nameof(IRootCommand.Name), staticProp)!.GetValue(null)!;
			maxWidth = Max(commandName.Length, maxWidth);

			string description = (string)commandType.GetProperty(nameof(IRootCommand.Description), staticProp)!.GetValue(null)!;
			var parts = description.SplitByLength(Console.LargestWindowWidth);

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
