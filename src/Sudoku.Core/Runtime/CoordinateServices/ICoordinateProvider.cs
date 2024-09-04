namespace Sudoku.Runtime.CoordinateServices;

/// <summary>
/// Represents a type that supports formatting or parsing rules around coordinates.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
public interface ICoordinateProvider<out TSelf> : IFormatProvider where TSelf : ICoordinateProvider<TSelf>, allows ref struct
{
	/// <summary>
	/// Indicates the <typeparamref name="TSelf"/> instance for the invariant culture, meaning it ignores culture your device uses.
	/// </summary>
	public static abstract TSelf InvariantCultureInstance { get; }


	/// <summary>
	/// Try to get a <typeparamref name="TSelf"/> instance from the specified culture.
	/// </summary>
	/// <param name="formatProvider">The format provider instance.</param>
	/// <returns>A <typeparamref name="TSelf"/> instance from the specified culture.</returns>
	public static abstract TSelf GetInstance(IFormatProvider? formatProvider);
}
