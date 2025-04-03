namespace Sudoku.Concepts;

/// <summary>
/// Provides a way to output an instance of type <see cref="IGrid{TSelf}"/>, for bits.
/// </summary>
/// <seealso cref="IGrid{TSelf}"/>
public static class GridBitOutput
{
	/// <summary>
	/// Returns the binary text that represents a <typeparamref name="TGrid"/> instance, with text colorized.
	/// </summary>
	/// <typeparam name="TGrid">The type of grid.</typeparam>
	/// <param name="this">The instance.</param>
	/// <returns>The string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetTextColorized<TGrid>(in TGrid @this) where TGrid : unmanaged, IGrid<TGrid>
		=> GetTextCore(@this, "\e[90m", "\e[33m", null);

	/// <summary>
	/// Returns the binary text that represents a <typeparamref name="TGrid"/> instance, without colorized.
	/// </summary>
	/// <typeparam name="TGrid">The type of grid.</typeparam>
	/// <param name="this">The instance.</param>
	/// <returns>The string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetText<TGrid>(in TGrid @this) where TGrid : unmanaged, IGrid<TGrid>
		=> GetTextCore(@this, null, null, null);

	/// <summary>
	/// The core method to output text.
	/// </summary>
	private static string GetTextCore<TGrid>(in TGrid @this, string? part1, string? part2, string? part3)
		where TGrid : unmanaged, IGrid<TGrid>
	{
		var masks = @this.Elements;
		var sb = new StringBuilder();

		var part1End = part1 is null ? null : "\e[0m";
		var part2End = part2 is null ? null : "\e[0m";
		var part3End = part3 is null ? null : "\e[0m";
		for (var i = 0; i < 81; i++)
		{
			var bits = Convert.ToString(masks[i], 2).PadLeft(16, '0');
			sb.Append($"{part1}{bits[..4]}{part1End}{part2}{bits[4..7]}{part2End}{part3}{bits[7..]}{part3End} ");
			if ((i + 1) % 9 == 0)
			{
				sb.AppendLine();
			}
		}
		return sb.ToString();
	}
}
