namespace Sudoku.Text.Parsers;

/// <summary>
/// Represents an Excel grid parser.
/// </summary>
/// <example>
/// For example:
/// <code><![CDATA[
/// 1			7	8	9	4	5	6
/// 4	5	6	1	2	3	7	8	9
/// 7	8	9	4	5	6	1	2	
/// 9	1	2	6		8	3	4	
/// 3	4	5				6	7	8
/// 	7	8	3		5	9	1	2
/// 	9	1	5	6	7	2	3	4
/// 2	3	4	8	9	1	5	6	7
/// 5	6	7	2	3	4			1
/// ]]></code>
/// </example>
public sealed record ExcelGridParser : IConceptParser<Grid>
{
	/// <inheritdoc/>
	public Func<string, Grid> Parser
		=> static str =>
		{
			if (!str.Contains('\t'))
			{
				return Grid.Undefined;
			}

			if (str.SplitBy('\r', '\n') is not { Length: 9 } values)
			{
				return Grid.Undefined;
			}

			var sb = new StringBuilder(81);
			foreach (var value in values)
			{
				foreach (var digitString in value.Split('\t'))
				{
					sb.Append(string.IsNullOrEmpty(digitString) ? '.' : digitString[0]);
				}
			}

			return Grid.Parse(sb.ToString());
		};
}
