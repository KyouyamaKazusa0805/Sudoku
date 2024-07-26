namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator type that can generate puzzles with complex single techniques.
/// </summary>
public abstract class ComplexSingleBaseGenerator : TechniqueGenerator, ITechniqueGenerator, IJustOneCellGenerator
{
	/// <summary>
	/// Indicates the backing analyzer.
	/// </summary>
	protected static readonly Analyzer Analyzer = Analyzer.Default
		.WithStepSearchers(
			new SingleStepSearcher { EnableFullHouse = true, HiddenSinglesInBlockFirst = true },
			new DirectIntersectionStepSearcher { AllowDirectClaiming = true, AllowDirectPointing = true },
			new DirectSubsetStepSearcher
			{
				AllowDirectHiddenSubset = true,
				AllowDirectLockedHiddenSubset = true,
				AllowDirectLockedSubset = true,
				AllowDirectNakedSubset = true,
				DirectHiddenSubsetMaxSize = 4,
				DirectNakedSubsetMaxSize = 4
			}
		)
		.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true });


	/// <inheritdoc/>
	public abstract TechniqueSet SupportedTechniques { get; }

	/// <summary>
	/// Indicates the creator instance that creates a list of cells indicating pattern interim cells.
	/// </summary>
	protected abstract FuncRefReadOnly<Grid, Step, CellMap> InterimCellsCreator { get; }

	/// <summary>
	/// Indicates the local step filter.
	/// </summary>
	protected abstract FuncRefReadOnly<Step, bool> LocalStepFilter { get; }


	/// <inheritdoc/>
	public sealed override Grid GenerateUnique(CancellationToken cancellationToken = default)
	{
		var generator = new Generator();
		while (true)
		{
			var puzzle = generator.Generate(cancellationToken: cancellationToken);
			if (puzzle.IsUndefined)
			{
				return Grid.Undefined;
			}

			switch (Analyzer.Analyze(in puzzle, cancellationToken: cancellationToken))
			{
				case { FailedReason: FailedReason.UserCancelled }:
				{
					return Grid.Undefined;
				}
				case { IsSolved: true, StepsSpan: var steps } when steps.Any(LocalStepFilter):
				{
					return puzzle;
				}
				default:
				{
					cancellationToken.ThrowIfCancellationRequested();
					break;
				}
			}
		}
	}


	/// <inheritdoc/>
	public Grid GenerateJustOneCell(CancellationToken cancellationToken = default)
		=> GenerateJustOneCell(out _, out _, cancellationToken);

	/// <inheritdoc/>
	public Grid GenerateJustOneCell(out Step? step, CancellationToken cancellationToken = default)
		=> GenerateJustOneCell(out _, out step, cancellationToken);

	/// <inheritdoc/>
	public Grid GenerateJustOneCell(out Grid phasedGrid, out Step? step, CancellationToken cancellationToken = default)
	{
		var generator = new Generator();
		while (true)
		{
			var puzzle = generator.Generate(cancellationToken: cancellationToken);
			switch (Analyzer.Analyze(in puzzle, cancellationToken: cancellationToken))
			{
				case { FailedReason: FailedReason.UserCancelled }:
				{
					(phasedGrid, step) = (Grid.Undefined, null);
					return Grid.Undefined;
				}
				case { IsSolved: true, StepsSpan: var steps, GridsSpan: var grids }:
				{
					var solvingSteps = StepMarshal.Combine(grids, steps);
					foreach (var (g, s) in solvingSteps)
					{
						if (LocalStepFilter(in s))
						{
							// Reserves the given cells that are used in the pattern.
							var reservedCells = InterimCellsCreator(in g, in s);
							var result = Grid.Empty;
							foreach (var cell in reservedCells)
							{
								result.SetDigit(cell, g.GetDigit(cell));
								result.SetState(cell, CellState.Given);
							}

							(phasedGrid, step) = (g, s);
							return result;
						}
					}
					goto default;
				}
				default:
				{
					cancellationToken.ThrowIfCancellationRequested();
					break;
				}
			}
		}
	}
}
