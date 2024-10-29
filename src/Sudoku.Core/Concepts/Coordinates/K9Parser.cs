namespace Sudoku.Concepts.Coordinates;

/// <summary>
/// Represents a parser type that uses <b>K9</b> notation rule to parse text,
/// converting into a valid instance that can be represented as a sudoku concept.
/// </summary>
public sealed partial record K9Parser : CoordinateParser
{
	/// <inheritdoc/>
	public override Func<string, CellMap> CellParser => OnCellParsing;

	/// <inheritdoc/>
	public override Func<string, CandidateMap> CandidateParser => OnCandidateParsing;

	/// <inheritdoc/>
	[Obsolete(DeprecatedInfo_NotSupported, true)]
	public override Func<string, HouseMask> HouseParser => throw new NotSupportedException();

	/// <inheritdoc/>
	public override Func<string, ConclusionSet> ConclusionParser => OnConclusionParsing;

	/// <inheritdoc/>
	public override Func<string, Mask> DigitParser => new RxCyParser().DigitParser;

	/// <inheritdoc/>
	[Obsolete(DeprecatedInfo_NotSupported, true)]
	public override Func<string, ReadOnlySpan<Chute>> ChuteParser => throw new NotSupportedException();

	/// <inheritdoc/>
	public override Func<string, ReadOnlySpan<Conjugate>> ConjugateParser => OnConjugateParsing;

	/// <inheritdoc/>
	[Obsolete(DeprecatedInfo_NotSupported, true)]
	public override Func<string, ReadOnlySpan<Miniline>> IntersectionParser => throw new NotSupportedException();


	[GeneratedRegex("""[a-k]+[1-9]+""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitCellGroupPattern { get; }

	[GeneratedRegex("""([a-k]+)([1-9]+)\.([1-9]+)""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitCandidateGroupPattern { get; }

	[GeneratedRegex("""([a-k]+[1-9]+(,\s*[a-k]+[1-9]+)*)\s*(==?|!=|<>)\s*([1-9]+)""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitConclusionGroupPattern { get; }

	[GeneratedRegex("""([a-k][1-9])\s*={2}\s*([a-k][1-9])\.([1-9])""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitConjugateGroupPattern { get; }


	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public override object? GetFormat(Type? formatType) => formatType == typeof(CoordinateParser) ? this : null;


	private static CellMap OnCellParsing(string str)
	{
		if (string.IsNullOrWhiteSpace(str))
		{
			return [];
		}

		if (UnitCellGroupPattern.Matches(str) is not { Count: not 0 } matches)
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

		if (UnitCandidateGroupPattern.Matches(str) is not { Count: not 0 } matches)
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

	private ConclusionSet OnConclusionParsing(string str)
	{
		if (string.IsNullOrWhiteSpace(str))
		{
			return [];
		}

		if (UnitConclusionGroupPattern.Matches(str) is not { Count: not 0 } matches)
		{
			return [];
		}

		var result = new ConclusionSet();
		foreach (var match in matches.Cast<Match>())
		{
			if (match.Captures is not [{ Value: var cells }, _, { Value: var equalityOperator }, { Value: var digits }])
			{
				continue;
			}

			var conclusionType = equalityOperator is "=" or "==" ? Assignment : Elimination;
			foreach (var cell in cells)
			{
				foreach (var digit in digits)
				{
					result.Add(new(conclusionType, cell, digit - '1'));
				}
			}
		}
		return result;
	}

	private ReadOnlySpan<Conjugate> OnConjugateParsing(string str)
	{
		if (string.IsNullOrWhiteSpace(str))
		{
			return [];
		}

		if (UnitConjugateGroupPattern.Matches(str) is not { Count: not 0 } matches)
		{
			return [];
		}

		var result = new List<Conjugate>();
		foreach (var match in matches.Cast<Match>())
		{
			if (match.Captures is [{ Value: var cell1 }, { Value: var cell2 }, { Value: [var digitChar] }])
			{
				result.Add(new(CellParser(cell1)[0], CellParser(cell2)[0], digitChar - '1'));
			}
		}
		return result.AsSpan();
	}
}
