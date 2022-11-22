#define NORMAL_DISTRIBUTION

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

		if (rawCoordinate.Match("""[\u884c\u5217\u5bab]\s*[1-9]""") is { } parts
			&& parts.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) is
			[
				[var houseNotation],
				[var label]
			])
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
	/// Generates a value that describes the experience point that the current user can be earned.
	/// </summary>
	/// <param name="distribution">The distribution.</param>
	/// <returns>The value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int GenerateOriginalValueEarned(Distribution distribution = Distribution.Normal)
	{
		switch (distribution)
		{
			case Distribution.Constant:
			{
				return 4;
			}
			case Distribution.Exponent:
			{
				var a = new[] { 2, 3, 4, 6, 12 };
				return a[
					Rng.Next(0, 10000) switch
					{
						< 5000 => 0,
						>= 5000 and < 7500 => 1,
						>= 7500 and < 8750 => 2,
						>= 8750 and < 9375 => 3,
						_ => 4
					}
				];
			}
			case Distribution.Normal:
			{
				const double sigma = 2.5, mu = 0;
				var table = new[] { -1, 1, 2, 3, 4, 5, 6, 8, 10, 12, 16 };
				return getNextRandomGaussian(sigma, mu, table);


				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				static int getNextRandomGaussian(double sigma, double mu, int[] table)
				{
					var u1 = 1.0 - Rng.NextDouble();
					var u2 = 1.0 - Rng.NextDouble();
					var target = (int)(sigma * Sqrt(-2.0 * Log(u1)) * Sin(2.0 * PI * u2) + mu + (table.Length - 1) / 2.0);
					return table[Clamp(target, 0, table.Length - 1)];
				}
			}
			default:
			{
				throw new ArgumentOutOfRangeException(nameof(distribution));
			}
		}
	}

	/// <summary>
	/// Gets the factor that describes whether today is weekend.
	/// </summary>
	/// <returns>The factor value. If today is weekend, 2; otherwise 1.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int GetWeekendFactor() => DateTime.Today.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? 2 : 1;

	/// <summary>
	/// Generates a value that describes the experience point that the current user can be earned.
	/// </summary>
	/// <param name="continuousDaysCount">The number of continuous days that the user has already been checking-in.</param>
	/// <returns>The value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int GenerateValueEarned(int continuousDaysCount)
	{
		var earned = GenerateOriginalValueEarned();
		var level = continuousDaysCount / 7;
		return (int)Round(earned * (level * .2 + 1)) * GetWeekendFactor();
	}

	/// <summary>
	/// Gets the experience point that can be earned by a player in a single gaming.
	/// </summary>
	/// <param name="targetCells">The target cells.</param>
	/// <param name="difficultyLevel">The difficulty level of the puzzle.</param>
	/// <returns>The experience point.</returns>
	/// <exception cref="NotSupportedException">Throws when the specified difficulty level or target cells count is not supported.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the specified difficulty level is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int GetEachGamingExperiencePointCanBeEarned(int[] targetCells, DifficultyLevel difficultyLevel)
	{
		var @base = difficultyLevel switch
		{
			DifficultyLevel.Easy => 12,
			DifficultyLevel.Moderate => 18,
			_ when Enum.IsDefined(difficultyLevel) => throw new NotSupportedException("Other kinds of difficulty levels are not supported."),
			_ => throw new ArgumentOutOfRangeException(nameof(difficultyLevel))
		};

		var answeredValuesExtra = targetCells.Length switch
		{
			2 => 0,
			3 => 1,
			5 => 2,
			_ => throw new NotSupportedException("The specified number of target cells is not supported.")
		};

		return (@base + answeredValuesExtra) * GetWeekendFactor();
	}

	/// <summary>
	/// Gets the time limit for a single gaming.
	/// </summary>
	/// <param name="targetCells">
	/// The target cells. The valid length must be 2, 3 or 5; otherwise, a <see cref="NotSupportedException"/> instance will be thrown.
	/// </param>
	/// <param name="difficultyLevel">The difficulty level of the puzzle.</param>
	/// <returns>The time limit.</returns>
	/// <exception cref="NotSupportedException">Throws when the specified argument value is not supported.</exception>
	internal static TimeSpan GetGamingTimeLimit(int[] targetCells, DifficultyLevel difficultyLevel)
	{
		var @base = targetCells.Length switch
		{
			2 => 3.Minutes(),
			3 => 5.Minutes(),
			5 => 7.Minutes(),
			_ => throw new NotSupportedException("The specified length of the target solution data is not supported.")
		};

		var difficultyExtra = difficultyLevel switch
		{
			DifficultyLevel.Easy => TimeSpan.Zero,
			DifficultyLevel.Moderate => 30.Seconds(),
			_ => throw new NotSupportedException("The specified difficulty is not supported.")
		};

		return @base + difficultyExtra;
	}
}

/// <summary>
/// Provides with file-local constants.
/// </summary>
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
