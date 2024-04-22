namespace Sudoku.Text.Converters;

/// <summary>
/// Represents with a converter that uses <c>Excel</c>-compatible <c>csv</c>-format to convert <see cref="Grid"/> instances.
/// </summary>
public sealed record ExcelGridConverter : IConceptConverter<Grid>
{
	/// <summary>
	/// Indicates the tab character.
	/// </summary>
	private const char Tab = '\t';

	/// <summary>
	/// Indicates the zero character.
	/// </summary>
	private const char Zero = '0';


	/// <inheritdoc/>
	public FuncRefReadOnly<Grid, string> Converter
		=> (ref readonly Grid grid) =>
		{
			var span = grid.ToString(SusserGridConverter.Default with { Placeholder = Zero }).AsSpan();
			var sb = new StringBuilder(81 + 72 + 9);
			for (var i = 0; i < 9; i++)
			{
				for (var j = 0; j < 9; j++)
				{
					if (span[i * 9 + j] - Zero is var digit and not 0)
					{
						sb.Append(digit);
					}
					sb.Append(Tab);
				}
				sb.RemoveFrom(^1).AppendLine();
			}
			return sb.RemoveFrom(^Environment.NewLine.Length).ToString();
		};
}
