using System.Text;
using Sudoku.Concepts;

namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a coordinate converter using <b>Hodoku Triplet</b> notation.
/// </summary>
public sealed record HodokuTripletConverter : IConceptConverter<CandidateMap>
{
	/// <inheritdoc/>
	public FuncRefReadOnly<CandidateMap, string> Converter
		=> static (scoped ref readonly CandidateMap candidates) =>
		{
			return candidates switch { [] => string.Empty, [var p] => $"{p % 9 + 1}{p / 9 / 9 + 1}{p / 9 % 9 + 1}", _ => f(in candidates) };


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
}
