namespace Sudoku.Drawing.Parsing;

/// <summary>
/// Represents a cell argument parser.
/// </summary>
internal sealed class CellArgumentParser : ArgumentParser
{
	/// <inheritdoc/>
	public override ReadOnlySpan<ViewNode> Parse(ReadOnlySpan<string> arguments, ColorIdentifier colorIdentifier, CoordinateParser coordinateParser)
		=> from cell in new CellMap(arguments, coordinateParser) select new CellViewNode(colorIdentifier, cell);
}
