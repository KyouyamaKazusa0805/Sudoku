namespace Sudoku.Resources;

/// <summary>
/// Represents the naming of techniques.
/// </summary>
public static class TechniqueNaming
{
	/// <summary>
	/// Indicates the digit characters.
	/// </summary>
	private static readonly string ChineseDigitCharacters = "\u4e00\u4e8c\u4e09\u56db\u4e94\u516d\u4e03\u516b\u4e5d\u5341";


	/// <summary>
	/// Returns a character that represents the digit.
	/// </summary>
	/// <param name="culture">The culture.</param>
	/// <param name="digit">The digit value.</param>
	/// <returns>The character that represents the specified digit.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static char GetDigitCharacter(CultureInfo culture, Digit digit)
		=> SR.IsChinese(culture) ? ChineseDigitCharacters[digit] : (char)(digit + '1');

	/// <summary>
	/// Try to get the real name for the specified size of subset.
	/// </summary>
	/// <param name="size">The number of cells used in a subset.</param>
	/// <returns>The name of the subset.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetSubsetName(Digit size) => SR.Get($"SubsetNamesSize{size}", CultureInfo.CurrentUICulture);

	/// <summary>
	/// Make the real name of the regular wing.
	/// </summary>
	/// <param name="size">Indicates the size of the wing.</param>
	/// <param name="isIncomplete">A <see cref="bool"/> value indicating whether the wing is incomplete.</param>
	/// <returns>The real name of the regular wing.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="size"/> isn't between 3 and 9.</exception>
	internal static string GetRegularWingEnglishName(Digit size, bool isIncomplete)
		=> size switch
		{
			3 => isIncomplete ? "XY-Wing" : "XYZ-Wing",
			>= 4 and <= 9 when size switch
			{
				4 => "WXYZ-Wing",
				5 => "VWXYZ-Wing",
				6 => "UVWXYZ-Wing",
				7 => "TUVWXYZ-Wing",
				8 => "STUVWXYZ-Wing",
				9 => "RSTUVWXYZ-Wing"
			} is var name => isIncomplete ? $"Incomplete {name}" : name,
			_ => throw new ArgumentOutOfRangeException(nameof(size))
		};

	/// <summary>
	/// Gets the name of the fish via the specified size.
	/// </summary>
	/// <param name="size">The size.</param>
	/// <returns>The fish name.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="size"/> is 0.</exception>
	internal static string GetFishEnglishName(Digit size)
		=> size switch
		{
			0 => throw new ArgumentOutOfRangeException(nameof(size)),
			1 => "Cyclopsfish",
			2 => "X-Wing",
			3 => "Swordfish",
			4 => "Jellyfish",
			5 => "Squirmbag",
			6 => "Whale",
			7 => "Leviathan",
			_ => $"{size}-Fish"
		};

	/// <summary>
	/// Try to fetch corresponding <see cref="Technique"/> instance via the real name representing as a <see cref="string"/> text.
	/// </summary>
	/// <param name="englishName">The real name of the regular wing technique.</param>
	/// <returns>The <see cref="Technique"/> instance.</returns>
	/// <exception cref="InvalidOperationException">Throws when the argument <paramref name="englishName"/> is invalid.</exception>
	internal static Technique MakeRegularWingTechniqueCode(string englishName)
		=> englishName switch
		{
			"XY-Wing" => Technique.XyWing,
			"XYZ-Wing" => Technique.XyzWing,
			"WXYZ-Wing" => Technique.WxyzWing,
			"VWXYZ-Wing" => Technique.VwxyzWing,
			"UVWXYZ-Wing" => Technique.UvwxyzWing,
			"TUVWXYZ-Wing" => Technique.TuvwxyzWing,
			"STUVWXYZ-Wing" => Technique.StuvwxyzWing,
			"RSTUVWXYZ-Wing" => Technique.RstuvwxyzWing,
			"Incomplete WXYZ-Wing" => Technique.IncompleteWxyzWing,
			"Incomplete VWXYZ-Wing" => Technique.IncompleteVwxyzWing,
			"Incomplete UVWXYZ-Wing" => Technique.IncompleteUvwxyzWing,
			"Incomplete TUVWXYZ-Wing" => Technique.IncompleteTuvwxyzWing,
			"Incomplete STUVWXYZ-Wing" => Technique.IncompleteStuvwxyzWing,
			"Incomplete RSTUVWXYZ-Wing" => Technique.IncompleteRstuvwxyzWing,
			_ => throw new InvalidOperationException($"The argument {nameof(englishName)} must be valid.")
		};
}
