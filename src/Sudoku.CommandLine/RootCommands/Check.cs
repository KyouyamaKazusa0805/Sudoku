namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a check command.
/// </summary>
[SupportedArguments(new[] { "check" })]
[Usage("check --grid <grid> --type <type>", IsFact = true)]
[Usage($"""check -g "{SampleGrid}" -t validity""", "To check the validity of the specified sudoku grid.")]
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
	[Command('g', "grid", "Indicates the sudoku grid as string representation.", IsRequired = true)]
	[CommandConverter(typeof(GridConverter))]
	public Grid Grid { get; set; }

	/// <inheritdoc/>
	public static string Name => "check";

	/// <inheritdoc/>
	public static string Description => "To check the attributes for a sudoku grid.";


	/// <inheritdoc/>
	public void Execute()
	{
		switch (CheckType)
		{
			case CheckType.Validity:
			{
				Terminal.WriteLine(
					$"""
					Puzzle: '{Grid:#}'
					The puzzle {(Grid.IsValid ? "has" : "doesn't have")} a unique solution.
					"""
				);

				return;
			}
			default:
			{
				throw new CommandLineRuntimeException((int)ErrorCode.ArgAttributeNameIsInvalid);
			}
		}
	}
}
