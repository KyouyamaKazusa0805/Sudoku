namespace Sudoku.Data;

/// <summary>
/// Encapsulates an unknown value that used in the technique <b>Unknown Covering</b>.
/// </summary>
/// <param name="Cell">Indicates the cell that used and marked.</param>
/// <param name="UnknownIdentifier">Indicates the identifier that identifies the value range.</param>
/// <param name="DigitsMask">Indicates a mask that holds a serial of candidate values.</param>
public readonly partial record struct UnknownValue(int Cell, char UnknownIdentifier, short DigitsMask) : IValueEquatable<UnknownValue>, IJsonSerializable<UnknownValue, UnknownValue.JsonConverter>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(in UnknownValue other) =>
		Cell == other.Cell && UnknownIdentifier == other.UnknownIdentifier && DigitsMask == other.DigitsMask;


	/// <inheritdoc/>
	public static bool operator ==(in UnknownValue left, in UnknownValue right) => left.Equals(in right);

	/// <inheritdoc/>
	public static bool operator !=(in UnknownValue left, in UnknownValue right) => !left.Equals(in right);
}
