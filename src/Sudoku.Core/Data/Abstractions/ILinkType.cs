namespace Sudoku.Data.Abstractions;

/// <summary>
/// Defines a basic constraint that applied onto a <see cref="LinkType"/>.
/// </summary>
/// <typeparam name="T">The type. The type is always <see cref="LinkType"/>.</typeparam>
internal interface ILinkType<[Self] T> where T : struct, ILinkType<T>
{
	/// <summary>
	/// The type kind.
	/// </summary>
	byte TypeKind { get; }


	/// <summary>
	/// Gets the notation of the chain link that combines 2 <see cref="Node"/>s.
	/// </summary>
	/// <returns>The notation <see cref="string"/> representation.</returns>
	string GetNotation();

	/// <inheritdoc cref="object.ToString"/>
	string ToString();

	/// <inheritdoc cref="object.GetHashCode"/>
	int GetHashCode();


	/// <summary>
	/// Explicit cast from <see cref="byte"/> to <typeparamref name="T"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	static abstract explicit operator T(byte value);

	/// <summary>
	/// Explicit cast from <typeparamref name="T"/> to <see cref="byte"/>.
	/// </summary>
	/// <param name="linkType">The link type.</param>
	static abstract explicit operator byte(T linkType);
}