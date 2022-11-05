namespace Sudoku.Text.Formatting;

/// <summary>
/// <para>
/// Defines a grid formatter that can convert the current <see cref="Grid"/> instance into a valid and parsable <see cref="string"/> text value
/// representing this instance. This type is used as arguments being passed in method <see cref="Grid.ToString(IGridFormatter)"/>.
/// </para>
/// <!--THE FOLLOWING COMMENT BLOCK IS TEMPORARILY DISABLED! THE TEXT ARE TOO LONG TO BE DISPLAYED ONTO TOOLTIP IN VISUAL STUDIO!-->
/// <!--
/// <para>
/// The built-in derived types are:
/// <list type="table">
/// <listheader>
/// <term>Formatter type</term>
/// <description>Description about this type</description>
/// </listheader>
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
/// <term><see cref="SusserFormatEliminationsOnly"/></term>
/// <description>
/// Represents with a formatter using Susser formatting rule. Different with <see cref="SusserFormat"/>,
/// this formatter only contains pre-eliminations. The so-called <b>pre-eliminations</b> means the eliminations
/// that had already been eliminated before the current grid formed.
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
/// <term><see cref="GridMaskFormat"/></term>
/// <description>
/// Represents with a formatter using masks formatting rule, treating the current grid as 81 <see cref="short"/> masks
/// as the inner raw value. For more information about the data structure of type <see cref="Grid"/>, please visit that type.
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
/// </para>
/// -->
/// <para>
/// If you want to control the customized formatting on <see cref="Grid"/> instances, this type will be very useful.
/// For more information about this type and its derived (implemented) types, please visit the documentation comments
/// of members <see cref="Grid.ToString(IGridFormatter)"/> and <see cref="Grid.ToString(string?, IFormatProvider?)"/>,
/// specially for arguments in those members.
/// </para>
/// <para>
/// In addition, you can also define your own formatter, by using this type, you can just implement this interface:
/// <code><![CDATA[
/// // We suggest you use record types instead of classes, in order to define a default-implemented type by compiler.
/// // In addition, using record types can also help you define more properties, especially for initialization-only properties,
/// // by adding your own properties followed by the type name, just like defining a parameter list:
/// //
/// //     public sealed record Record(int Property1, double Property2, string Property3);
/// //                                 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
/// //                                         Initialization-only Properties
/// //
/// // This syntax feature is called "Record", introduced by C# 9; such properties are called "Initialization-only Properties"
/// // (or "Init-only Properties" as its abbreviation), introduced by C# 9.
/// public sealed record CustomFormatter : IGridFormatter // Implements this interface type.
/// {
///     // Define a singleton instance that is the only way to visit the type.
///     public static readonly CustomFormatter Default = new();
/// 
///     // Hides the interface implementation by using explicit interface implementation of static members.
///     // This kind of usage is based on a new C# syntax feature called "DIM of Static Members", introduced by C# 11.
///	    static IGridFormatter IGridFormatter.Instance => Default;
///	
///     // Here we should implement this method, as the default way to create a string representation describing the grid.
///     // Keyword 'scoped' is limited the reference only being scoped inside the method, which means you cannot
///     // assign the reference (no matter whether the reference is read-only or not) outside the method, e.g. as return value,
///     // or assigning it to the field if the type is a ref struct.
///     // This kind of usage is based on a new C# syntax feature called "Ref Fields and Scoping", introduced by C# 11.
///     public string ToString(scoped in Grid grid)
///     {
///         // Define your own logic here.
///     }
/// }
/// ]]></code>
/// </para>
/// </summary>
/// <seealso cref="Grid"/>
/// <seealso cref="Grid.ToString(IGridFormatter)"/>
/// <seealso cref="Grid.ToString(string?, IFormatProvider?)"/>
public interface IGridFormatter : IFormatProvider, ICustomFormatter
{
	/// <summary>
	/// Indicates the singleton instance.
	/// </summary>
	/// <remarks>
	/// The main idea of this property is to tell you the impelmentation type should disallow
	/// users calling parameterless constructors. Although C# does not restrict with it, you should disallow it,
	/// in order to provide users with better utility experience.
	/// </remarks>
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
		=> (format, arg, formatProvider) switch
		{
			(null, Grid targetGrid, IGridFormatter targetFormatter) => targetFormatter.ToString(targetGrid),
			(_, Grid targetGrid, { } targetFormatter) => targetFormatter.GetFormat(GetType()) switch
			{
				IGridFormatter gridFormatter => gridFormatter.ToString(targetGrid),
				_ => throw new FormatException("Unexpected error has been encountered due to not awared of target formatter type instance."),
			},
			(_, Grid targetGrid, null) => GridFormatterFactory.GetBuiltInFormatter(format) switch
			{
				{ } formatter => formatter.ToString(targetGrid),
				_ => GetType().GetCustomAttribute<ExtendedFormatAttribute>() switch
				{
					{ Format: var f } when f == format => ToString(targetGrid),
					_ => throw new FormatException($"The target format '{nameof(format)}' is invalid.")
				}
			},
			(_, not Grid, _) => throw new FormatException($"The argument '{nameof(arg)}' must be of type '{nameof(Grid)}'.")
		};
}
