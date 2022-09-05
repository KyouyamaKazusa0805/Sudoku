namespace System;

/// <summary>
/// Represents a type that supports logical not operators.
/// </summary>
/// <typeparam name="TSelf">The type of the value.</typeparam>
/// <typeparam name="TResult">The result of the negation.</typeparam>
public interface ILogicalNotOperators<in TSelf, out TResult> where TSelf : ILogicalNotOperators<TSelf, TResult>
{
	/// <summary>
	/// Negates an instance of type <typeparamref name="TSelf"/>
	/// to get the result of type <typeparamref name="TResult"/>.
	/// </summary>
	/// <param name="value">The value of type <typeparamref name="TSelf"/>.</param>
	/// <returns>The result after negation, of type <typeparamref name="TResult"/>.</returns>
	public static abstract TResult operator !(TSelf value);
}
