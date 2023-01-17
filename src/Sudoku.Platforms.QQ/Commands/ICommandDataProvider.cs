namespace Sudoku.Platforms.QQ.Commands;

using static Constants;

/// <summary>
/// Extracts a type that creates data used by commands.
/// </summary>
internal interface ICommandDataProvider
{
	/// <summary>
	/// Try to fetch the identifier name via the color name.
	/// </summary>
	/// <param name="name">The name of the color.</param>
	/// <returns>The identifier instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Identifier? GetIdentifier(string name)
	{
		if (Enum.TryParse<KnownColor>(name, out var knownColor))
		{
			return f(Color.FromKnownColor(knownColor));
		}

		if (KnownColors.TryGetValue(name, out var dicColor))
		{
			return f(dicColor);
		}

		if (name is ['%', .. var rawColorKind] && KnownKinds.TryGetValue(rawColorKind, out var colorKind))
		{
			return Identifier.FromNamedKind(colorKind);
		}

		if (Patterns.ColorIdPattern().Match(name) is { Success: true, Value: [_, .. var rawId] colorLabel })
		{
			return Identifier.FromId(int.Parse(rawId));
		}

		if (Patterns.ColorHexValuePattern().Match(name) is { Success: true, Value: var colorHtml })
		{
			return f(ColorTranslator.FromHtml(colorHtml));
		}

		return null;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Identifier f(Color c) => Identifier.FromColor(c.A, c.R, c.G, c.B);
	}

	/// <summary>
	/// Try to fetch the coordinate value.
	/// </summary>
	/// <param name="rawCoordinate">The coordinate string value.</param>
	/// <returns>
	/// Returns a value that can be <see cref="CellMap"/>, <see cref="Candidates"/> and <see cref="int"/> value, where:
	/// <list type="table">
	/// <item>
	/// <term><see cref="CellMap"/></term>
	/// <description>The cells parsed if the string value can be parsed as <see cref="CellMap"/>.</description>
	/// </item>
	/// <item>
	/// <term><see cref="Candidates"/></term>
	/// <description>The candidates parsed if the string value can be parsed as <see cref="Candidates"/>.</description>
	/// </item>
	/// <item>
	/// <term><see cref="int"/></term>
	/// <description>The house parsed if the string value can be parsed as house index.</description>
	/// </item>
	/// </list>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static OneOf<CellMap, Candidates, int> GetCoordinate(string rawCoordinate)
	{
		const StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

		if (RxCyNotation.TryParseCandidates(rawCoordinate, out var candidates1))
		{
			return candidates1;
		}

		if (RxCyNotation.TryParseCells(rawCoordinate, out var cells2))
		{
			return cells2;
		}

		if (K9Notation.TryParseCells(rawCoordinate, out var cells1))
		{
			return cells1;
		}

		if (Patterns.ChineseHouseIndexPattern().Match(rawCoordinate) is { Success: true, Value: var parts }
			&& parts.Split(' ', splitOptions) is [ [var houseNotation], [var label]])
		{
			return houseNotation switch { '\u884c' => 9, '\u5217' => 18, _ => 0 } + (label - '1');
		}

		return default;
	}

	/// <summary>
	/// Try to fetch the coordinate value.
	/// </summary>
	/// <param name="rawCoordinate">The coordinate string value.</param>
	/// <returns>The cell index parsed. If failed to be parsed, <see langword="null"/> will be returned.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int? GetCell(string rawCoordinate)
	{
		if (RxCyNotation.TryParseCell(rawCoordinate, out var cell2))
		{
			return cell2;
		}

		if (K9Notation.TryParseCell(rawCoordinate, out var cell1))
		{
			return cell1;
		}

		return null;
	}

	/// <summary>
	/// Gets the time limit for a single gaming.
	/// </summary>
	/// <param name="difficultyLevel">The difficulty level of the puzzle.</param>
	/// <returns>The time limit.</returns>
	/// <exception cref="NotSupportedException">Throws when the specified argument value is not supported.</exception>
	internal static TimeSpan GetGamingTimeLimit(DifficultyLevel difficultyLevel)
		=> difficultyLevel switch
		{
			DifficultyLevel.Easy => 3.Minutes(),
			DifficultyLevel.Moderate => 5.Minutes(),
			DifficultyLevel.Hard => 7.Minutes(),
			_ => throw new NotSupportedException("The specified difficulty is not supported.")
		};
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="constant"]'/>
file static class Constants
{
	/// <summary>
	/// The table of known colors.
	/// </summary>
	internal static readonly Dictionary<string, Color> KnownColors = new()
	{
		{ R["ColorRed"]!, Color.Red },
		{ R["ColorGreen"]!, Color.Green },
		{ R["ColorBlue"]!, Color.Blue },
		{ R["ColorYellow"]!, Color.Yellow },
		{ R["ColorBlack"]!, Color.Black },
		{ R["ColorPurple"]!, Color.Purple },
		{ R["ColorSkyblue"]!, Color.SkyBlue },
		{ R["ColorDarkYellow"]!, Color.Gold },
		{ R["ColorDarkGreen"]!, Color.DarkGreen },
		{ R["ColorPink"]!, Color.Pink },
		{ R["ColorOrange1"]!, Color.Orange },
		{ R["ColorOrange2"]!, Color.Orange },
		{ R["ColorGray"]!, Color.Gray }
	};

	/// <summary>
	/// The table of known kinds.
	/// </summary>
	internal static readonly Dictionary<string, DisplayColorKind> KnownKinds = new()
	{
		{ R["ColorKind_Normal"]!, DisplayColorKind.Normal },
		{ R["ColorKind_Aux1"]!, DisplayColorKind.Auxiliary1 },
		{ R["ColorKind_Aux2"]!, DisplayColorKind.Auxiliary2 },
		{ R["ColorKind_Aux3"]!, DisplayColorKind.Auxiliary3 },
		{ R["ColorKind_Assignment"]!, DisplayColorKind.Assignment },
		{ R["ColorKind_Elimination"]!, DisplayColorKind.Elimination },
		{ R["ColorKind_Exofin"]!, DisplayColorKind.Exofin },
		{ R["ColorKind_Endofin"]!, DisplayColorKind.Endofin },
		{ R["ColorKind_Cannibalism"]!, DisplayColorKind.Cannibalism },
		{ R["ColorKind_Als1"]!, DisplayColorKind.AlmostLockedSet1 },
		{ R["ColorKind_Als2"]!, DisplayColorKind.AlmostLockedSet2 },
		{ R["ColorKind_Als3"]!, DisplayColorKind.AlmostLockedSet3 },
		{ R["ColorKind_Als4"]!, DisplayColorKind.AlmostLockedSet4 },
		{ R["ColorKind_Als5"]!, DisplayColorKind.AlmostLockedSet5 }
	};
}
