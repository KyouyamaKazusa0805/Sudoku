namespace Sudoku.Concepts.Coordinates;

/// <summary>
/// Represents a parser type that uses <b>RxCy</b> notation rule to parse text,
/// converting into a valid instance that can be represented as a sudoku concept.
/// </summary>
public sealed partial record RxCyParser : CoordinateParser
{
	/// <inheritdoc/>
	public override Func<string, CellMap> CellParser => OnCellParsing;

	/// <inheritdoc/>
	public override Func<string, CandidateMap> CandidateParser => OnCandidateParsing;

	/// <inheritdoc/>
	public override Func<string, HouseMask> HouseParser => OnHouseParsing;

	/// <inheritdoc/>
	public override Func<string, ConclusionSet> ConclusionParser => OnConclusionParsing;

	/// <inheritdoc/>
	public override Func<string, Mask> DigitParser => OnDigitParsing;

	/// <inheritdoc/>
	public override Func<string, ReadOnlySpan<Chute>> ChuteParser => OnChuteParsing;

	/// <inheritdoc/>
	public override Func<string, ReadOnlySpan<Conjugate>> ConjugateParser => OnConjugateParsing;

	/// <inheritdoc/>
	public override Func<string, ReadOnlySpan<Miniline>> IntersectionParser => OnIntersectionParsing;


	[GeneratedRegex("""r[1-9]+c[1-9]+""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitCellGroupPattern { get; }

	[GeneratedRegex("""r[1-9]+c[1-9]+\([1-9]+\)|[1-9]+r[1-9]+c[1-9]+""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitCandidateGroupPattern { get; }

	[GeneratedRegex("""r[1-9]+c[1-9]+(,\s*r[1-9]+c[1-9]+)*\s*(==?|!=|<>)\s*[1-9]+""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitConclusionGroupPattern { get; }

	[GeneratedRegex("""[rcb][1-9]+""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitHousePattern { get; }

	[GeneratedRegex("""[rc][1-9]+b[1-9]+""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitIntersectionGroupPattern { get; }

	[GeneratedRegex("""m[rc][1-3]+""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitMegaLineGroupPattern { get; }

	[GeneratedRegex("""r[1-9]c[1-9]\s*={2}\s*r[1-9]c[1-9]\([1-9]\)""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
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
			var indexOfColumnLabel = s.IndexOfAny(['C', 'c']);
			var rowDigits = s[1..indexOfColumnLabel];
			var columnDigits = s[(indexOfColumnLabel + 1)..];
			foreach (var row in rowDigits)
			{
				foreach (var column in columnDigits)
				{
					result.Add((row - '1') * 9 + column - '1');
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
			var s = match.Value;
			if (s.Contains('('))
			{
				var indexOfColumnLabel = s.IndexOfAny(['C', 'c']);
				var indexOfOpenBraceLabel = s.IndexOf('(');
				var rowDigits = s[1..indexOfColumnLabel];
				var columnDigits = s[(indexOfColumnLabel + 1)..indexOfOpenBraceLabel];
				var digitDigits = s[(indexOfOpenBraceLabel + 1)..^1];
				foreach (var row in rowDigits)
				{
					foreach (var column in columnDigits)
					{
						foreach (var digit in digitDigits)
						{
							result.Add(((row - '1') * 9 + column - '1') * 9 + digit - '1');
						}
					}
				}
			}
			else
			{
				var indexOfRowLabel = s.IndexOfAny(['R', 'r']);
				var indexOfColumnLabel = s.IndexOfAny(['C', 'c']);
				var digitDigits = s[..indexOfRowLabel];
				var rowDigits = s[(indexOfRowLabel + 1)..indexOfColumnLabel];
				var columnDigits = s[(indexOfColumnLabel + 1)..];
				foreach (var row in rowDigits)
				{
					foreach (var column in columnDigits)
					{
						foreach (var digit in digitDigits)
						{
							result.Add(((row - '1') * 9 + column - '1') * 9 + digit - '1');
						}
					}
				}
			}
		}

		return result;
	}

	private static HouseMask OnHouseParsing(string str)
	{
		if (string.IsNullOrWhiteSpace(str))
		{
			return 0;
		}

		if (UnitHousePattern.Matches(str) is not { Count: not 0 } matches)
		{
			return 0;
		}

		var result = 0;
		foreach (var match in matches.Cast<Match>())
		{
			var s = match.Value;
			var label = s[0];
			foreach (var house in from digit in s[1..] select digit - '1')
			{
				result |= 1 << label switch { 'R' or 'r' => 9, 'C' or 'c' => 18, _ => 0 } + house;
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
			var s = match.Value;
			var indexOfEqualityOperatorCharacters = s.Split(["==", "<>", "=", "!="], 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var cells = CellParser(indexOfEqualityOperatorCharacters[0]);
			var digits = MaskOperations.Create(from character in indexOfEqualityOperatorCharacters[1] select character - '1');
			var conclusionType = s.Match("""==?|<>|!=""") is "==" or "=" ? Assignment : Elimination;
			foreach (var cell in cells)
			{
				foreach (var digit in digits)
				{
					result.Add(new(conclusionType, cell, digit));
				}
			}
		}

		return result;
	}

	private static Mask OnDigitParsing(string str)
		=> str.MatchAll("""\d""") is { Length: <= 9 } matches
			? MaskOperations.Create(from digitString in matches select digitString[0] - '1')
			: throw new InvalidOperationException(SR.ExceptionMessage("ErrorInfo_DuplicatedValuesMayExistOrInvalid"));

	private static ReadOnlySpan<Chute> OnChuteParsing(string str)
	{
		if (string.IsNullOrWhiteSpace(str))
		{
			return [];
		}

		if (UnitMegaLineGroupPattern.Matches(str) is not { Count: not 0 } matches)
		{
			return [];
		}

		var result = new List<Chute>(6);
		foreach (var match in matches.Cast<Match>())
		{
			switch (match.Value)
			{
				case [_, 'R' or 'r', var r and >= '1' and <= '3']:
				{
					result.AddRef(in Chutes[r - '1']);
					break;
				}
				case [_, 'C' or 'c', var c and >= '1' and <= '3']:
				{
					result.AddRef(in Chutes[c + 3 - '1']);
					break;
				}
			}
		}
		return result.AsSpan();
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
			var s = match.Value;
			var split = s.Split("==", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var leftCell = CellParser(split[0])[0];
			var rightCellAndDigit = CandidateParser(split[1])[0];
			var rightCell = rightCellAndDigit / 9;
			var digit = rightCellAndDigit % 9;
			result.Add(new(leftCell, rightCell, digit));
		}
		return result.AsSpan();
	}

	private static ReadOnlySpan<Miniline> OnIntersectionParsing(string str)
	{
		if (string.IsNullOrWhiteSpace(str))
		{
			return [];
		}

		if (UnitIntersectionGroupPattern.Matches(str) is not { Count: not 0 } matches)
		{
			return [];
		}

		var result = new List<Miniline>();
		foreach (var match in matches.Cast<Match>())
		{
			var s = match.Value;
			var indexOfBlockLabel = s.IndexOfAny(['B', 'b']);
			var lineLabel = s[0];
			var lines = s[1..indexOfBlockLabel];
			var blocks = s[(indexOfBlockLabel + 1)..];
			foreach (var line in lines)
			{
				foreach (var block in blocks)
				{
					var @base = new MinilineBase(
						(byte)(lineLabel is 'R' or 'r' ? line + 9 - '1' : line + 18 - '1'),
						(byte)(block - '1')
					);
					result.Add(new(in @base, in Miniline.Map[@base]));
				}
			}
		}
		return result.AsSpan();
	}
}
