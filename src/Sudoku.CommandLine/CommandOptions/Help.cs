namespace Sudoku.CommandLine.CommandOptions;

/// <summary>
/// Defines the type that stores the help options.
/// </summary>
public sealed class Help : IHelpCommand
{
	/// <inheritdoc/>
	public static string Name => "help";

	/// <inheritdoc/>
	public static string Description => "Displays all possible root commands provided.";

	/// <inheritdoc/>
	public static string[] SupportedCommands => new[] { "help", "?" };

	/// <inheritdoc/>
	public static IEnumerable<(string CommandLine, string Meaning)>? UsageCommands =>
		new[] { ("""help""", "Lists the help information.") };


	/// <inheritdoc/>
	public void Execute()
	{
		var commandTypes = typeof(Help).Assembly.GetDerivedTypes(typeof(IRootCommand));

		// TODO: Implement the displaying logic.
	}
}
