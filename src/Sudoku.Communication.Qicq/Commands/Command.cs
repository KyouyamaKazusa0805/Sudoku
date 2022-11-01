namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Defines a command.
/// </summary>
internal abstract class Command
{
	/// <summary>
	/// The table of known colors.
	/// </summary>
	private static readonly Dictionary<string, Color> KnownColors = new()
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
	private static readonly Dictionary<string, DisplayColorKind> KnownKinds = new()
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


	/// <summary>
	/// Indicates the environment command that the current command relies on.
	/// </summary>
	public virtual string? EnvironmentCommand { get; } = null;

	/// <summary>
	/// Indicates the prefix.
	/// </summary>
	public virtual string[] Prefixes { get; } = new[] { "!", "\uff01" };


	/// <summary>
	/// Execute the command.
	/// </summary>
	/// <param name="args">The arguments.</param>
	/// <param name="e">The event arguments.</param>
	/// <returns>
	/// Returns a task instance that returns a <see cref="bool"/> value indicating whether the operation executed successfully.
	/// </returns>
	public async Task<bool> ExecuteAsync(string args, GroupMessageReceiver e)
	{
		var otherArgs = Prefixes.FirstOrDefault(args.StartsWith) switch
		{
			{ } prefix when args.IndexOf(prefix) + 1 is var i && i < args.Length => args[i..],
			_ => null
		};

		if (otherArgs is null)
		{
			return false;
		}

		if (EnvironmentCommandExecuting != EnvironmentCommand)
		{
			return false;
		}

		return await ExecuteCoreAsync(otherArgs, e);
	}

	/// <inheritdoc cref="ExecuteAsync(string, GroupMessageReceiver)"/>
	protected abstract Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e);


	/// <summary>
	/// Try to fetch the identifier name via the color name.
	/// </summary>
	/// <param name="name">The name of the color.</param>
	/// <returns>The identifier instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static Identifier? GetIdentifier(string name)
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

		if (name.Match("""#([1-9]|1[0-5])""") is [_, .. var rawId] colorLabel)
		{
			return Identifier.FromId(int.Parse(rawId));
		}

		if (name.Match("""#[\dA-Fa-f]{6}([\dA-Fa-f]{2})?""") is { } colorHtml)
		{
			return f(ColorTranslator.FromHtml(colorHtml));
		}

		return null;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Identifier f(Color c) => Identifier.FromColor(c.A, c.R, c.G, c.B);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static OneOf<CellMap, Candidates, int> GetCoordinate(string rawCoordinate)
	{
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

#pragma warning disable IDE0055
		if (rawCoordinate.Match("""[\u884c\u5217\u5bab]\s*[1-9]""") is { } parts
			&& parts.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) is [[var houseNotation], [var label]])
#pragma warning restore IDE0055
		{
			return houseNotation switch { '\u884c' => 9, '\u5217' => 18, _ => 0 } + (label - '1');
		}

		return default;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static int? GetCell(string rawCoordinate)
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
}
