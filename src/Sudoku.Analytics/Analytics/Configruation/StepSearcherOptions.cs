using System.Runtime.CompilerServices;
using Sudoku.Text.Coordinate;

namespace Sudoku.Analytics.Configuration;

/// <summary>
/// Represents a type that encapsulates a list of options adjusted by users and used by <see cref="StepSearcher"/> instances.
/// Some options may not relate to a real <see cref="StepSearcher"/> instance directly, but relate to a <see cref="Step"/>
/// that a <see cref="StepSearcher"/> instance can create.
/// For example, setting notation to the coordinates.
/// </summary>
/// <param name="Converter">
/// <para><inheritdoc cref="CoordinateConverter" path="/summary"/></para>
/// <para><inheritdoc cref="CoordinateConverter" path="/remarks"/></para>
/// </param>
public sealed record StepSearcherOptions(CoordinateConverter Converter)
{
	/// <summary>
	/// Initializes a <see cref="StepSearcherOptions"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StepSearcherOptions() : this(new RxCyConverter())
	{
	}


	/// <summary>
	/// Represents a default option-provider instance.
	/// </summary>
	/// <remarks>
	/// This default option makes the internal members be:
	/// <list type="bullet">
	/// <item><see cref="Converter"/>: <see cref="RxCyConverter"/></item>
	/// </list>
	/// </remarks>
	public static StepSearcherOptions Default => new();
}
