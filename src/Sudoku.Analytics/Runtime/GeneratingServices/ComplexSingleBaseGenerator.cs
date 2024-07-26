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

			var analysisResult = Analyzer.Analyze(in puzzle, cancellationToken: cancellationToken);
			switch (analysisResult)
			{
				case { IsSolved: false, FailedReason: FailedReason.UserCancelled }:
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
	public abstract Grid GenerateJustOneCell(out Step? step, CancellationToken cancellationToken = default);

	/// <inheritdoc/>
	public abstract Grid GenerateJustOneCell(out Grid phasedGrid, out Step? step, CancellationToken cancellationToken = default);
}
