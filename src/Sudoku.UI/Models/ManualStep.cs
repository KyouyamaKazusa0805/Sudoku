namespace Sudoku.UI.Models;

/// <summary>
/// Defines a manual step.
/// </summary>
[AutoDeconstruction(nameof(Grid), nameof(Step))]
public sealed partial class ManualStep
{
	/// <summary>
	/// Indicates the current grid used.
	/// </summary>
	public required Grid Grid { get; set; }

	/// <summary>
	/// Indicates the step.
	/// </summary>
	public required IStep Step { get; set; }


	/// <summary>
	/// Gets the display string value that can describe the main information of the current step.
	/// </summary>
	/// <returns>The string value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToDisplayString()
	{
		string openBrace = R["Token_OpenBrace"]!;
		string diffStr = R["DifficultyRating"]!;
		string closedBrace = R["Token_ClosedBrace"]!;
		return $"{openBrace}{diffStr} {Step.Difficulty:0.0}{closedBrace} {Step.ToSimpleString()}";
	}
}
