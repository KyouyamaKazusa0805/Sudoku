namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents the answer to a just-one-cell sudoku puzzle.
/// </summary>
/// <param name="cell">Indicates the target cell.</param>
/// <param name="digit">Indicates the target digit.</param>
/// <param name="step">
/// <para>Indiactes the step for the pattern.</para>
/// <para>
/// Assign a not-<see langword="null"/> value to this parameter
/// if argument <see cref="PuzzleBase.Result"/> is <see cref="GeneratingResult.Success"/>.
/// Set the arguments of constructor <see cref="Step.Step(Conclusion[], View[], StepSearcherOptions)"/>
/// to be <c>[]</c>, <c>[]</c> and <c><see langword="new"/>()</c> respectively,
/// in order to avoid the potential bug on displaying details.
/// </para>
/// </param>
[GetHashCode]
public sealed partial class JustOneCellPuzzle(
	[PrimaryConstructorParameter] Cell cell,
	[PrimaryConstructorParameter] Digit digit,
	[PrimaryConstructorParameter] Step? step
) :
	PuzzleBase,
	ICultureFormattable
{
	/// <summary>
	/// Initializes a <see cref="JustOneCellPuzzle"/> instance.
	/// </summary>
	public JustOneCellPuzzle() : this(-1, -1, null)
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => ToString(null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CultureInfo? culture = null)
	{
		if (Step is null)
		{
			return string.Empty;
		}

		var str = $"{Step.ToString(culture)}";
		return str[..(str.IndexOf(str.Match("""=>[\s\S]*$""")!) + 1)];
	}


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] PuzzleBase? other)
		=> other is JustOneCellPuzzle comparer
		&& (Result, Puzzle, Cell, Digit, Step) == (comparer.Result, comparer.Puzzle, comparer.Cell, comparer.Digit, comparer.Step);
}
