namespace Sudoku.Drawing.Parsing;

/// <summary>
/// Represents a parser that generates a list of drawing items.
/// </summary>
[TypeImpl(TypeImplFlags.AllObjectMethods)]
public readonly ref partial struct DrawingParser
{
	/// <summary>
	/// Indicates the well-known identifiers, and their own key used in parsing.
	/// </summary>
	private static readonly (string[] Keys, WellKnownColorIdentifierKind Kind)[] WellKnownIdentifiers = [
		(["normal", "n", "0"], WellKnownColorIdentifierKind.Normal),
		(["auxiliary1", "aux1", "1"], WellKnownColorIdentifierKind.Auxiliary1),
		(["auxiliary2", "aux2", "2"], WellKnownColorIdentifierKind.Auxiliary2),
		(["auxiliary3", "aux3", "3"], WellKnownColorIdentifierKind.Auxiliary3),
		(["assignment", "a", "4"], WellKnownColorIdentifierKind.Assignment),
		(["overlapped_assignment", "overlapped", "o", "5"], WellKnownColorIdentifierKind.OverlappedAssignment),
		(["elimination", "elim", "e", "6"], WellKnownColorIdentifierKind.Elimination),
		(["cannibalism", "cannibal", "c", "7"], WellKnownColorIdentifierKind.Cannibalism),
		(["exofin", "f", "8"], WellKnownColorIdentifierKind.Exofin),
		(["endofin", "ef", "9"], WellKnownColorIdentifierKind.Endofin),
		(["link", "l", "10"], WellKnownColorIdentifierKind.Link),
		(["almost_locked_set1", "als1", "11"], WellKnownColorIdentifierKind.AlmostLockedSet1),
		(["almost_locked_set2", "als2", "12"], WellKnownColorIdentifierKind.AlmostLockedSet2),
		(["almost_locked_set3", "als3", "13"], WellKnownColorIdentifierKind.AlmostLockedSet3),
		(["almost_locked_set4", "als4", "14"], WellKnownColorIdentifierKind.AlmostLockedSet4),
		(["almost_locked_set5", "als5", "15"], WellKnownColorIdentifierKind.AlmostLockedSet5),
		(["rectangle1", "rect1", "r1", "16"], WellKnownColorIdentifierKind.Rectangle1),
		(["rectangle2", "rect2", "r2", "17"], WellKnownColorIdentifierKind.Rectangle2),
		(["rectangle3", "rect3", "r3", "18"], WellKnownColorIdentifierKind.Rectangle3)
	];


	/// <summary>
	/// Parses the string, split by line separator.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="parser">The parser. By default it's <see langword="new"/> <see cref="RxCyParser"/>().</param>
	/// <param name="comparison">The comparison. By default it's <see cref="StringComparison.OrdinalIgnoreCase"/>.</param>
	/// <exception cref="InvalidOperationException">Throws when a line is invalid.</exception>
	public View Parse(string str, CoordinateParser? parser = null, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
	{
		parser ??= new RxCyParser();

		var result = View.Empty;
		foreach (var line in str.SplitBy('\r', '\n'))
		{
			if (line.SplitBy(' ') is not [var keyword, ['#' or '!' or '&', ..] colorIdentifierString, .. var args])
			{
				throw new InvalidOperationException($"Invalid line string: '{line}'.");
			}

			var identifier = ParseColorIdentifier(colorIdentifierString);
			if (keyword.Equals("cell", comparison))
			{
				foreach (var cell in new CellMap(args, parser))
				{
					result.Add(new CellViewNode(identifier, cell));
				}
			}
			if (keyword.Equals("candidate", comparison))
			{
				foreach (var candidate in new CandidateMap(args, parser))
				{
					result.Add(new CandidateViewNode(identifier, candidate));
				}
			}
			if (keyword.Equals("icon", comparison) && args is [var iconKindString, .. var iconArgs])
			{
				Func<Cell, IconViewNode> creator = iconKindString.ToLower() switch
				{
					"circle" => cell => new CircleViewNode(identifier, cell),
					"cross" => cell => new CrossViewNode(identifier, cell),
					"diamond" => cell => new DiamondViewNode(identifier, cell),
					"heart" => cell => new HeartViewNode(identifier, cell),
					"square" => cell => new SquareViewNode(identifier, cell),
					"star" => cell => new StarViewNode(identifier, cell),
					"triangle" => cell => new TriangleViewNode(identifier, cell),
					_ => throw new InvalidOperationException($"Invalid icon kind string: '{iconKindString}'.")
				};
				foreach (var cell in new CellMap(iconArgs, parser))
				{
					result.Add(creator(cell));
				}
			}
			if (keyword.Equals("house", comparison))
			{
				var houses = 0;
				foreach (var arg in args)
				{
					houses |= parser.HouseParser(arg);
				}
				foreach (var house in houses)
				{
					result.Add(new HouseViewNode(identifier, house));
				}
			}
			if (keyword.Equals("chute", comparison))
			{
				var chutes = new List<Chute>(6);
				foreach (var arg in args)
				{
					chutes.AddRange(parser.ChuteParser(arg));
				}
				foreach (var chute in chutes)
				{
					result.Add(new ChuteViewNode(identifier, chute.Index));
				}
			}
			if (keyword.Equals("baba", comparison) && args is [[var babaGroupingChar], .. var babaGroupingArgs])
			{
				foreach (var cell in new CellMap(args, parser))
				{
					result.Add(new BabaGroupViewNode(identifier, cell, babaGroupingChar, Grid.MaxCandidatesMask));
				}
			}
			if (keyword.Equals("link", comparison) && args is [var linkKeyword, .. { Length: var linkArgsLength } linkArgs])
			{
				linkKeyword = linkKeyword.ToLower();
				Func<dynamic, dynamic, dynamic?, ILinkViewNode> creator = linkKeyword switch
				{
					"cell" => (start, end, _) => new CellLinkViewNode(identifier, start, end),
					"chain" => (start, end, isStrongLink) => new ChainLinkViewNode(identifier, start, end, isStrongLink),
					"conjugate" => (start, end, digit) => new ConjugateLinkViewNode(identifier, start, end, digit),
					_ => throw new InvalidOperationException($"Invalid link kind string: '{linkKeyword}'.")
				};

				for (var i = 0; i < linkArgsLength;)
				{
					var left = linkArgs[i];
					var right = linkArgs[i + 1];
					var extra = i + 2 < linkArgsLength ? linkArgs[i + 2] : null;
					var (leftArg, rightArg, extraArg) = linkKeyword switch
					{
						"cell" => (parser.CellParser(left), parser.CellParser(right), null),
						_ when extra is null => throw new InvalidOperationException("Extra argument expected."),
						"chain" => (parser.CandidateParser(left), parser.CandidateParser(right), bool.Parse(extra)),
						_ => ((object, object, object?))(parser.CellParser(left), parser.CellParser(right), Digit.Parse(extra))
					};
					result.Add((ViewNode)creator(leftArg, rightArg, extraArg));

					i += linkKeyword switch { "cell" => 2, _ => 3 };
				}
			}
		}
		return result;
	}

	/// <summary>
	/// Parses a string and returns the equivalent color identifier.
	/// </summary>
	/// <param name="str">The string to be parsed.</param>
	/// <param name="comparison">The comparison. By default it's <see cref="StringComparison.OrdinalIgnoreCase"/>.</param>
	/// <returns>A <see cref="ColorIdentifier"/> value returned.</returns>
	/// <remarks>
	/// Identifier Syntax:
	/// <code><![CDATA[
	/// identifier_syntax
	///   : '#' hex_color_string
	///   | '!' alias_string_or_id
	///   | '&' palette_id
	///   ;
	///
	/// hex_color_string
	///   : hex_text{6}
	///   | hex_text{8}
	///   ;
	///
	/// hex_text
	///   : '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9'
	///   | 'a' | 'b' | 'c' | 'd' | 'e' | 'f'
	///   ;
	///
	/// alias_string_or_id
	///   : 'normal' | 'n'
	///   | ('auxiliary' | 'aux') ('1' | '2' | '3')
	///   | 'assignment' | 'a'
	///   | 'overlapped_assignment' | 'overlapped' | 'o'
	///   | 'elimination' | 'elim' | 'e'
	///   | 'cannibalism' | 'cannibal' | 'c'
	///   | 'exofin' | 'f'
	///   | 'endofin' | 'ef'
	///   | 'link' | 'l'
	///   | ('almost_locked_set' | 'als') ('1' | '2' | '3' | '4' | '5')
	///   | ('rectangle' | 'r') ('1' | '2' | '3')
	///   ;
	///
	/// palette_id
	///   : '1' | '2' | '3' | '4' | '5'
	///   | '6' | '7' | '8' | '9' | '10'
	///   | '11' | '12' | '13' | '14' | '15'
	///   | 'a' | 'b' | 'c' | 'd' | 'e' | 'f'
	///   ;
	/// ]]></code>
	/// </remarks>
	private ColorIdentifier ParseColorIdentifier(string str, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
	{
		switch (str)
		{
			case ['#', .. { Length: 6 } hexColorString]:
			{
				var r = (byte)Convert.ToInt32(hexColorString[..2], 16);
				var g = (byte)Convert.ToInt32(hexColorString[2..4], 16);
				var b = (byte)Convert.ToInt32(hexColorString[4..], 16);
				return new ColorColorIdentifier(255, r, g, b);
			}
			case ['#', .. { Length: 8 } hexColorString]:
			{
				var a = (byte)Convert.ToInt32(hexColorString[..2], 16);
				var r = (byte)Convert.ToInt32(hexColorString[2..4], 16);
				var g = (byte)Convert.ToInt32(hexColorString[4..6], 16);
				var b = (byte)Convert.ToInt32(hexColorString[6..], 16);
				return new ColorColorIdentifier(a, r, g, b);
			}
			case ['!', .. var aliasOrIdString] when getFoundIndex(aliasOrIdString) is var foundKind and not (WellKnownColorIdentifierKind)(-1):
			{
				return new WellKnownColorIdentifier(foundKind);
			}
			case ['&', var paletteIdChar and (>= 'a' and <= 'f' or >= 'A' or <= 'F')]:
			{
				return new PaletteIdColorIdentifier(10 + paletteIdChar - (paletteIdChar is >= 'A' and <= 'F' ? 'A' : 'a'));
			}
			case ['&', .. var paletteIdString] when int.TryParse(paletteIdString, out var paletteId) && paletteId is >= 1 and <= 15:
			{
				return new PaletteIdColorIdentifier(paletteId);
			}
			default:
			{
				throw new InvalidOperationException($"Invalid identifier string: '{str}'.");
			}
		}


		WellKnownColorIdentifierKind getFoundIndex(string aliasOrIdString)
		{
			foreach (var (keys, value) in WellKnownIdentifiers)
			{
				foreach (var key in keys)
				{
					if (key.Equals(aliasOrIdString, comparison))
					{
						return value;
					}
				}
			}
			return (WellKnownColorIdentifierKind)(-1);
		}
	}
}
