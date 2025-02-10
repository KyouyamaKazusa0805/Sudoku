namespace Sudoku.Drawing.Parsing;

/// <summary>
/// Represents a candidate argument parser.
/// </summary>
internal sealed class CandidateArgumentParser : ArgumentParser
{
	/// <inheritdoc/>
	public override ReadOnlySpan<ViewNode> Parse(ReadOnlySpan<string> arguments, ColorIdentifier colorIdentifier, CoordinateParser coordinateParser)
		=> from candidate in new CandidateMap(arguments, coordinateParser) select new CandidateViewNode(colorIdentifier, candidate);
}
