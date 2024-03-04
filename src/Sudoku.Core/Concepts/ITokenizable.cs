namespace Sudoku.Concepts;

/// <summary>
/// Represents an object that can create a token of type <see cref="string"/> to describe the encrpyted representation.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
public interface ITokenizable<TSelf> where TSelf : ITokenizable<TSelf>
{
	/// <summary>
	/// Indicates the token of the object.
	/// </summary>
	public abstract string Token { get; }

	/// <summary>
	/// Indicates the equality comparer instance of type <typeparamref name="TSelf"/>.
	/// </summary>
	public sealed IEqualityComparer<TSelf> EqualityComparer
		=> ValueComparison.Create<TSelf>(static (x, y) => x?.Token == y?.Token, static v => v.TokenHashCode);

	/// <summary>
	/// Indicates the hash code that is calculated with token.
	/// </summary>
	protected internal sealed int TokenHashCode => Token.GetHashCode();


	/// <summary>
	/// Create an instance of type <typeparamref name="TSelf"/> that is used a token.
	/// </summary>
	/// <param name="token">The token to be used.</param>
	/// <returns>A <typeparamref name="TSelf"/> instance created.</returns>
	/// <exception cref="FormatException">Throws when the token is invalid.</exception>
	public static abstract TSelf Create(string token);
}
