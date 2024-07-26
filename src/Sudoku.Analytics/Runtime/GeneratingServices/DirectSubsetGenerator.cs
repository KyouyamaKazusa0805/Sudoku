namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator that supports generating for puzzles that can be solved by only using Direct Subsets.
/// </summary>
public sealed class DirectSubsetGenerator : ComplexSingleBaseGenerator
{
	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques
		=> [
			Technique.NakedPairFullHouse, Technique.NakedPairCrosshatchingBlock, Technique.NakedPairCrosshatchingRow,
			Technique.NakedPairCrosshatchingColumn, Technique.NakedPairNakedSingle,
			Technique.NakedTripleFullHouse, Technique.NakedTripleCrosshatchingBlock, Technique.NakedTripleCrosshatchingRow,
			Technique.NakedTripleCrosshatchingColumn, Technique.NakedTripleNakedSingle,
			Technique.NakedQuadrupleFullHouse, Technique.NakedQuadrupleCrosshatchingBlock, Technique.NakedQuadrupleCrosshatchingRow,
			Technique.NakedQuadrupleCrosshatchingColumn, Technique.NakedQuadrupleNakedSingle,
			Technique.HiddenPairFullHouse, Technique.HiddenPairCrosshatchingBlock, Technique.HiddenPairCrosshatchingRow,
			Technique.HiddenPairCrosshatchingColumn, Technique.HiddenPairNakedSingle,
			Technique.HiddenTripleFullHouse, Technique.HiddenTripleCrosshatchingBlock, Technique.HiddenTripleCrosshatchingRow,
			Technique.HiddenTripleCrosshatchingColumn, Technique.HiddenTripleNakedSingle,
			Technique.HiddenQuadrupleFullHouse, Technique.HiddenQuadrupleCrosshatchingBlock, Technique.HiddenQuadrupleCrosshatchingRow,
			Technique.HiddenQuadrupleCrosshatchingColumn, Technique.HiddenQuadrupleNakedSingle,
			Technique.LockedPairFullHouse, Technique.LockedPairCrosshatchingBlock, Technique.LockedPairCrosshatchingRow,
			Technique.LockedPairCrosshatchingColumn, Technique.LockedPairNakedSingle,
			Technique.LockedTripleFullHouse, Technique.LockedTripleCrosshatchingBlock, Technique.LockedTripleCrosshatchingRow,
			Technique.LockedTripleCrosshatchingColumn, Technique.LockedTripleNakedSingle,
			Technique.LockedHiddenPairFullHouse, Technique.LockedHiddenPairCrosshatchingBlock, Technique.LockedHiddenPairCrosshatchingRow,
			Technique.LockedHiddenPairCrosshatchingColumn, Technique.LockedHiddenPairNakedSingle,
			Technique.LockedHiddenTripleFullHouse, Technique.LockedHiddenTripleCrosshatchingBlock, Technique.LockedHiddenTripleCrosshatchingRow,
			Technique.LockedHiddenTripleCrosshatchingColumn, Technique.LockedHiddenTripleNakedSingle
		];

	/// <inheritdoc/>
	protected override FuncRefReadOnly<Grid, Step, CellMap> InterimCellsCreator
		=> static (ref readonly Grid _, ref readonly Step _) => CellMap.Empty;

	/// <inheritdoc/>
	protected override FuncRefReadOnly<Step, bool> LocalStepFilter => static (ref readonly Step step) => step is DirectSubsetStep;
}
