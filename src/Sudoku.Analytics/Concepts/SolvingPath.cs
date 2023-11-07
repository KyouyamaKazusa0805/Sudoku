using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Analytics;

namespace Sudoku.Concepts;

using Pair = (Grid SteppingGrid, Step Step);

/// <summary>
/// Represents a solving path.
/// </summary>
[Equals(EqualsBehavior.ThrowNotSupportedException)]
[GetHashCode(GetHashCodeBehavior.ThrowNotSupportedException)]
public readonly ref partial struct SolvingPath
{
	/// <summary>
	/// The reference to the first of stepping grids.
	/// </summary>
	private readonly Grid[] _steppingGridFirst;

	/// <summary>
	/// The reference to the first of steps.
	/// </summary>
	private readonly Step[] _stepsFirst;


	/// <summary>
	/// Initializes a <see cref="SolvingPath"/> instance.
	/// </summary>
	/// <param name="steppingGrids">The stepping grids.</param>
	/// <param name="steps">The steps.</param>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the length of arguments are not equal.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SolvingPath(Grid[] steppingGrids, Step[] steps)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(steppingGrids.Length, steps.Length);

		_steppingGridFirst = steppingGrids;
		_stepsFirst = steps;
		(Length, IsSolved) = (steppingGrids.Length, true);
	}


	/// <summary>
	/// Indicates whether the puzzle has been solved.
	/// </summary>
	public bool IsSolved { get; }

	/// <summary>
	/// Indicates the number of elements stored in this collection.
	/// </summary>
	public int Length { get; }

	/// <summary>
	/// Indicates the stepping grids.
	/// </summary>
	public ReadOnlySpan<Grid> SteppingGrids => _steppingGridFirst;

	/// <summary>
	/// Indicates the steps.
	/// </summary>
	public ReadOnlySpan<Step> Steps => _stepsFirst;

	/// <summary>
	/// Indicates the internal pairs.
	/// </summary>
	internal ReadOnlySpan<Pair> Pairs
	{
		get
		{
			var result = new Pair[Length];
			for (var i = 0; i < SteppingGrids.Length; i++)
			{
				result[i] = (SteppingGrids[i], Steps[i]);
			}
			return result;
		}
	}


	/// <summary>
	/// Try to fetch the pair of stepping grid and step at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The reference to the pair of stepping grid and step.</returns>
	public ref readonly Pair this[int index] => ref Pairs[index];


	/// <summary>
	/// Try to fetch the stepping grid at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The reference to the stepping grid at the specified index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref readonly Grid SteppingGridAt(int index) => ref SteppingGrids[index];

	/// <summary>
	/// Gets an enumerator instance to iterate on each pair of stepping grids and actual step.
	/// </summary>
	/// <returns>An enumerator instance that iterates on each pair of stepping grid and actual step.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<Pair>.Enumerator GetEnumerator() => Pairs.GetEnumerator();

	/// <summary>
	/// Try to enumerate all steps.
	/// </summary>
	/// <returns>An enumerator instance that iterates on each stepping grid.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<Grid>.Enumerator EnumerateSteppingGrids() => SteppingGrids.GetEnumerator();

	/// <summary>
	/// Try to enumerate all steps.
	/// </summary>
	/// <returns>An enumerator instance that iterates on each steps.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<Step>.Enumerator EnumerateSteps() => Steps.GetEnumerator();

	/// <summary>
	/// Converts the current list into an array.
	/// </summary>
	/// <returns>An array of pairs of stepping grid and step.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Pair[] ToArray() => [.. Pairs];

	/// <summary>
	/// Try to fetch the step at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The step at the specified index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Step StepAt(int index) => Steps[index];

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
		=> $$"""{{nameof(SolvingPath)}} { {{nameof(SteppingGrids)}}.{{nameof(Array.Length)}} = {{SteppingGrids.Length}}, {{nameof(Steps)}}.{{nameof(Array.Length)}} = {{Steps.Length}} }""";


	/// <summary>
	/// Compares the reference of two instances, returning a <see cref="bool"/> value indicating whether they reference a same memory block.
	/// </summary>
	/// <param name="left">The left-side value to be determined.</param>
	/// <param name="right">The right-side value to be determined.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(SolvingPath left, SolvingPath right)
		=> ReferenceEquals(left._steppingGridFirst, right._steppingGridFirst)
		&& ReferenceEquals(left._stepsFirst, right._stepsFirst);

	/// <summary>
	/// Compares the reference of two instances,
	/// returning a <see cref="bool"/> value indicating whether they don't reference a same memory block.
	/// </summary>
	/// <param name="left">The left-side value to be determined.</param>
	/// <param name="right">The right-side value to be determined.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(SolvingPath left, SolvingPath right) => !(left == right);
}
