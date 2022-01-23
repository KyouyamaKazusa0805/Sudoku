namespace Sudoku.Presentation;

/// <summary>
/// Encapsulates an unknown value that used in the technique <b>Unknown Covering</b>.
/// </summary>
/// <param name="Cell">Indicates the cell that used and marked.</param>
/// <param name="UnknownIdentifier">Indicates the identifier that identifies the value range.</param>
/// <param name="DigitsMask">Indicates a mask that holds a serial of candidate values.</param>
public readonly record struct UnknownValue(int Cell, char UnknownIdentifier, short DigitsMask)
: IDefaultable<UnknownValue>
, IEqualityOperators<UnknownValue, UnknownValue>
{
	/// <summary>
	/// <inheritdoc cref="IDefaultable{TStruct}.Default"/>
	/// </summary>
	public static readonly UnknownValue Default = new(-1, '\0', -1);


	/// <inheritdoc/>
	public bool IsDefault => this == Default;

	/// <inheritdoc/>
	static UnknownValue IDefaultable<UnknownValue>.Default => Default;
}
