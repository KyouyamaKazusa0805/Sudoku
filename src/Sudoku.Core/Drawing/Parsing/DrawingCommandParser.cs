namespace Sudoku.Drawing.Parsing;

/// <summary>
/// Represents a parser that generates a list of drawing items.
/// </summary>
/// <remarks>
/// Please visit <see href="https://sudokustudio.kazusa.tech/user-manual/drawing-command-line">this link</see>
/// to learn more information about drawing command syntax.
/// </remarks>
[TypeImpl(TypeImplFlags.AllObjectMethods)]
public readonly ref partial struct DrawingCommandParser
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
	/// Indicates argument parsers.
	/// </summary>
	private static readonly Dictionary<string, Func<ArgumentParser>> ArgumentParsers = new()
	{
		{ "cell", static () => new CellArgumentParser() },
		{ "candidate", static () => new CandidateArgumentParser() },
		{ "icon", static () => new IconArgumentParser() },
		{ "house", static () => new HouseArgumentParser() },
		{ "chute", static () => new ChuteArgumentParser() },
		{ "baba", static () => new BabaGroupArgumentParser() },
		{ "link", static () => new LinkArgumentParser() }
	};


	/// <summary>
	/// Try to parse the string, split by line separator; return <see langword="false"/> if failed to be parsed.
	/// This method never throws <see cref="FormatException"/>.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="result">The result view.</param>
	/// <param name="parser">The parser. By default it's <see langword="new"/> <see cref="RxCyParser"/>().</param>
	/// <param name="comparison">The comparison. By default it's <see cref="StringComparison.OrdinalIgnoreCase"/>.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the command-line syntax is valid.</returns>
	public bool TryParse(
		string str,
		[NotNullWhen(true)] out View? result,
		CoordinateParser? parser = null,
		StringComparison comparison = StringComparison.OrdinalIgnoreCase
	)
	{
		try
		{
			result = Parse(str, parser, comparison);
			return true;
		}
		catch (FormatException)
		{
			result = null;
			return false;
		}
	}

	/// <summary>
	/// Parses the string, split by line separator.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="parser">The parser. By default it's <see langword="new"/> <see cref="RxCyParser"/>().</param>
	/// <param name="comparison">The comparison. By default it's <see cref="StringComparison.OrdinalIgnoreCase"/>.</param>
	/// <exception cref="FormatException">Throws when a line is invalid.</exception>
	public View Parse(string str, CoordinateParser? parser = null, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
	{
		parser ??= new RxCyParser();

		var result = View.Empty;
		foreach (var line in str.SplitBy('\r', '\n'))
		{
			if (!line.StartsWith("cell") && !line.StartsWith("candidate") && !line.StartsWith("icon")
				&& !line.StartsWith("house") && !line.StartsWith("chute") && !line.StartsWith("link")
				&& !line.StartsWith("baba"))
			{
				// Skip for invalid keyword (like 'load').
				continue;

				// Other keywords should be ignored in order not to throw exceptions.
				//throw new FormatException("Invalid keyword.");
			}

			if (line.SplitBy(' ') is not [var keyword, ['#' or '!' or '&', ..] colorIdentifierString, .. var args])
			{
				throw new FormatException($"Invalid line string: '{line}'.");
			}

			result.AddRange(
				ArgumentParsers.TryGetValue(keyword, out var parserCreator)
					? parserCreator().Parse(args, ParseColorIdentifier(colorIdentifierString), parser)
					: throw new FormatException($"Invalid keyword '{keyword}'.")
			);
		}
		return result;
	}

	/// <summary>
	/// Parses a string and returns the equivalent color identifier.
	/// </summary>
	/// <param name="str">The string to be parsed.</param>
	/// <param name="comparison">The comparison. By default it's <see cref="StringComparison.OrdinalIgnoreCase"/>.</param>
	/// <returns>A <see cref="ColorIdentifier"/> value returned.</returns>
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
			case ['&', var paletteIdChar and (>= 'a' and <= 'f' or >= 'A' and <= 'F')]:
			{
				return new PaletteIdColorIdentifier(10 + paletteIdChar - (paletteIdChar is >= 'A' and <= 'F' ? 'A' : 'a'));
			}
			case ['&', .. var paletteIdString] when int.TryParse(paletteIdString, out var paletteId) && paletteId is >= 1 and <= 15:
			{
				return new PaletteIdColorIdentifier(paletteId);
			}
			default:
			{
				throw new FormatException($"Invalid identifier string: '{str}'.");
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
