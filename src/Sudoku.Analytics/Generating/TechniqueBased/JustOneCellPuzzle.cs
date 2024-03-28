namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents the answer to a just-one-cell sudoku puzzle.
/// </summary>
/// <param name="Result">Indicates the generation result for the pattern.</param>
/// <param name="Puzzle">Indicates the generated puzzle.</param>
/// <param name="Cell">Indicates the target cell.</param>
/// <param name="Digit">Indicates the target digit.</param>
/// <param name="Step">
/// <para>Indiactes the step for the pattern.</para>
/// <para>
/// Assign a not-<see langword="null"/> value to this parameter
/// if argument <paramref name="Result"/> is <see cref="GeneratingResult.Success"/>.
/// Set the arguments of constructor <see cref="Step.Step(Conclusion[], View[], StepSearcherOptions)"/>
/// to be <c>[]</c>, <c>[]</c> and <c><see langword="new"/>()</c> respectively,
/// in order to avoid the potential bug on displaying details.
/// </para>
/// </param>
public sealed record JustOneCellPuzzle(GeneratingResult Result, scoped ref readonly Grid Puzzle, Cell Cell, Digit Digit, Step? Step) :
	PuzzleBase(Result, in Puzzle),
	ICultureFormattable
{
	/// <summary>
	/// Initializes a <see cref="JustOneCellPuzzle"/> instance, via the values.
	/// </summary>
	/// <param name="result">The generating result.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal JustOneCellPuzzle(GeneratingResult result) : this(result, in Grid.Undefined, -1, -1, null)
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
}
