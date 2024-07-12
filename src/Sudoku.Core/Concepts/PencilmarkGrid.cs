namespace Sudoku.Concepts;

/// <summary>
/// Represents a sudoku grid that can be only used by users to append pencilmarks.
/// </summary>
[InlineArray(Grid.CellsCount)]
public struct PencilmarkGrid// : IGrid<PencilmarkGrid>
{
	/// <inheritdoc cref="Grid._values"/>
	[SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private Mask _values;
}
