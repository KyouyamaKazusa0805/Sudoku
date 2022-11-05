namespace Sudoku.Text.Formatting;

/// <summary>
/// Defines a formatter that formats a <see cref="CellMap"/>, converting into a <see cref="string"/> of a list of binary value
/// to display all values.
/// </summary>
/// <param name="WithSeparator">
/// <para>Indicates whether the formatter will emit a separator between two adjacent 27-bit chunks.</para>
/// <para>The default value is <see langword="true"/>.</para>
/// </param>
public sealed record CellMapBinaryFormat(bool WithSeparator = true) : ICellMapFormatter
{
	/// <inheritdoc cref="ICellMapFormatter.Instance"/>
	public static readonly CellMapBinaryFormat Default = new();


	/// <inheritdoc/>
	static ICellMapFormatter ICellMapFormatter.Instance => Default;


	/// <inheritdoc/>
	public string ToString(scoped in CellMap cellMap)
	{
		scoped var sb = new StringHandler(81);
		int i;
		var tempInteger = (Int128)cellMap;
		var (low, high) = ((ulong)(tempInteger >> 64 & ulong.MaxValue), (ulong)(tempInteger & ulong.MaxValue));

		var value = low;
		for (i = 0; i < 27; i++, value >>= 1)
		{
			sb.Append(value & 1);
		}
		if (WithSeparator)
		{
			sb.Append(' ');
		}
		for (; i < 41; i++, value >>= 1)
		{
			sb.Append(value & 1);
		}
		for (value = high; i < 54; i++, value >>= 1)
		{
			sb.Append(value & 1);
		}
		if (WithSeparator)
		{
			sb.Append(' ');
		}
		for (; i < 81; i++, value >>= 1)
		{
			sb.Append(value & 1);
		}

		sb.Reverse();
		return sb.ToStringAndClear();
	}
}
