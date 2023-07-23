namespace Sudoku.Text.Formatting;

/// <summary>
/// Represents with a formatter type that formats and parses a <see cref="CellMap"/> instance,
/// converting it into an equivalent <see cref="string"/> value.
/// </summary>
/// <seealso cref="CellMap"/>
public interface ICellMapFormatter : IFormatProvider, ICustomFormatter
{
	/// <inheritdoc cref="IGridFormatter.Instance"/>
	static abstract ICellMapFormatter Instance { get; }


	/// <summary>
	/// Try to format a <see cref="CellMap"/> instance into the specified target-formatted <see cref="string"/> representation.
	/// </summary>
	/// <param name="cellMap">A <see cref="CellMap"/> instance to be formatted.</param>
	/// <returns>A <see cref="string"/> representation as result.</returns>
	string ToString(scoped in CellMap cellMap);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull(nameof(formatType))]
	object? IFormatProvider.GetFormat(Type? formatType) => formatType == GetType() ? this : null;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	string ICustomFormatter.Format(string? format, object? arg, IFormatProvider? formatProvider)
		=> (format, arg, formatProvider) switch
		{
			(null, CellMap target, ICellMapFormatter targetFormatter) => targetFormatter.ToString(target),
			(_, CellMap target, { } targetFormatter) => targetFormatter.GetFormat(GetType()) switch
			{
				ICellMapFormatter cellMapFormatter => cellMapFormatter.ToString(target),
				_ => throw new FormatException("Unexpected error has been encountered due to not aware of target formatter type instance."),
			},
			(_, CellMap target, null) => CellMapFormatterFactory.GetBuiltInFormatter(format) switch
			{
				{ } formatter => formatter.ToString(target),
				_ => GetType().GetCustomAttribute<ExtendedFormatAttribute>() switch
				{
					{ Format: var f } when f == format => ToString(target),
					_ => throw new FormatException($"The target format '{nameof(format)}' is invalid.")
				}
			},
			(_, not CellMap, _) => throw new FormatException($"The argument '{nameof(arg)}' must be of type '{nameof(CellMap)}'.")
		};
}
