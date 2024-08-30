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
	/// Indicates the initial letter is English letter '<c>A (a)</c>'. The sequence would be <c>A, B, C, ...</c>.
	/// </summary>
	EnglishLetter_A,

	/// <summary>
	/// Indicates the initial letter is English letter '<c>X (x)</c>'. The sequence would be <c>X, Y, Z, W, ...</c>.
	/// </summary>
	EnglishLetter_X,

	/// <summary>
	/// Indicates the initial letter is Greece letter '<c>A (α)</c>'. The sequence would be <c>A, B, Γ...</c>.
	/// </summary>
	GreeceLetter_Alpha,

	/// <summary>
	/// Indicates the initial letter is Chinese character '<c>一</c>' (pinyin: yī, meaning "one").
	/// The sequence would be <c>一, 二, 三...</c>.
	/// </summary>
	ChineseCharacter_One,

	/// <summary>
	/// Indicates the initial letter is Chinese Havenly stem '<c>甲</c>' (pinyin: jiǎ, meaning "one").
	/// The sequence would be <c>甲, 乙, 丙...</c>.
	/// </summary>
	ChineseHavenlyStem_Jia,

	/// <summary>
	/// Indicates the initial letter is Chinese character '<c>壹</c>' (pinyin: yī, meaning "one").
	/// The sequence would be <c>壹, 贰, 叁...</c>.
	/// </summary>
	CapitalChineseCharacter_One,

	/// <summary>
	/// Indicates the initial letter is Japanese character (i.e. Kanji) '<c>壱</c>' (Roman: Ichi, meaning "one").
	/// The sequence would be <c>壱, 弐, 参...</c>.
	/// </summary>
	Kanji_One
}
