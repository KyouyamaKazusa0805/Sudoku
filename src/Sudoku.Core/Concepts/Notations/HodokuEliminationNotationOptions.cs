namespace Sudoku.Concepts.Notations;

/// <summary>
/// Provides with a type that is used for displaying a cell list, using hodoku elimination notation.
/// </summary>
/// <param name="DigitFirst">Indicates whether the output string will put the digit at first.</param>
public readonly record struct HodokuEliminationNotationOptions(bool DigitFirst = true) :
	IDefaultable<HodokuEliminationNotationOptions>,
	INotationHandlerOptions<HodokuEliminationNotationOptions>
{
	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	public static readonly HodokuEliminationNotationOptions Default = new(true);


	/// <inheritdoc/>
	bool IDefaultable<HodokuEliminationNotationOptions>.IsDefault => this == Default;

	/// <inheritdoc/>
	static HodokuEliminationNotationOptions IDefaultable<HodokuEliminationNotationOptions>.Default => Default;
}
