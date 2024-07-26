namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator that supports generating for puzzles that can be solved by only using Direct Intersections.
/// </summary>
/// <seealso cref="Technique.PointingFullHouse"/>
/// <seealso cref="Technique.PointingCrosshatchingBlock"/>
/// <seealso cref="Technique.PointingCrosshatchingRow"/>
/// <seealso cref="Technique.PointingCrosshatchingColumn"/>
/// <seealso cref="Technique.PointingNakedSingle"/>
/// <seealso cref="Technique.ClaimingFullHouse"/>
/// <seealso cref="Technique.ClaimingCrosshatchingBlock"/>
/// <seealso cref="Technique.ClaimingCrosshatchingRow"/>
/// <seealso cref="Technique.ClaimingCrosshatchingColumn"/>
/// <seealso cref="Technique.ClaimingNakedSingle"/>
public sealed class DirectIntersectionGenerator : ComplexSingleBaseGenerator
{
	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques
		=> [
			Technique.PointingFullHouse,
			Technique.PointingCrosshatchingBlock,
			Technique.PointingCrosshatchingRow,
			Technique.PointingCrosshatchingColumn,
			Technique.PointingNakedSingle,
			Technique.ClaimingFullHouse,
			Technique.ClaimingCrosshatchingBlock,
			Technique.ClaimingCrosshatchingRow,
			Technique.ClaimingCrosshatchingColumn,
			Technique.ClaimingNakedSingle
		];

	/// <inheritdoc/>
	protected override FuncRefReadOnly<Step, bool> LocalStepFilter
		=> static (ref readonly Step step) => step is DirectIntersectionStep;


	/// <inheritdoc/>
	public override Grid GenerateJustOneCell(out Step? step, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public override Grid GenerateJustOneCell(out Grid phasedGrid, out Step? step, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
