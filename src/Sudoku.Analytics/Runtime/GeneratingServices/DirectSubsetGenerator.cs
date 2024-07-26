namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator that supports generating for puzzles that can be solved by only using Direct Subsets.
/// </summary>
/// <seealso cref="Technique.NakedPairFullHouse"/>
/// <seealso cref="Technique.NakedPairCrosshatchingBlock"/>
/// <seealso cref="Technique.NakedPairCrosshatchingRow"/>
/// <seealso cref="Technique.NakedPairCrosshatchingColumn"/>
/// <seealso cref="Technique.NakedPairNakedSingle"/>
/// <seealso cref="Technique.NakedTripleFullHouse"/>
/// <seealso cref="Technique.NakedTripleCrosshatchingBlock"/>
/// <seealso cref="Technique.NakedTripleCrosshatchingRow"/>
/// <seealso cref="Technique.NakedTripleCrosshatchingColumn"/>
/// <seealso cref="Technique.NakedTripleNakedSingle"/>
/// <seealso cref="Technique.NakedQuadrupleFullHouse"/>
/// <seealso cref="Technique.NakedQuadrupleCrosshatchingBlock"/>
/// <seealso cref="Technique.NakedQuadrupleCrosshatchingRow"/>
/// <seealso cref="Technique.NakedQuadrupleCrosshatchingColumn"/>
/// <seealso cref="Technique.NakedQuadrupleNakedSingle"/>
/// <seealso cref="Technique.HiddenPairFullHouse"/>
/// <seealso cref="Technique.HiddenPairCrosshatchingBlock"/>
/// <seealso cref="Technique.HiddenPairCrosshatchingRow"/>
/// <seealso cref="Technique.HiddenPairCrosshatchingColumn"/>
/// <seealso cref="Technique.HiddenPairNakedSingle"/>
/// <seealso cref="Technique.HiddenTripleFullHouse"/>
/// <seealso cref="Technique.HiddenTripleCrosshatchingBlock"/>
/// <seealso cref="Technique.HiddenTripleCrosshatchingRow"/>
/// <seealso cref="Technique.HiddenTripleCrosshatchingColumn"/>
/// <seealso cref="Technique.HiddenTripleNakedSingle"/>
/// <seealso cref="Technique.HiddenQuadrupleFullHouse"/>
/// <seealso cref="Technique.HiddenQuadrupleCrosshatchingBlock"/>
/// <seealso cref="Technique.HiddenQuadrupleCrosshatchingRow"/>
/// <seealso cref="Technique.HiddenQuadrupleCrosshatchingColumn"/>
/// <seealso cref="Technique.HiddenQuadrupleNakedSingle"/>
/// <seealso cref="Technique.LockedPairFullHouse"/>
/// <seealso cref="Technique.LockedPairCrosshatchingBlock"/>
/// <seealso cref="Technique.LockedPairCrosshatchingRow"/>
/// <seealso cref="Technique.LockedPairCrosshatchingColumn"/>
/// <seealso cref="Technique.LockedPairNakedSingle"/>
/// <seealso cref="Technique.LockedTripleFullHouse"/>
/// <seealso cref="Technique.LockedTripleCrosshatchingBlock"/>
/// <seealso cref="Technique.LockedTripleCrosshatchingRow"/>
/// <seealso cref="Technique.LockedTripleCrosshatchingColumn"/>
/// <seealso cref="Technique.LockedTripleNakedSingle"/>
/// <seealso cref="Technique.LockedHiddenPairFullHouse"/>
/// <seealso cref="Technique.LockedHiddenPairCrosshatchingBlock"/>
/// <seealso cref="Technique.LockedHiddenPairCrosshatchingRow"/>
/// <seealso cref="Technique.LockedHiddenPairCrosshatchingColumn"/>
/// <seealso cref="Technique.LockedHiddenPairNakedSingle"/>
/// <seealso cref="Technique.LockedHiddenTripleFullHouse"/>
/// <seealso cref="Technique.LockedHiddenTripleCrosshatchingBlock"/>
/// <seealso cref="Technique.LockedHiddenTripleCrosshatchingRow"/>
/// <seealso cref="Technique.LockedHiddenTripleCrosshatchingColumn"/>
/// <seealso cref="Technique.LockedHiddenTripleNakedSingle"/>
public sealed class DirectSubsetGenerator : ComplexSingleBaseGenerator
{
	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques
		=> [
			Technique.NakedPairFullHouse,
			Technique.NakedPairCrosshatchingBlock,
			Technique.NakedPairCrosshatchingRow,
			Technique.NakedPairCrosshatchingColumn,
			Technique.NakedPairNakedSingle,
			Technique.NakedTripleFullHouse,
			Technique.NakedTripleCrosshatchingBlock,
			Technique.NakedTripleCrosshatchingRow,
			Technique.NakedTripleCrosshatchingColumn,
			Technique.NakedTripleNakedSingle,
			Technique.NakedQuadrupleFullHouse,
			Technique.NakedQuadrupleCrosshatchingBlock,
			Technique.NakedQuadrupleCrosshatchingRow,
			Technique.NakedQuadrupleCrosshatchingColumn,
			Technique.NakedQuadrupleNakedSingle,
			Technique.HiddenPairFullHouse,
			Technique.HiddenPairCrosshatchingBlock,
			Technique.HiddenPairCrosshatchingRow,
			Technique.HiddenPairCrosshatchingColumn,
			Technique.HiddenPairNakedSingle,
			Technique.HiddenTripleFullHouse,
			Technique.HiddenTripleCrosshatchingBlock,
			Technique.HiddenTripleCrosshatchingRow,
			Technique.HiddenTripleCrosshatchingColumn,
			Technique.HiddenTripleNakedSingle,
			Technique.HiddenQuadrupleFullHouse,
			Technique.HiddenQuadrupleCrosshatchingBlock,
			Technique.HiddenQuadrupleCrosshatchingRow,
			Technique.HiddenQuadrupleCrosshatchingColumn,
			Technique.HiddenQuadrupleNakedSingle,
			Technique.LockedPairFullHouse,
			Technique.LockedPairCrosshatchingBlock,
			Technique.LockedPairCrosshatchingRow,
			Technique.LockedPairCrosshatchingColumn,
			Technique.LockedPairNakedSingle,
			Technique.LockedTripleFullHouse,
			Technique.LockedTripleCrosshatchingBlock,
			Technique.LockedTripleCrosshatchingRow,
			Technique.LockedTripleCrosshatchingColumn,
			Technique.LockedTripleNakedSingle,
			Technique.LockedHiddenPairFullHouse,
			Technique.LockedHiddenPairCrosshatchingBlock,
			Technique.LockedHiddenPairCrosshatchingRow,
			Technique.LockedHiddenPairCrosshatchingColumn,
			Technique.LockedHiddenPairNakedSingle,
			Technique.LockedHiddenTripleFullHouse,
			Technique.LockedHiddenTripleCrosshatchingBlock,
			Technique.LockedHiddenTripleCrosshatchingRow,
			Technique.LockedHiddenTripleCrosshatchingColumn,
			Technique.LockedHiddenTripleNakedSingle
		];

	/// <inheritdoc/>
	protected override FuncRefReadOnly<Step, bool> LocalStepFilter => static (ref readonly Step step) => step is DirectSubsetStep;


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
