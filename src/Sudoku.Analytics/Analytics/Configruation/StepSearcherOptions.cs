using Sudoku.Text;

namespace Sudoku.Analytics.Configuration;

/// <summary>
/// Represents a type that encapsulates a list of options adjusted by users and used by <see cref="StepSearcher"/> instances.
/// Some options may not relate to a real <see cref="StepSearcher"/> instance directly, but relate to a <see cref="Step"/>
/// that a <see cref="StepSearcher"/> instance can create.
/// For example, setting notation to the coordinates.
/// </summary>
public sealed class StepSearcherOptions
{
	/// <summary>
	/// Represents a default option-provider instance.
	/// </summary>
	/// <remarks>
	/// This default option makes the internal members be:
	/// <list type="bullet">
	/// <item><see cref="CoordinateConverter"/>: <see cref="RxCyConverter"/></item>
	/// </list>
	/// </remarks>
	public static StepSearcherOptions Default => new();


	/// <inheritdoc cref="Text.CoordinateConverter"/>
	/// <remarks>By default, the value is an instance of type <see cref="RxCyConverter"/>.</remarks>
	public CoordinateConverter CoordinateConverter { get; set; } = new RxCyConverter();
}
