using System.Runtime.CompilerServices;
using System.SourceGeneration;
using System.Text;
using System.Text.RegularExpressions;
using Sudoku.Concepts;

namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a notation that represents for a candidate or a list of candidates.
/// </summary>
public sealed partial class CandidateNotation : INotation<CandidateNotation, CandidateMap, Candidate, CandidateNotation.Kind>
{
	/// <summary>
	/// Try to parse the specified text, converting it into the target candidate value via RxCy Notation rule.
	/// </summary>
	/// <param name="text"><inheritdoc cref="Parse(string, Kind)" path="/param[@name='text']"/></param>
	/// <returns><inheritdoc cref="Parse(string, Kind)" path="/returns"/></returns>
	/// <seealso cref="Kind.RxCy"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidate Parse(string text) => Parse(text, Kind.RxCy);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidate Parse(string text, Kind notation)
		=> (notation, text) switch
		{
			(
				Kind.RxCy,
				['R' or 'r', var r and >= '1' and <= '9', 'C' or 'c', var c and >= '1' and <= '9', '(', var d and >= '1' and <= '9', ')']
			) => ((r - '1') * 9 + (c - '1')) * 9 + (d - '1'),
			(Kind.RxCy, _) => throw new InvalidOperationException(),
			(
				Kind.K9,
				[var r and (>= 'A' and <= 'I' or 'K'), var c and >= '1' and <= '9', '.', var d and >= '1' and <= '9']
			) => ((r - '1') * 9 + (c - '1')) * 9 + (d - '1'),
			(Kind.K9, _) => throw new InvalidOperationException(),
			(
				Kind.HodokuTriplet,
				[var r and >= '1' and <= '9', var c and >= '1' and <= '9', var d and >= '1' and <= '9']
			) => ((r - '1') * 9 + (c - '1')) * 9 + (d - '1'),
			(Kind.HodokuTriplet, _) => throw new InvalidOperationException(),
			_ => throw new ArgumentOutOfRangeException(nameof(notation))
		};

	/// <summary>
	/// Try to parse the specified text using basic parsing rule, converting it into a collection of type <see cref="CandidateMap"/>.
	/// </summary>
	/// <param name="text">The text to be parsed.</param>
	/// <returns>The target result instance of type <see cref="CandidateMap"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap ParseCollection(string text) => ParseCollection(text, Kind.RxCy);

	/// <inheritdoc/>
	/// <exception cref="NotSupportedException">
	/// Throws when the argument <paramref name="notation"/> is <see cref="Kind.K9"/>.
	/// </exception>
	/// <exception cref="FormatException">
	/// Throws when the argument <paramref name="text"/> cannot be parsed as a valid <see cref="CandidateMap"/> result.
	/// </exception>
	public static CandidateMap ParseCollection(string text, Kind notation)
	{
		switch (notation)
		{
			case Kind.RxCy:
			{
				if (prepositionalForm(text, out var r))
				{
					return r;
				}
				if (postpositionalForm(text, out r))
				{
					return r;
				}
				if (complexPrepositionalForm(text, out r))
				{
					return r;
				}
				if (complexPostpositionalForm(text, out r))
				{
					return r;
				}

				throw new FormatException("The target string cannot be parsed as a valid candidates collection.");


				static bool prepositionalForm(string str, out CandidateMap result)
				{
					if (NotationPatterns.Candidates_PrepositionalFormPattern().Match(str) is not { Success: true, Value: var s })
					{
						goto ReturnInvalid;
					}

					if (s.Split(['R', 'r', 'C', 'c']) is not [var digits, var rows, var columns])
					{
						goto ReturnInvalid;
					}

					var r = CandidateMap.Empty;
					foreach (var row in rows)
					{
						foreach (var column in columns)
						{
							foreach (var digit in digits)
							{
								r.Add(((row - '1') * 9 + (column - '1')) * 9 + (digit - '1'));
							}
						}
					}

					result = r;
					return true;

				ReturnInvalid:
					Unsafe.SkipInit(out result);
					return false;
				}

				static bool postpositionalForm(string str, out CandidateMap result)
				{
					if (NotationPatterns.Candidates_PostpositionalFormPattern().Match(str) is not { Success: true, Value: [_, .. var s, _] })
					{
						goto ReturnInvalid;
					}

					if (s.SplitBy(['C', 'c', '(']) is not [var rows, var columns, var digits])
					{
						goto ReturnInvalid;
					}

					var r = CandidateMap.Empty;
					foreach (var row in rows)
					{
						foreach (var column in columns)
						{
							foreach (var digit in digits)
							{
								r.Add(((row - '1') * 9 + (column - '1')) * 9 + (digit - '1'));
							}
						}
					}

					result = r;
					return true;

				ReturnInvalid:
					Unsafe.SkipInit(out result);
					return false;
				}

				static bool complexPrepositionalForm(string str, out CandidateMap result)
				{
					if (NotationPatterns.Candidates_ComplexPrepositionalFormPattern().Match(str) is not { Success: true, Value: var s })
					{
						goto ReturnInvalid;
					}

					var cells = CellMap.Empty;
					foreach (var match in NotationPatterns.CellOrCellListPattern_RxCy().Matches(s).Cast<Match>())
					{
						cells |= CellNotation.ParseCollection(match.Value);
					}

					var digits = s[..s.IndexOf('{')];
					var r = CandidateMap.Empty;
					foreach (var cell in cells)
					{
						foreach (var digit in digits)
						{
							r.Add(cell * 9 + (digit - '1'));
						}
					}

					result = r;
					return true;

				ReturnInvalid:
					Unsafe.SkipInit(out result);
					return false;
				}

				static bool complexPostpositionalForm(string str, out CandidateMap result)
				{
					if (NotationPatterns.Candidates_ComplexPostpositionalFormPattern().Match(str) is not { Success: true, Value: var s })
					{
						goto ReturnInvalid;
					}

					var cells = CellMap.Empty;
					foreach (var match in NotationPatterns.CellOrCellListPattern_RxCy().Matches(s).Cast<Match>())
					{
						cells |= CellNotation.ParseCollection(match.Value);
					}

					var digits = s[(s.IndexOf('(') + 1)..s.IndexOf(')')];
					var r = CandidateMap.Empty;
					foreach (var cell in cells)
					{
						foreach (var digit in digits)
						{
							r.Add(cell * 9 + (digit - '1'));
						}
					}

					result = r;
					return true;

				ReturnInvalid:
					Unsafe.SkipInit(out result);
					return false;
				}
			}
			case Kind.K9:
			{
				throw new NotSupportedException("Cannot parse collection via K9 notation.");
			}
			case Kind.HodokuTriplet:
			{
				var segments = text.SplitBy([' ']);
				if (Array.IndexOf(segments, string.Empty) != -1)
				{
					throw new FormatException("The string contains empty segment.");
				}

				var result = CandidateMap.Empty;
				foreach (var segment in segments)
				{
					if (segment is [var d and >= '1' and <= '9', var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
					{
						result.Add(((r - '1') * 9 + c - '1') * 9 + d - '1');
						continue;
					}

					throw new FormatException("Each candidate segment contains invalid character.");
				}

				return result;
			}
			default:
			{
				throw new ArgumentOutOfRangeException(nameof(notation));
			}
		}
	}

	/// <summary>
	/// Gets the text notation that can represent the specified value.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns><inheritdoc cref="ToString(Candidate, Kind)" path="/returns"/></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToString(Candidate value) => ToString(value, Kind.RxCy);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToString(Candidate value, Kind notation)
		=> notation switch
		{
			Kind.RxCy => $"r{value / 9 / 9 + 1}c{value / 9 % 9 + 1}({value % 9 + 1})",
			Kind.K9 => $"{(char)(value / 9 / 9 + 'A')}{value / 9 % 9 + 1}.{value % 9 + 1}",
			Kind.HodokuTriplet => $"{value % 9 + 1}{value / 9 / 9 + 1}{value / 9 % 9 + 1}",
			_ => throw new ArgumentOutOfRangeException(nameof(notation))
		};

	/// <summary>
	/// Gets the text notation that can represent the specified collection.
	/// </summary>
	/// <param name="collection"><inheritdoc cref="ToString(Candidate)" path="/param[@name='value']"/></param>
	/// <returns><inheritdoc cref="ToString(Candidate)" path="/returns"/></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCollectionString(scoped in CandidateMap collection) => ToCollectionString(collection, Kind.RxCy);

	/// <inheritdoc cref="INotation{TSelf, TCollection, TElement, TConceptKindPresenter}.ToCollectionString(TCollection, TConceptKindPresenter)"/>
	/// <exception cref="NotSupportedException">Throws when <paramref name="notation"/> is <see cref="Kind.K9"/>.</exception>
	[ExplicitInterfaceImpl(typeof(INotation<,,,>))]
	public static string ToCollectionString(scoped in CandidateMap collection, Kind notation)
	{
		switch (notation)
		{
			case Kind.RxCy:
			{
				scoped var sb = new StringHandler(50);

				foreach (var digitGroup in
					from candidate in collection
					group candidate by candidate % 9 into digitGroups
					orderby digitGroups.Key
					select digitGroups)
				{
					var cells = CellMap.Empty;
					foreach (var candidate in digitGroup)
					{
						cells.Add(candidate / 9);
					}

					sb.Append(CellNotation.ToCollectionString(cells));
					sb.Append('(');
					sb.Append(digitGroup.Key + 1);
					sb.Append(')');
					sb.Append(", ");
				}

				sb.RemoveFromEnd(2);
				return sb.ToStringAndClear();
			}
			case Kind.K9:
			{
				throw new NotSupportedException("K9 Notation is not supported for K9 Notation.");
			}
			case Kind.HodokuTriplet:
			{
				return collection switch
				{
					[] => string.Empty,
					[var p] => $"{p % 9 + 1}{p / 9 / 9 + 1}{p / 9 % 9 + 1}",
					_ => f(collection)
				};


				static string f(scoped in CandidateMap collection)
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
			}
			default:
			{
				throw new ArgumentOutOfRangeException(nameof(notation));
			}
		}
	}
}
