namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator type that can generate puzzles with direct techniques.
/// </summary>
public abstract class DirectTechniqueGenerator : TechniqueGenerator, ITechniqueGenerator, IJustOneCellGenerator
{
	/// <inheritdoc/>
	public abstract TechniqueSet SupportedTechniques { get; }


	/// <inheritdoc/>
	static ref readonly Random IJustOneCellGenerator.Rng => ref Rng;


	/// <inheritdoc/>
	public abstract Grid GenerateJustOneCell(out Step? step, CancellationToken cancellationToken = default);

	/// <inheritdoc/>
	public abstract Grid GenerateJustOneCell(out Grid phasedGrid, out Step? step, CancellationToken cancellationToken = default);
}
