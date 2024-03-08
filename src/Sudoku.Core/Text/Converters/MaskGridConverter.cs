namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a type that converts the grid into an equivalent <see cref="string"/> representation using mask displaying rule.
/// </summary>
/// <param name="Separator">
/// <para>Indicates the mask separator.</para>
/// <para>The default value is a comma followed by a space: <c>", "</c>.</para>
/// </param>
/// <remarks>
/// Please note that the method cannot be called with a correct behavior using
/// <see cref="DebuggerDisplayAttribute"/> to output. It seems that Visual Studio
/// doesn't print correct values when indices of this grid aren't 0. In other words,
/// when we call this method using <see cref="DebuggerDisplayAttribute"/>, only <c>grid[0]</c>
/// can be output correctly, and other values will be incorrect: they're always 0.
/// </remarks>
public sealed record MaskGridConverter(string Separator = ", ") : IConceptConverter<Grid>
{
	/// <inheritdoc/>
	public unsafe FuncRefReadOnly<Grid, string> Converter
		=> (scoped ref readonly Grid grid) =>
			new StringBuilder(400)
				.AppendRangeWithSeparator(in grid[0], 81, &CommonMethods.ToStringConverter, Separator)
				.ToString();
}
