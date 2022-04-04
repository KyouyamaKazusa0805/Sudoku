namespace Sudoku.Concepts.Notations;

/// <summary>
/// Provides with a type that is used for displaying a cell list, using K9 notation.
/// </summary>
/// <param name="UpperCasing">
/// Indicates whether the method should use upper-casing to handle the result notation of cells.
/// For example, if <see langword="true"/>, the concept "row 3 column 3" will be displayed
/// as <c>C3</c>; otherwise, <c>c3</c>.
/// </param>
/// <param name="AvoidConfusionOnRowLetters">
/// Indicates whether the method should avoid confusion for the letter I and digit 1. For example,
/// if <see langword="true"/>, row 9 column 9 will be notated as <c>K9</c>; otherwise, <c>I9</c>.
/// </param>
/// <param name="Separator">
/// Indicates the separator string value that inserts two coordinate elements, to combine them.
/// For example, cells <c>C1</c> and <c>D2</c> can be combined to <c>C1|D2</c> if the seperator
/// is <c>"|"</c>.
/// </param>
public readonly record struct K9NotationOptions(
	bool UpperCasing = false, bool AvoidConfusionOnRowLetters = false, string Separator = "|") :
	IDefaultable<K9NotationOptions>,
	INotationHandlerOptions<K9NotationOptions>
{
	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	public static readonly K9NotationOptions Default = new(false, false, "|");


	/// <inheritdoc/>
	bool IDefaultable<K9NotationOptions>.IsDefault => this == Default;

	/// <inheritdoc/>
	static K9NotationOptions IDefaultable<K9NotationOptions>.Default => Default;
}
