namespace Sudoku.Text.Parsers;

/// <summary>
/// Represents a Sukaku grid parser.
/// </summary>
/// <param name="SingleLine">
/// Indicates whether the parser use single-line logic to parse the string text.
/// The single-line text uses 729 characters to describe all possible existences for all 9 digits in all 81 cells.
/// </param>
/// <example>
/// For example:
/// <list type="number">
/// <item>
/// <code><![CDATA[
/// 023406789123456700003050780120050789003006009123456009100056089023456080123000789
/// 003456789023006700023006700120056780100006789120400780000406789103000080020450000
/// 003406089020056709120050080100000700120056780120000709123056000123050080000450089
/// 123050709100456089003400780100450009003400009120400009123056709003000700023406000
/// 000406009123006000000056709003406700120050009103056089103456709003056009120456009
/// 123456780023400000103406709003056789020406700100050009020400000100006789020006789
/// 020456780123056080120400700000406789120400080023456789003006080020456789103450709
/// 123400009123400080023406700123406709103400080123456009120456789023406709023450080
/// 100406080103056009100400009123000789100400709100406780123050700000050080023406009
/// ]]></code>
/// </item>
/// <item>
/// <code><![CDATA[
///  23456789  23456789  23456789 123456789 123456789  23456789 123456789  23456789  23456789
///  23456789 123456789 123456789  23456789  23456789  23456789  23456789  23456789  23456789
///  23456789  23456789  23456789  23456789 123456789  23456789 123456789  23456789 123456789
/// 123456789  23456789  23456789 123456789 123456789  23456789  23456789  23456789 123456789
///  23456789  23456789  23456789  23456789 123456789  23456789 123456789  23456789  23456789
/// 123456789  23456789  23456789 123456789 123456789  23456789 123456789  23456789 123456789
///  23456789 123456789 123456789  23456789  23456789 123456789  23456789 123456789  23456789
///  23456789 123456789 123456789  23456789  23456789 123456789  23456789 123456789  23456789
///  23456789 123456789 123456789  23456789  23456789 123456789  23456789 123456789  23456789
/// ]]></code>
/// </item>
/// </list>
/// </example>
public sealed partial record SukakuGridParser(bool SingleLine = false) : IConceptParser<Grid>
{
	/// <inheritdoc/>
	public Func<string, Grid> Parser
		=> str =>
		{
			if (SingleLine)
			{
				if (str.Length < 729)
				{
					return Grid.Undefined;
				}

				var result = Grid.Empty;
				for (var i = 0; i < 729; i++)
				{
					var c = str[i];
					if (c is not (>= '0' and <= '9' or '.'))
					{
						return Grid.Undefined;
					}

					if (c is '0' or '.')
					{
						result.SetExistence(i / 9, i % 9, false);
					}
				}

				return result;
			}
			else
			{
				var matches = from m in GridSukakuSegmentPattern().Matches(str) select m.Value;
				if (matches is { Length: not 81 })
				{
					return Grid.Undefined;
				}

				var result = Grid.Empty;
				for (var offset = 0; offset < 81; offset++)
				{
					var s = matches[offset].Reserve(@"\d");
					if (s.Length > 9)
					{
						// More than 9 characters.
						return Grid.Undefined;
					}

					var mask = (Mask)0;
					foreach (var c in s)
					{
						mask |= (Mask)(1 << c - '1');
					}

					if (mask == 0)
					{
						return Grid.Undefined;
					}

					for (var digit = 0; digit < 9; digit++)
					{
						result.SetExistence(offset, digit, (mask >> digit & 1) != 0);
					}
				}

				return result;
			}
		};


	[GeneratedRegex("""\d*[\-\+]?\d+""", RegexOptions.Compiled, 5000)]
	public static partial Regex GridSukakuSegmentPattern();
}
