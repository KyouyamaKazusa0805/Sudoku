using Sudoku.Concepts;

namespace Sudoku.Text.SudokuGrid;

/// <summary>
/// Represents a grid converter type that can converts into a <see cref="string"/> text representing the equivalent <see cref="Grid"/> instance.
/// </summary>
/// <seealso cref="Grid"/>
public abstract record GridConverter : SpecifiedConceptConverter<Grid>
{
	/// <inheritdoc cref="SpecifiedConceptConverter{T}.Converter"/>
	public abstract GridNotationConverter TargetConverter { get; }

	/// <inheritdoc/>
	public sealed override Func<Grid, string> Converter => grid => TargetConverter(in grid);
}
