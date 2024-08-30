namespace Sudoku.Analytics.Configuration;

/// <summary>
/// Provides a style that displays for a letter in a baba group.
/// </summary>
public enum BabaGroupInitialLetter : byte
{
	/// <summary>
	/// Indicates the initial letter is digit '<c>0</c>'. The sequence would be <c>0, 1, 2 ...</c>.
	/// </summary>
	Digit_Zero,

	/// <summary>
	/// Indicates the initial letter is digit '<c>1</c>'. The sequence would be <c>1, 2, 3 ...</c>.
	/// </summary>
	Digit_One,

	/// <summary>
	/// Indicates the initial letter is romatic number '<c>&#8544;</c>'.
	/// The sequence would be <c>&#8544;, &#8545;, &#8546; ...</c>.
	/// </summary>
	RomanticNumber_One,

	/// <summary>
	/// Indicates the initial letter is English letter '<c>A</c>'. The sequence would be <c>A, B, C, ...</c>.
	/// </summary>
	EnglishLetter_A,

	/// <summary>
	/// Indicates the initial letter is English letter '<c>X</c>'. The sequence would be <c>X, Y, Z, W, ...</c>.
	/// </summary>
	EnglishLetter_X,

	/// <summary>
	/// Indicates the initial letter is Greece letter '<c>&#913;</c>'. The sequence would be <c>&#913;, &#914;, &#915; ...</c>.
	/// </summary>
	GreeceLetter_Alpha,

	/// <summary>
	/// Indicates the initial letter is Chinese character '<c>&#19968;</c>' (pinyin: yī, meaning "one").
	/// The sequence would be <c>&#19968;, &#20108;, &#19977; ...</c>.
	/// </summary>
	ChineseCharacter_One,

	/// <summary>
	/// Indicates the initial letter is Chinese Havenly stem '<c>&#30002;</c>' (pinyin: jiǎ, meaning "one").
	/// The sequence would be <c>&#30002;, &#20057;, &#19993; ...</c>.
	/// </summary>
	ChineseHavenlyStem_Jia,

	/// <summary>
	/// Indicates the initial letter is Chinese character '<c>&#22777;</c>' (pinyin: yī, meaning "one").
	/// The sequence would be <c>&#22777;, &#36144;, &#21441; ...</c>.
	/// </summary>
	CapitalChineseCharacter_One,

	/// <summary>
	/// Indicates the initial letter is Japanese character (i.e. Kanji) '<c>&#22769;</c>' (Roman: Ichi, meaning "one").
	/// The sequence would be <c>&#22769;, &#24336;, &#21442; ...</c>.
	/// </summary>
	Kanji_One
}
