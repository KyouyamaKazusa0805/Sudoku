namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator that supports generating for puzzles that can be solved by only using Full Houses.
/// </summary>
/// <seealso cref="Technique.FullHouse"/>
public sealed class FullHousePrimaryGenerator : IPrimaryGenerator, ITechniqueBasedGenerator
{
	/// <summary>
	/// Represents a seed array for cells that can be used in core methods.
	/// </summary>
	private static readonly Cell[] CellSeed = Enumerable.Range(0, 81).ToArray();

	/// <summary>
	/// Represents a seed array for houses that can be used in core methods.
	/// </summary>
	private static readonly House[] HouseSeed = Enumerable.Range(0, 27).ToArray();

	/// <summary>
	/// Represents a seed array for digits that can be used in core methods.
	/// </summary>
	private static readonly Digit[] DigitSeed = Enumerable.Range(0, 9).ToArray();

	/// <summary>
	/// Indicates the random number generator.
	/// </summary>
	private static readonly Random Rng = Random.Shared;

	/// <summary>
	/// Represents an analyzer.
	/// </summary>
	private static readonly Analyzer Analyzer = Analyzer.Default.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true });


	/// <inheritdoc/>
	public TechniqueSet SupportedTechniques => [Technique.FullHouse];


	/// <inheritdoc cref="ITechniqueBasedGenerator.GenerateUnique"/>
	public Grid GenerateUnique(Cell emptyCellsCount, CancellationToken cancellationToken = default)
	{
		if (emptyCellsCount is not (-1 or >= 1 and <= PeersCount + 1))
		{
			emptyCellsCount = Math.Clamp(emptyCellsCount, 1, PeersCount + 1);
		}

		var generator = new Generator();
		while (true)
		{
			// Try generating a solution.
			var grid = generator.Generate(cancellationToken: cancellationToken);
			if (grid.IsUndefined)
			{
				return Grid.Undefined;
			}

			// Replace with solution grid.
			grid = grid.GetSolutionGrid().UnfixedGrid;

			// Then randomly removed some digits in some cells, and keeps the grid valid.
			shuffleSequence(CellSeed);

			// Removes the selected cells.
			var pattern = CellSeed[..(emptyCellsCount == -1 ? Rng.Next(1, 22) : emptyCellsCount)].AsCellMap();
			foreach (var cell in pattern)
			{
				grid.SetDigit(cell, -1);
			}

			// Fix the grid and check validity.
			grid.Fix();
			if (grid.GetIsValid()
				&& Analyzer.Analyze(in grid, cancellationToken: cancellationToken) is { IsSolved: true, InterimSteps: var steps }
				&& new SortedSet<Technique>(from step in steps select step.Code).Max == Technique.FullHouse)
			{
				return grid.FixedGrid;
			}

			cancellationToken.ThrowIfCancellationRequested();
		}


		static void shuffleSequence<T>(T[] values)
		{
			Rng.Shuffle(values);
			Rng.Shuffle(values);
			Rng.Shuffle(values);
		}
	}

	/// <inheritdoc/>
	Grid ITechniqueBasedGenerator.GenerateUnique(CancellationToken cancellationToken) => GenerateUnique(21, cancellationToken);

	/// <inheritdoc/>
	Grid IPrimaryGenerator.GeneratePrimary(CancellationToken cancellationToken) => GenerateUnique(21, cancellationToken);
}
