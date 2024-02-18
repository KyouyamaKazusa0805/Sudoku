namespace Sudoku.Concepts;

/// <summary>
/// Represents an object that can create a token of type <see cref="string"/> to describe the encrpyted representation.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
public interface ITokenizable<TSelf> : IEqualityComparer<TSelf> where TSelf : ITokenizable<TSelf>, IEqualityComparer<TSelf>
{
	/// <summary>
	/// Indicates the token of the object.
	/// </summary>
	public abstract string Token { get; }

	/// <summary>
	/// Indicates the hash code that is calculated with token.
	/// </summary>
	protected internal sealed int TokenHashCode => Token.GetHashCode();


	/// <inheritdoc/>
	bool IEqualityComparer<TSelf>.Equals(TSelf? x, TSelf? y) => x?.Token == y?.Token;

	/// <inheritdoc/>
	int IEqualityComparer<TSelf>.GetHashCode([DisallowNull] TSelf obj) => obj.TokenHashCode;


	/// <inheritdoc cref="IEqualityComparer{T}.Equals(T, T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static new virtual bool Equals(TSelf left, TSelf right) => left.Token == right.Token;

	/// <inheritdoc cref="IEqualityComparer{T}.GetHashCode(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static new virtual int GetHashCode(TSelf obj) => obj.TokenHashCode;

	/// <summary>
	/// Create an instance of type <typeparamref name="TSelf"/> that is used a token.
	/// </summary>
	/// <param name="token">The token to be used.</param>
	/// <returns>A <typeparamref name="TSelf"/> instance created.</returns>
	/// <exception cref="FormatException">Throws when the token is invalid.</exception>
	public static abstract TSelf Create(string token);
}
