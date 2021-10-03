namespace Sudoku.Data;

/// <summary>
/// Indicates the value is a constant of type <see cref="ChainLinkType"/>.
/// </summary>
public sealed class ChainLinkTypeConstantAttribute : CustomConstantAttribute
{
	/// <summary>
	/// Initializes a <see cref="ChainLinkTypeConstantAttribute"/> instance
	/// via a specified <see cref="byte"/> value as the type kind of a chain link.
	/// </summary>
	/// <param name="typeKind">The type kind.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ChainLinkTypeConstantAttribute(byte typeKind) : base() =>
		Value = ValueWithoutBoxOperation = new ChainLinkType(typeKind);


	/// <summary>
	/// Indicates the real value stored. The property is same as the overriden property <see cref="Value"/>
	/// but without any box and unbox operations.
	/// </summary>
	/// <seealso cref="Value"/>
	public ChainLinkType ValueWithoutBoxOperation { get; }

	/// <inheritdoc/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override object Value { get; }
}
