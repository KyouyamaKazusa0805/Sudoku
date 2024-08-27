namespace System.Collections.Generic;

/// <summary>
/// Represents an object that is a set of elements of type <typeparamref name="T"/>, and isn't a closed set,
/// meaning the set doesn't contain concept "all", i.e. set cannot be negated.
/// </summary>
/// <typeparam name="TSelf">The type itself.</typeparam>
/// <typeparam name="T"><inheritdoc cref="ISet{T}" path="/typeparam[@name='T']"/></typeparam>
public interface IInfiniteSet<TSelf, T> : ISet<T> where TSelf : IInfiniteSet<TSelf, T>, allows ref struct
{
	/// <summary>
	/// Makes an subtraction from two objects of type <typeparamref name="TSelf"/>,
	/// to get elements that are in the current collection, but not in the collection <paramref name="other"/>.
	/// </summary>
	/// <param name="other">The collection to make subraction.</param>
	/// <returns>An instance as the result.</returns>
	public abstract TSelf ExceptWith(TSelf other);
}
