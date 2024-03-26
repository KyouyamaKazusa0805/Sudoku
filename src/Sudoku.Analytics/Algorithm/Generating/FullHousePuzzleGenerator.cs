namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a type that generates puzzles that only contains full house usages.
/// </summary>
public sealed class FullHousePuzzleGenerator :
	TechniqueBasedPuzzleGenerator,
	IElementaryTechniqueBasedPuzzleGenerator<FullHousePuzzleGenerator>
{
	/// <summary>
	/// Represents a seed array that can be used in the following methods.
	/// </summary>
	private static readonly Cell[] Seed = Enumerable.Range(0, 81).ToArray();

	/// <summary>
	/// Represents an analyzer.
	/// </summary>
	private static readonly Analyzer Analyzer = new() { StepSearchers = [new SingleStepSearcher { EnableFullHouse = true }] };


	/// <inheritdoc/>
	/// <remarks>
	/// The type is special: this property is <see langword="true"/> by default, because all puzzles contain full houses.
	/// There is no need implementing generalized logic for this technique.
	/// </remarks>
	public bool CanOnlyUseThisTechnique { get; set; } = true;

	/// <summary>
	/// <para>Indicates the number of empty cells that generated puzzles will be used.</para>
	/// <para>
	/// The value can be all possible integers between -1 and 21, without 0.
	/// If the value is -1, all possible number of empty cells in a puzzle can be tried; otherwise set a value between 1 and 21.
	/// </para>
	/// </summary>
	public int EmptyCellsCount { get; set; } = -1;

	/// <inheritdoc/>
	public override Technique SupportedTechnique => Technique.FullHouse;


	/// <inheritdoc/>
	public override bool TryGenerateUnique(out Grid result, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
		=> CanOnlyUseThisTechnique
			? TryGenerateUniqueOnlyThis(out result, progress, cancellationToken)
			: ReturnDefault(out result); // All puzzles contain full houses; unmeaningful to define handling logic.

	/// <inheritdoc/>
	public override bool TryGenerateOnlyOneCell(out Grid result, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
		=> ReturnDefault(out result);

	/// <summary>
	/// The core method that creates a <see cref="Grid"/> that can be solved via only this technique.
	/// </summary>
	/// <param name="result">The result grid.</param>
	/// <param name="progress">The progress object.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the process is successfully-executed.</returns>
	private bool TryGenerateUniqueOnlyThis(out Grid result, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
	{
		EmptyCellsCount = Math.Clamp(EmptyCellsCount, 1, 21);

		for (var i = 1; ; i++)
		{
			// Try generating a solution.
			var grid = new HodokuPuzzleGenerator().Generate(cancellationToken: cancellationToken);
			if (grid.IsUndefined)
			{
				result = grid;
				return false;
			}

			// Replace with solution grid.
			grid = grid.SolutionGrid.UnfixedGrid;

			// Then randomly removed some digits in some cells, and keeps the grid valid.
			for (var times = 0; times < 3; times++)
			{
				Rng.Shuffle(Seed);
			}

			// Removes the selected cells.
			var pattern = (CellMap)Seed[..(EmptyCellsCount == -1 ? Rng.Next(1, 22) : EmptyCellsCount)];
			foreach (var cell in pattern)
			{
				grid.SetDigit(cell, -1);
			}

			// Fix the grid and check validity.
			grid.Fix();
			if (grid.IsValid
				&& Analyzer.Analyze(in grid, cancellationToken: cancellationToken) is { IsSolved: true, Steps: var steps }
				&& new SortedSet<Technique>(from step in steps select step.Code).Max == Technique.FullHouse)
			{
				// Check validity of the puzzle.
				result = grid.FixedGrid;
				return true;
			}

			progress?.Report(i);
			cancellationToken.ThrowIfCancellationRequested();
		}
	}
}
