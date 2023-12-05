using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Sudoku.Analytics;
using Sudoku.Concepts.Primitive;
using Sudoku.Text.Parsers;

namespace Sudoku.Concepts.Parsers;

/// <summary>
/// Represents a parser type that uses <b>K9</b> notation rule to parse text,
/// converting into a valid <see cref="ICoordinateObject{TSelf}"/> instance.
/// </summary>
/// <seealso cref="ICoordinateObject{TSelf}"/>
public sealed partial record K9Parser : CoordinateParser
{
	/// <inheritdoc/>
	public override Func<string, CellMap> CellParser => OnCellParsing;

	/// <inheritdoc/>
	public override Func<string, CandidateMap> CandidateParser => OnCandidateParsing;

	/// <inheritdoc/>
	[Obsolete(DeprecatedInfo_NotSupported, true)]
	public override Func<string, HouseMask> HouseParser => OnHouseParsing;

	/// <inheritdoc/>
	public override Func<string, Conclusion[]> ConclusionParser => OnConclusionParsing;

	/// <inheritdoc/>
	public override Func<string, Mask> DigitParser => new RxCyParser().DigitParser;

	/// <inheritdoc/>
	[Obsolete(DeprecatedInfo_NotSupported, true)]
	public override Func<string, Chute[]> ChuteParser => OnChuteParsing;

	/// <inheritdoc/>
	public override Func<string, Conjugate[]> ConjuagteParser => OnConjugateParsing;

	/// <inheritdoc/>
	[Obsolete(DeprecatedInfo_NotSupported, true)]
	public override Func<string, IntersectionCollection> IntersectionParser => OnIntersectionParsing;


	private static CellMap OnCellParsing(string str)
	{
		if (string.IsNullOrWhiteSpace(str))
		{
			return [];
		}

		if (UnitCellGroupPattern().Matches(str) is not { Count: not 0 } matches)
		{
			return [];
		}

		var result = CellMap.Empty;
		foreach (var match in matches.Cast<Match>())
		{
			var s = match.Value;
			var indexOfTheFirstDigit = Array.FindIndex(s.ToCharArray(), char.IsDigit);
			var rows = s[..indexOfTheFirstDigit];
			var columns = s[(indexOfTheFirstDigit + 1)..];
			foreach (var row in rows)
			{
				var finalRow = row is 'I' or 'i' or 'J' or 'j' or 'K' or 'k' ? 'I' : char.ToUpper(row);
				foreach (var column in columns)
				{
					result.Add((finalRow - 'A') * 9 + column - '1');
				}
			}
		}

		return result;
	}

	private static CandidateMap OnCandidateParsing(string str)
	{
		if (string.IsNullOrWhiteSpace(str))
		{
			return [];
		}

		if (UnitCandidateGroupPattern().Matches(str) is not { Count: not 0 } matches)
		{
			return [];
		}

		var result = CandidateMap.Empty;
		foreach (var match in matches.Cast<Match>())
		{
			if (match.Captures is not [{ Value: var rows }, { Value: var columns }, { Value: var digits }])
			{
				continue;
			}

			foreach (var row in rows)
			{
				var finalRow = row is 'I' or 'i' or 'J' or 'j' or 'K' or 'k' ? 'I' : char.ToUpper(row);
				foreach (var column in columns)
				{
					foreach (var digit in digits)
					{
						result.Add(((finalRow - 'A') * 9 + column - '1') * 9 + digit - '1');
					}
				}
			}
		}

		return result;
	}

	private Conclusion[] OnConclusionParsing(string str)
	{
		if (string.IsNullOrWhiteSpace(str))
		{
			return [];
		}

		if (UnitConclusionGroupPattern().Matches(str) is not { Count: not 0 } matches)
		{
			return [];
		}

		var result = new List<Conclusion>();
		foreach (var match in matches.Cast<Match>())
		{
			if (match.Captures is not [{ Value: var cells }, _, { Value: var equalityOperator }, { Value: var digits }])
			{
				continue;
			}

			var cellsInConclusion = CellParser(cells);
			var conclusionType = equalityOperator is "=" or "==" ? ConclusionType.Assignment : ConclusionType.Elimination;
			foreach (var cell in cells)
			{
				foreach (var digit in digits)
				{
					result.Add(new(conclusionType, cell, digit - '1'));
				}
			}
		}

		return [.. result];
	}

	private Conjugate[] OnConjugateParsing(string str)
	{
		if (string.IsNullOrWhiteSpace(str))
		{
			return [];
		}

		if (UnitConjugateGroupPattern().Matches(str) is not { Count: not 0 } matches)
		{
			return [];
		}

		var result = new List<Conjugate>();
		foreach (var match in matches.Cast<Match>())
		{
			if (match.Captures is not [{ Value: var cell1 }, { Value: var cell2 }, { Value: [var digitChar] }])
			{
				continue;
			}

			result.Add(new(CellParser(cell1)[0], CellParser(cell2)[0], digitChar - '1'));
		}

		return [.. result];
	}

	[DoesNotReturn]
	private static HouseMask OnHouseParsing(string _) => throw new NotSupportedException(DeprecatedInfo_NotSupported);

	[DoesNotReturn]
	private static IntersectionCollection OnIntersectionParsing(string _) => throw new NotSupportedException(DeprecatedInfo_NotSupported);

	[DoesNotReturn]
	private static Chute[] OnChuteParsing(string _) => throw new NotSupportedException(DeprecatedInfo_NotSupported);

	[GeneratedRegex("""[a-k]+[1-9]+""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitCellGroupPattern();

	[GeneratedRegex("""([a-k]+)([1-9]+)\.([1-9]+)""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitCandidateGroupPattern();

	[GeneratedRegex("""([a-k]+[1-9]+(,\s*[a-k]+[1-9]+)*)\s*(==?|!=|<>)\s*([1-9]+)""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitConclusionGroupPattern();

	[GeneratedRegex("""([a-k][1-9])\s*={2}\s*([a-k][1-9])\.([1-9])""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitConjugateGroupPattern();
}
