using System.Diagnostics.CodeAnalysis;
using System.Text;
using Sudoku.Analytics;
using Sudoku.Concepts;

namespace Sudoku.Text.Coordinate;

/// <summary>
/// Represents a coordinate converter using K9 notation.
/// </summary>
public sealed record HodokuTripletConverter : CoordinateConverter
{
#nullable disable
	/// <inheritdoc/>
	public override CellNotationConverter CellConverter
		=> [DoesNotReturn] static (scoped ref readonly CellMap _) => throw new NotSupportedException("Triplet notation does not support cell text output.");
#nullable restore

	/// <inheritdoc/>
	public override CandidateNotationConverter CandidateConverter
		=> (scoped ref readonly CandidateMap candidates) =>
		{
			return candidates switch
			{
				[] => string.Empty,
				[var p] => $"{p % 9 + 1}{p / 9 / 9 + 1}{p / 9 % 9 + 1}",
				_ => f(in candidates)
			};


			static string f(scoped ref readonly CandidateMap collection)
			{
				scoped var sb = new StringHandler();
				foreach (var candidate in collection)
				{
					var (cell, digit) = (candidate / 9, candidate % 9);
					sb.Append($"{digit + 1}{cell / 9 + 1}{cell % 9 + 1} ");
				}

				sb.RemoveFromEnd(1);

				return sb.ToStringAndClear();
			}
		};

#nullable disable
	/// <inheritdoc/>
	public override HouseNotationConverter HouseConverter
		=> [DoesNotReturn] static (HouseMask _) => throw new NotSupportedException("Triplet notation does not support house text output.");

	/// <inheritdoc/>
	public override ConclusionNotationConverter ConclusionConverter
		=> [DoesNotReturn] static (scoped ReadOnlySpan<Conclusion> _) => throw new NotSupportedException("Triplet notation does not support conclusion text output.");

	/// <inheritdoc/>
	public override DigitNotationConverter DigitConverter
		=> [DoesNotReturn] static (Mask _) => throw new NotSupportedException("Triplet notation does not support digit text output.");

	/// <inheritdoc/>
	public override IntersectionNotationConverter IntersectionConverter
		=> [DoesNotReturn] static (scoped ReadOnlySpan<(IntersectionBase Base, IntersectionResult Result)> _) => throw new NotSupportedException("Triplet notation does not support intersection text output.");

	/// <inheritdoc/>
	public override ChuteNotationConverter ChuteConverter
		=> [DoesNotReturn] static (scoped ReadOnlySpan<Chute> _) => throw new NotSupportedException("Triplet notation does not support chute text output.");

	/// <inheritdoc/>
	public override ConjugateNotationConverter ConjugateConverter
		=> [DoesNotReturn] static (scoped ReadOnlySpan<Conjugate> _) => throw new NotSupportedException("Triplet notation does not support conjugate text output.");
#nullable restore
}
