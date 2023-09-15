using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Concepts;
using Sudoku.Text.Formatting;

namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a notation that represents for a <see cref="Grid"/> instance.
/// </summary>
/// <seealso cref="Grid"/>
public sealed partial class GridNotation : INotation<GridNotation, Grid, GridNotation.Kind>
{
	/// <summary>
	/// Same as <see cref="Grid.Parse(string)"/>.
	/// </summary>
	/// <param name="text">The text to be parsed.</param>
	/// <returns>The target <see cref="Grid"/> instance.</returns>
	/// <seealso cref="Grid.Parse(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string text) => new GridParser(text).Parse();

	/// <inheritdoc/>
	/// <exception cref="NotSupportedException">Throws when the argument <paramref name="notation"/> is not supported.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="notation"/> is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string text, Kind notation)
		=> new GridParser(text).Parse(
			notation switch
			{
				Kind.Susser or Kind.SusserTreatingValuesAsGivens or Kind.HodokuLibrary => GridParsingOption.Susser,
				Kind.SusserElimination => throw new NotSupportedException("The current value is not supported."),
				Kind.Pencilmark => GridParsingOption.PencilMarked,
				Kind.MultipleLine => GridParsingOption.Table,
				Kind.Sukaku => GridParsingOption.Sukaku,
				Kind.Excel => GridParsingOption.Excel,
				Kind.OpenSudoku => GridParsingOption.OpenSudoku,
				_ => throw new ArgumentOutOfRangeException(nameof(notation))
			}
		);

	/// <summary>
	/// Same as <see cref="Grid.ToString()"/>.
	/// </summary>
	/// <param name="value">The grid to be output.</param>
	/// <returns>The string representation of the grid.</returns>
	/// <seealso cref="Grid.ToString()"/>
	[ExplicitInterfaceImpl(typeof(INotation<,>))]
	public static string ToString(scoped ref readonly Grid value) => value.ToString();

	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"/>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="notation"/> is not defined.</exception>
	[ExplicitInterfaceImpl(typeof(INotation<,,>))]
	public static string ToString(scoped ref readonly Grid value, Kind notation)
		=> value.ToString(
			notation switch
			{
				Kind.Susser => SusserFormat.Default,
				Kind.SusserTreatingValuesAsGivens => SusserFormatTreatingValuesAsGivens.Default,
				Kind.SusserElimination => SusserFormatEliminationsOnly.Default,
				Kind.Pencilmark => PencilMarkFormat.Default,
				Kind.HodokuLibrary => HodokuLibraryFormat.Default,
				Kind.MultipleLine => MultipleLineFormat.Default,
				Kind.Sukaku => SukakuFormat.Default,
				Kind.Excel => ExcelFormat.Default,
				Kind.OpenSudoku => OpenSudokuFormat.Default,
				_ => throw new ArgumentOutOfRangeException(nameof(notation))
			}
		);
}
