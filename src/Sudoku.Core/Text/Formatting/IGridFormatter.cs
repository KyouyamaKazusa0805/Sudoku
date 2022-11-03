namespace Sudoku.Text.Formatting;

/// <summary>
/// Defines a grid formatter.
/// </summary>
public interface IGridFormatter : IFormatProvider, ICustomFormatter
{
	/// <summary>
	/// Indicates the singleton instance.
	/// </summary>
	public static abstract IGridFormatter Instance { get; }


	/// <summary>
	/// Try to format a <see cref="Grid"/> instance into the specified target-formatted <see cref="string"/> representation.
	/// </summary>
	/// <param name="grid">A <see cref="Grid"/> instance to be formatted.</param>
	/// <returns>A <see cref="string"/> representation as result.</returns>
	public abstract string ToString(scoped in Grid grid);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull(nameof(formatType))]
	object? IFormatProvider.GetFormat(Type? formatType) => formatType == GetType() ? this : null;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	string ICustomFormatter.Format(string? format, object? arg, IFormatProvider? formatProvider)
		=> (arg, formatProvider) switch
		{
			(Grid targetGrid, IGridFormatter targetFormatter) => targetFormatter.ToString(targetGrid),
			(not Grid, _) => throw new FormatException($"The argument '{nameof(arg)}' must be of type '{nameof(Grid)}'."),
			_ => throw new FormatException($"The argument '{nameof(formatProvider)}' must be of type '{nameof(IGridFormatter)}'.")
		};
}
