namespace Sudoku.Data;

/// <summary>
/// Defines a basic constraint that applied onto a <see cref="ChainLinkType"/>.
/// </summary>
/// <typeparam name="TSelf">The type. The type is always <see cref="ChainLinkType"/>.</typeparam>
internal interface IChainLinkType<TSelf> where TSelf : struct, IChainLinkType<TSelf>
{
	/// <summary>
	/// The type kind.
	/// </summary>
	byte TypeKind { get; }


	/// <summary>
	/// Gets the notation of the chain link that combines 2 <see cref="ChainNode"/>s.
	/// </summary>
	/// <returns>The notation <see cref="string"/> representation.</returns>
	string GetNotation();

	/// <inheritdoc cref="object.ToString"/>
	string ToString();

	/// <inheritdoc cref="object.GetHashCode"/>
	int GetHashCode();


	/// <summary>
	/// Explicit cast from <see cref="byte"/> to <typeparamref name="TSelf"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	static abstract explicit operator TSelf(byte value);

	/// <summary>
	/// Explicit cast from <typeparamref name="TSelf"/> to <see cref="byte"/>.
	/// </summary>
	/// <param name="linkType">The link type.</param>
	static abstract explicit operator byte(TSelf linkType);
}