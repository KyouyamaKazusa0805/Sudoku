namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a generator that create puzzles containing the specified technique.
/// </summary>
public static class TechniqueBasedGenerator
{
	/// <summary>
	/// Try to generate a puzzle that contains the specified technique.
	/// </summary>
	/// <param name="technique">The technique to be checked.</param>
	/// <param name="analyzer">The analyzer object that solves for the puzzle.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>A <see cref="Grid"/> result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="technique"/> is <see cref="Technique.None"/> or not defined.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Throws when the argument <paramref name="technique"/> is not implemented for searching rule.
	/// </exception>
	public static Grid Generate(Technique technique, Analyzer analyzer, CancellationToken cancellationToken = default)
	{
		switch (technique)
		{
			case Technique.None:
			case var _ when !Enum.IsDefined(technique):
			{
				throw new ArgumentOutOfRangeException(nameof(technique));
			}
			case var _ when technique.GetFeature().HasFlag(TechniqueFeatures.NotImplemented):
			{
				throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("TechniqueIsNotImplemented"));
			}
			case Technique.FullHouse when new FullHousePuzzleGenerator().TryGenerateUnique(out var result, cancellationToken):
			{
				return result;
			}
			default:
			{
				while (true)
				{
					var puzzle = new HodokuPuzzleGenerator().Generate(cancellationToken: cancellationToken);
					if (puzzle.IsUndefined)
					{
						return puzzle;
					}

					if (analyzer.Analyze(in puzzle, cancellationToken: cancellationToken) is not { IsSolved: true, Steps: var steps })
					{
						continue;
					}

					var techniques = new SortedSet<Technique>(from step in steps select step.Code);
					if (techniques.Contains(technique))
					{
						return puzzle;
					}

					cancellationToken.ThrowIfCancellationRequested();
				}
			}
		}
	}
}
