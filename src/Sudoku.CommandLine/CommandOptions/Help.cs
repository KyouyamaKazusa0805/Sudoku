namespace Sudoku.CommandLine.CommandOptions;

/// <summary>
/// Defines the type that stores the help options.
/// </summary>
public sealed class Help : IHelpCommand<ErrorCode>
{
	/// <inheritdoc/>
	public static string Name => "help";

	/// <inheritdoc/>
	public static string Description => "Displays all possible root commands provided.";

	/// <inheritdoc/>
	public static string[] SupportedCommands => new[] { "help", "?" };

	/// <inheritdoc/>
	public static IEnumerable<IRootCommand<ErrorCode>>? UsageCommands => new[] { new Help() };


	/// <inheritdoc/>
	public ErrorCode Execute()
	{
		var commandTypes =
			from type in typeof(Help).Assembly.GetTypes()
			where type.IsAssignableTo(typeof(IRootCommand<ErrorCode>))
			select type;

		// TODO: Implement the displaying logic.

		return ErrorCode.None;
	}
}
