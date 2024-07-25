namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator type that can generate puzzles with complex single techniques.
/// </summary>
public abstract class ComplexSingleGenerator : TechniqueGenerator, ITechniqueGenerator, IJustOneCellGenerator
{
	/// <inheritdoc/>
	public abstract TechniqueSet SupportedTechniques { get; }


	/// <inheritdoc/>
	public abstract Grid GenerateJustOneCell(out Step? step, CancellationToken cancellationToken = default);

	/// <inheritdoc/>
	public abstract Grid GenerateJustOneCell(out Grid phasedGrid, out Step? step, CancellationToken cancellationToken = default);
}
