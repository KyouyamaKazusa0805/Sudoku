namespace Sudoku.Text.Formatting;

/// <summary>
/// <para>
/// Defines a grid formatter that can convert the current <see cref="Grid"/> instance into a valid and parseable <see cref="string"/> text value
/// representing this instance. This type is used as arguments being passed in method <see cref="Grid.ToString(IGridFormatter)"/>.
/// </para>
/// <!--THE FOLLOWING COMMENT BLOCK IS TEMPORARILY DISABLED! THE TEXT ARE TOO LONG TO BE DISPLAYED ONTO TOOLTIP IN VISUAL STUDIO!-->
/// <para>
/// The built-in derived types are:
/// <list type="table">
/// <listheader>
/// <term>Formatter Type</term>
/// <description>Description</description>
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
/// <para>
/// If you want to control the customized formatting on <see cref="Grid"/> instances, this type will be very useful.
/// For more information about this type and its derived (implemented) types, please visit the documentation comments
/// of members <see cref="Grid.ToString(IGridFormatter)"/>,
/// specially for arguments in those members.
/// </para>
/// <!--
/// <para>
/// In addition, you can also define your own formatter, by using this type, you can just implement this interface:
/// <code><![CDATA[
/// public sealed record CustomFormatter : IGridFormatter
/// {
///     public static readonly CustomFormatter Default = new();
/// 
///	    static IGridFormatter IGridFormatter.Instance => Default;
///	
///     public string ToString(scoped in Grid grid)
///     {
///         // Define your own logic here.
///     }
/// }
/// ]]></code>
/// </para>
/// -->
/// </summary>
/// <seealso cref="Grid"/>
/// <seealso cref="Grid.ToString(IGridFormatter)"/>
public interface IGridFormatter
{
	/// <summary>
	/// Indicates the singleton instance.
	/// </summary>
	/// <remarks>
	/// The main idea of this property is to tell you the implementation type should disallow
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
}
