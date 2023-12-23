namespace Sudoku.Analytics.Configuration;

/// <summary>
/// Represents a type that encapsulates a list of options adjusted by users and used by <see cref="StepSearcher"/> instances.
/// Some options may not relate to a real <see cref="StepSearcher"/> instance directly, but relate to a <see cref="Step"/>
/// that a <see cref="StepSearcher"/> instance can create.
/// For example, setting notation to the coordinates.
/// </summary>
public sealed record StepSearcherOptions
{
	/// <summary>
	/// Indicates whether the step searchers will adjust the searching order to distinct two modes on displaying candidates,
	/// making the experience better.
	/// </summary>
	public bool DistinctDirectMode { get; init; } = false;

	/// <summary>
	/// Indicates whether the current solver uses direct mode to solve a puzzle, which means the UI will display the grid without any candidates.
	/// </summary>
	public bool IsDirectMode { get; init; } = false;

	/// <inheritdoc cref="CoordinateConverter"/>
	public CoordinateConverter Converter { get; init; } = new RxCyConverter();


	/// <summary>
	/// Represents a default option-provider instance.
	/// </summary>
	/// <remarks>
	/// This default option makes the internal members be:
	/// <list type="bullet">
	/// <item><see cref="Converter"/>: <see cref="RxCyConverter"/></item>
	/// <item><see cref="DistinctDirectMode"/>: <see langword="false"/></item>
	/// <item><see cref="IsDirectMode"/>: <see langword="false"/></item>
	/// </list>
	/// </remarks>
	public static StepSearcherOptions Default => new();
}
