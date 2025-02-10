namespace Sudoku.Drawing.Parsing;

/// <summary>
/// Represents a chute argument parser.
/// </summary>
internal sealed class ChuteArgumentParser : ArgumentParser
{
	/// <inheritdoc/>
	public override ReadOnlySpan<ViewNode> Parse(ReadOnlySpan<string> arguments, ColorIdentifier colorIdentifier, CoordinateParser coordinateParser)
		=>
		from arg in arguments
		select coordinateParser.ChuteParser(arg).ToArray() into chutes
		from chute in chutes
		select new ChuteViewNode(colorIdentifier, chute.Index);
}
