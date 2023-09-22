using System.Text.RegularExpressions;
using Sudoku.Analytics;
using Sudoku.Concepts;

namespace Sudoku.Text.Coordinate;

/// <summary>
/// Represents a parser type that uses <b>RxCy</b> notation rule to parse text,
/// converting into a valid <see cref="ICoordinateObject"/> instance.
/// </summary>
/// <seealso cref="ICoordinateObject"/>
public sealed partial record RxCyParser : CoordinateParser
{
	/// <inheritdoc/>
	public override Func<string, CellMap> CellParser
		=> static str =>
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
				var indexOfColumnLabel = s.IndexOf('c', StringComparison.OrdinalIgnoreCase);
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
		};

	/// <inheritdoc/>
	public override Func<string, CandidateMap> CandidateParser
		=> static str =>
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return [];
			}

			if (UnitCellGroupPattern().Matches(str) is not { Count: not 0 } matches)
			{
				return [];
			}

			var result = CandidateMap.Empty;
			foreach (var match in matches.Cast<Match>())
			{
				var s = match.Value;
				if (s.Contains('('))
				{
					var indexOfColumnLabel = s.IndexOf('c', StringComparison.OrdinalIgnoreCase);
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
					var indexOfRowLabel = s.IndexOf('r', StringComparison.OrdinalIgnoreCase);
					var indexOfColumnLabel = s.IndexOf('c', StringComparison.OrdinalIgnoreCase);
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
		};

	/// <inheritdoc/>
	public override Func<string, HouseMask> HouseParser
		=> static str =>
		{
			throw new NotSupportedException("Not supported now.");
		};

	/// <inheritdoc/>
	public override Func<string, Conclusion[]> ConclusionParser
		=> static str =>
		{
			throw new NotSupportedException("Not supported now.");
		};

	/// <inheritdoc/>
	public override Func<string, Mask> DigitParser
		=> static str =>
		{
			throw new NotSupportedException("Not supported now.");
		};

	/// <inheritdoc/>
	public override Func<string, (IntersectionBase Base, IntersectionResult Result)[]> IntersectionParser
		=> static str =>
		{
			throw new NotSupportedException("Not supported now.");
		};

	/// <inheritdoc/>
	public override Func<string, Chute[]> ChuteParser
		=> static str =>
		{
			throw new NotSupportedException("Not supported now.");
		};

	/// <inheritdoc/>
	public override Func<string, Conjugate[]> ConjuagteParser
		=> static str =>
		{
			throw new NotSupportedException("Not supported now.");
		};


	[GeneratedRegex("""r[1-9]+c[1-9]+""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitCellGroupPattern();

	[GeneratedRegex("""r[1-9]+c[1-9]+\([1-9]+\)|[1-9]+r[1-9]+c[1-9]+""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex UnitCandidateGroupPattern();
}
