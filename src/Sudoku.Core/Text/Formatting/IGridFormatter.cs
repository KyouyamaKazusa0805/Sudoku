namespace Sudoku.Text.Formatting;

/// <summary>
/// Defines a grid formatter that can convert the current <see cref="Grid"/> instance into a valid and parsable <see cref="string"/> text value
/// representing this instance. This type is used as arguments being passed in method <see cref="Grid.ToString(IGridFormatter)"/>.
/// The built-in derived types are:
/// <list type="table">
/// <item>
/// <term><see cref="SusserFormat"/> (Recommend)</term>
/// <description>
/// Represents with a formatter using Susser formatting rule.
/// </description>
/// </item>
/// <item>
/// <term><see cref="SusserFormatTreatingValuesAsGivens"/></term>
/// <description>
/// Represents with a formatter using Susser formatting rule. Different with <see cref="SusserFormat"/>,
/// this formatter will remove all modifiable tokens.
/// </description>
/// </item>
/// <item>
/// <term><see cref="PencilMarkFormat"/> (Recommend)</term>
/// <description>
/// Represents with a formatter using a multiple-line formatting rule, displaying candidates as a list of digits.
/// This formatter is useful on globalized Sudoku BBS.
/// </description>
/// </item>
/// <item>
/// <term><see cref="HodokuLibraryFormat"/></term>
/// <description>
/// Represents with a formatter using Hodoku Library formatting rule.
/// </description>
/// </item>
/// <item>
/// <term><see cref="MultipleLineFormat"/></term>
/// <description>
/// Represents with a formatter using multiple-line formatting rule, without displaying candidates.
/// </description>
/// </item>
/// <item>
/// <term><see cref="SukakuFormat"/></term>
/// <description>
/// Represents with a formatter using Sukaku game formatting rule, treating all cells (no matter what kind of the cell it is) as candidate lists.
/// </description>
/// </item>
/// <item>
/// <term><see cref="ExcelFormat"/></term>
/// <description>
/// Represents with a formatter using Excel formatting rule, using multiple lines to distinct sudoku lines
/// and using tab characters <c>'\t'</c> as separators inserted into a pair of adjacent cells.
/// </description>
/// </item>
/// <item>
/// <term><see cref="OpenSudokuFormat"/></term>
/// <description>
/// Represents with a formatter using OpenSudoku formatting rule, using a triplet to display the detail of a cell,
/// separated by pipe operator <c>'|'</c>.
/// </description>
/// </item>
/// </list>
/// If you want to control the customized formatting on <see cref="Grid"/> instances, this type will be very useful.
/// For more information about this type and its derived (implemented) types, please visit the documentation comments
/// of members <see cref="Grid.ToString(IGridFormatter)"/> and <see cref="Grid.ToString(string?, IFormatProvider?)"/>,
/// specially for arguments in those members.
/// </summary>
/// <seealso cref="Grid"/>
/// <seealso cref="Grid.ToString(IGridFormatter)"/>
/// <seealso cref="Grid.ToString(string?, IFormatProvider?)"/>
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
