namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator that supports generating for puzzles that can be solved by only using Hidden Singles and Full Houses.
/// </summary>
/// <seealso cref="Technique.CrosshatchingBlock"/>
/// <seealso cref="Technique.CrosshatchingRow"/>
/// <seealso cref="Technique.CrosshatchingColumn"/>
public sealed class HiddenSinglePrimaryGenerator : IPrimaryGenerator, ITechniqueBasedGenerator
{
	/// <summary>
	/// Indicates the backing analyzer.
	/// </summary>
	private static readonly Analyzer Analyzer = Analyzer.Default.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true, HiddenSinglesInBlockFirst = true });


	/// <inheritdoc/>
	public TechniqueSet SupportedTechniques => [Technique.CrosshatchingBlock, Technique.CrosshatchingRow, Technique.CrosshatchingColumn];


	/// <inheritdoc cref="ITechniqueBasedGenerator.GenerateUnique(CancellationToken)"/>
	public Grid GenerateUnique(bool allowsBlockExcluders, CancellationToken cancellationToken = default)
	{
		var generator = new Generator();
		while (true)
		{
			var puzzle = generator.Generate(cancellationToken: cancellationToken);
			if (puzzle.IsUndefined)
			{
				return Grid.Undefined;
			}

			if (!puzzle.CanPrimaryHiddenSingle(true))
			{
				cancellationToken.ThrowIfCancellationRequested();
				continue;
			}

			if (!allowsBlockExcluders && Analyzer.Analyze(in puzzle, cancellationToken: cancellationToken).HasBlockExcluders())
			{
				cancellationToken.ThrowIfCancellationRequested();
				continue;
			}

			return puzzle;
		}
	}

	/// <inheritdoc/>
	Grid ITechniqueBasedGenerator.GenerateUnique(CancellationToken cancellationToken) => GenerateUnique(true, cancellationToken);

	/// <inheritdoc/>
	Grid IPrimaryGenerator.GeneratePrimary(CancellationToken cancellationToken) => GenerateUnique(true, cancellationToken);
}
