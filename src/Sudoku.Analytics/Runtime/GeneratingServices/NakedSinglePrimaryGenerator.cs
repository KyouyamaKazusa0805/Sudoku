namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator that supports generating for puzzles that can be solved by only using Naked Singles.
/// </summary>
/// <seealso cref="Technique.NakedSingle"/>
public sealed class NakedSinglePrimaryGenerator : IPrimaryGenerator
{
	/// <inheritdoc/>
	public TechniqueSet SupportedTechniques => [Technique.NakedSingle];


	/// <inheritdoc/>
	public Grid GenerateUnique(CancellationToken cancellationToken = default)
	{
		var generator = new Generator();
		while (true)
		{
			var puzzle = new Generator().Generate(cancellationToken: cancellationToken);
			if (puzzle.IsUndefined)
			{
				return Grid.Undefined;
			}

			if (!puzzle.CanPrimaryNakedSingle())
			{
				cancellationToken.ThrowIfCancellationRequested();
				continue;
			}
			return puzzle;
		}
	}
}
