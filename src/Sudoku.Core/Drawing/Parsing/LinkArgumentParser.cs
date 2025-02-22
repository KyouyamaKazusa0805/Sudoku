namespace Sudoku.Drawing.Parsing;

/// <summary>
/// Represents link argument parser.
/// </summary>
internal sealed class LinkArgumentParser : ArgumentParser
{
	/// <inheritdoc/>
	public override ReadOnlySpan<ViewNode> Parse(ReadOnlySpan<string> arguments, ColorIdentifier colorIdentifier, CoordinateParser coordinateParser)
	{
		if (arguments is not [var linkKeyword, .. { Length: var linkArgsLength } linkArgs])
		{
			throw new FormatException("Invalid link command.");
		}

		linkKeyword = linkKeyword.ToLower();
		Func<object, object, object?, ILinkViewNode> creator = linkKeyword switch
		{
			"cell"
				=> (start, end, _) => new CellLinkViewNode(colorIdentifier, (Cell)start, (Cell)end),
			"chain"
				=> (start, end, isStrong) => new ChainLinkViewNode(colorIdentifier, (CandidateMap)start, (CandidateMap)end, (bool)isStrong!),
			"conjugate"
				=> (start, end, digit) => new ConjugateLinkViewNode(colorIdentifier, (Cell)start, (Cell)end, (Digit)digit! + 1),
			_
				=> throw new FormatException($"Invalid link kind string: '{linkKeyword}'.")
		};

		var result = new List<ViewNode>();
		for (var i = 0; i < linkArgsLength;)
		{
			var left = linkArgs[i];
			var right = linkArgs[i + 1];
			var extra = i + 2 < linkArgsLength ? linkArgs[i + 2] : null;
			var (leftArg, rightArg, extraArg) = linkKeyword switch
			{
				"cell"
					=> (coordinateParser.CellParser(left)[0], coordinateParser.CellParser(right)[0], null),
				_ when extra is null
					=> throw new FormatException("Extra argument expected."),
				"chain"
					=> (coordinateParser.CandidateParser(left), coordinateParser.CandidateParser(right), extra == "="),
				_
					=> ((object, object, object?))(coordinateParser.CellParser(left)[0], coordinateParser.CellParser(right)[0], Digit.Parse(extra))
			};
			result.Add((ViewNode)creator(leftArg, rightArg, extraArg));

			i += linkKeyword switch { "cell" => 2, _ => 3 };
		}
		return result.AsSpan();
	}
}
