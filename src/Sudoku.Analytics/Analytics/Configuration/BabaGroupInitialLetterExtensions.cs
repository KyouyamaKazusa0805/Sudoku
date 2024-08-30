namespace Sudoku.Analytics.Configuration;

/// <summary>
/// Provides with extension methods on <see cref="BabaGroupInitialLetter"/> instances.
/// </summary>
/// <seealso cref="BabaGroupInitialLetter"/>
public static class BabaGroupInitialLetterExtensions
{
	/// <summary>
	/// Indicates the sequences.
	/// </summary>
	private static readonly Dictionary<(BabaGroupInitialLetter Letter, BabaGroupLetterCasing Casing), string> CharSequences = new()
	{
		{ (BabaGroupInitialLetter.Digit_Zero, BabaGroupLetterCasing.Upper), "012345678" },
		{ (BabaGroupInitialLetter.Digit_Zero, BabaGroupLetterCasing.Lower), "012345678" },
		{ (BabaGroupInitialLetter.Digit_One, BabaGroupLetterCasing.Upper), "123456789" },
		{ (BabaGroupInitialLetter.Digit_One, BabaGroupLetterCasing.Lower), "123456789" },
		{ (BabaGroupInitialLetter.EnglishLetter_A, BabaGroupLetterCasing.Upper), "ABCDEFGHI" },
		{ (BabaGroupInitialLetter.EnglishLetter_A, BabaGroupLetterCasing.Lower), "abcdefghi" },
		{ (BabaGroupInitialLetter.EnglishLetter_X, BabaGroupLetterCasing.Upper), "XYZWVUTSR" },
		{ (BabaGroupInitialLetter.EnglishLetter_X, BabaGroupLetterCasing.Lower), "xyzwvutsr" },
		{ (BabaGroupInitialLetter.GreeceLetter_Alpha, BabaGroupLetterCasing.Upper), "ABΓΔEZHΘI" },
		{ (BabaGroupInitialLetter.GreeceLetter_Alpha, BabaGroupLetterCasing.Lower), "αβγδεζηθι" },
		{ (BabaGroupInitialLetter.ChineseCharacter_One, BabaGroupLetterCasing.Upper), "一二三四五六七八九" },
		{ (BabaGroupInitialLetter.ChineseCharacter_One, BabaGroupLetterCasing.Lower), "一二三四五六七八九" },
		{ (BabaGroupInitialLetter.ChineseHavenlyStem_Jia, BabaGroupLetterCasing.Upper), "甲乙丙丁戊己庚辛壬" },
		{ (BabaGroupInitialLetter.ChineseHavenlyStem_Jia, BabaGroupLetterCasing.Lower), "甲乙丙丁戊己庚辛壬" },
		{ (BabaGroupInitialLetter.CapitalChineseCharacter_One, BabaGroupLetterCasing.Upper), "壹贰叁肆伍陆柒捌玖" },
		{ (BabaGroupInitialLetter.CapitalChineseCharacter_One, BabaGroupLetterCasing.Lower), "壹贰叁肆伍陆柒捌玖" },
		{ (BabaGroupInitialLetter.Kanji_One, BabaGroupLetterCasing.Upper), "壱弐参肆伍陸漆捌玖" },
		{ (BabaGroupInitialLetter.Kanji_One, BabaGroupLetterCasing.Lower), "壱弐参肆伍陸漆捌玖" }
	};


	/// <summary>
	/// Try to get character sequence from the specified initial letter.
	/// </summary>
	/// <param name="this">The initial letter.</param>
	/// <param name="casing">The letter casing.</param>
	/// <returns>The character sequence.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the specified arguments are not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlyCharSequence GetSequence(this BabaGroupInitialLetter @this, BabaGroupLetterCasing casing)
		=> Enum.IsDefined(@this) && Enum.IsDefined(casing)
			? CharSequences[(@this, casing)].AsSpan()
			: throw new ArgumentOutOfRangeException(nameof(@this));
}
