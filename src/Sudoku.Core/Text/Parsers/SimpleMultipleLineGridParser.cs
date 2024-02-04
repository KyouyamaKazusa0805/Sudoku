namespace Sudoku.Text.Parsers;

/// <summary>
/// Represents a simple multiple-line grid parser.
/// </summary>
/// <example>
/// For example:
/// <list type="bullet">
/// <item>
/// <code><![CDATA[
/// .----------.----------.----------.
/// |  .  4  6 |  . +1  . |  3 +7 +5 |
/// | +3 +8  1 | +5  4  7 | +2 +9 +6 |
/// |  .  .  5 |  .  3  . | +1 +4  8 |
/// :----------+----------+----------:
/// |  8  .  4 |  . +5  . | +7  6  . |
/// |  .  9  . |  .  2  . | +8  5 +4 |
/// |  .  5  . |  .  .  . |  9  .  3 |
/// :----------+----------+----------:
/// |  5  .  . |  .  8  . |  6  . +9 |
/// | +4  .  8 |  1  9  . |  5  .  . |
/// |  .  .  9 |  .  . +5 |  4  8  . |
/// '----------'----------'----------'
/// ]]></code>
/// </item>
/// <item>
/// <code><![CDATA[
/// 080630040
/// 200085009
/// 090000081
/// 000300800
/// 000020000
/// 006001000
/// 970000030
/// 400850007
/// 010094050
/// ]]></code>
/// </item>
/// </list>
/// </example>
public sealed partial record SimpleMultipleLineGridParser : IConceptParser<Grid>
{
	/// <inheritdoc/>
	public Func<string, Grid> Parser
		=> [MethodImpl(MethodImplOptions.AggressiveInlining)] static (string str) =>
		{
			if (GridSimpleMultilinePattern().Match(str) is not { Success: true, Value: var match })
			{
				return Grid.Undefined;
			}

			// Remove all '\r's and '\n's.
			scoped var sb = new StringHandler(81 + (9 << 1));
			sb.Append(from @char in match where @char is not ('\r' or '\n') select @char);
			return new SusserGridParser().Parser(sb.ToStringAndClear());
		};


	[GeneratedRegex("""([\d\.\+]{9}(\r|\n|\r\n)){8}[\d\.\+]{9}""", RegexOptions.Compiled, 5000)]
	public static partial Regex GridSimpleMultilinePattern();
}
