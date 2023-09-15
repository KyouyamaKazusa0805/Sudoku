using System.Diagnostics;
using System.Text;
using Sudoku.Concepts;

namespace Sudoku.Text.Formatting;

/// <summary>
/// Represents with a grid mask formatter.
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
public sealed record GridMaskFormat(string Separator = ", ") : IGridFormatter
{
	/// <summary>
	/// Indicates the default instance. The properties set are:
	/// <list type="bullet">
	/// <item><see cref="Separator"/>: <c>", "</c></item>
	/// </list>
	/// </summary>
	public static readonly GridMaskFormat Default = new();


	/// <inheritdoc/>
	static IGridFormatter IGridFormatter.Instance => Default;


	/// <inheritdoc/>
	public unsafe string ToString(scoped ref readonly Grid grid)
	{
		scoped var sb = new StringHandler(400);
		sb.AppendRangeWithSeparatorRef(in grid[0], 81, &StringHandler.ElementToStringConverter, Separator);
		return sb.ToStringAndClear();
	}
}
