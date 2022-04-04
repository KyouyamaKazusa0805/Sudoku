namespace Sudoku.Concepts.Notations;

/// <summary>
/// Provides with a type that is used for displaying a cell list, using hodoku elimination notation.
/// </summary>
/// <param name="DigitFirst">
/// <para>Indicates whether the output string will put the digit at first.</para>
/// <para>The default value is <see langword="true"/>.</para>
/// </param>
/// <param name="Separator">
/// <para>Indicates the separator that is used for the insertion between 2 adjacent candidate elements.</para>
/// <para>The default value is <c>" "</c>.</para>
/// </param>
public readonly record struct EliminationNotationOptions(bool DigitFirst = true, string Separator = " ") :
	IDefaultable<EliminationNotationOptions>,
	INotationHandlerOptions<EliminationNotationOptions>
{
	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	public static readonly EliminationNotationOptions Default = new(true, " ");


	/// <inheritdoc/>
	bool IDefaultable<EliminationNotationOptions>.IsDefault => this == Default;

	/// <inheritdoc/>
	static EliminationNotationOptions IDefaultable<EliminationNotationOptions>.Default => Default;
}
