namespace Sudoku.CommandLine.CommandOptions;

/// <summary>
/// Represents a check command.
/// </summary>
public sealed class Check : IRootCommand
{
	/// <summary>
	/// Indicates the check type.
	/// </summary>
	[Command('t', "type", "Indicates what kind of attribute will be checked.")]
	[CommandConverter(typeof(EnumTypeConverter<CheckType>))]
	public CheckType CheckType { get; set; } = CheckType.Validity;

	/// <summary>
	/// Indicates the grid used.
	/// </summary>
	[Command('g', "grid", "Indicates the sudoku grid as string representation.")]
	[CommandConverter(typeof(GridConverter))]
	public Grid Grid { get; set; }

	/// <inheritdoc/>
	public static string Name => "check";

	/// <inheritdoc/>
	public static string Description => "To check the attributes for a sudoku grid.";

	/// <inheritdoc/>
	public static string[] SupportedCommands => new[] { "check" };

	/// <inheritdoc/>
	public static IEnumerable<(string CommandLine, string Meaning)>? UsageCommands =>
		new[]
		{
			(
				"""
				check -g "...892.....2...3..75.....69.359.814...........713.659.96.....21..4...6.....621..." -t validity
				""",
				"To check the validity of the specified sudoku grid."
			)
		};


	/// <inheritdoc/>
	public void Execute()
	{
		switch (CheckType)
		{
			case CheckType.Validity:
			{
				ConsoleExtensions.WriteLine(
					$"""
					Puzzle: '{Grid:#}'
					The puzzle {(Grid.IsValid ? "has" : "doesn't have")} a unique solution.
					"""
				);

				return;
			}
			default:
			{
				throw new CommandLineException((int)ErrorCode.ArgAttributeNameIsInvalid);
			}
		}
	}
}
