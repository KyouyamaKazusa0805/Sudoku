namespace System.Collections.Generic;

/// <summary>
/// Represents an object that is a set of elements of type <typeparamref name="T"/>, and is a closed set,
/// meaning the set contains concept "all", i.e. set can be negated.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
/// <typeparam name="T"><inheritdoc cref="ISet{T}" path="/typeparam[@name='T']"/></typeparam>
public interface IFiniteSet<TSelf, T> : ISet<T> where TSelf : IFiniteSet<TSelf, T>, allows ref struct
{
	/// <summary>
	/// Returns an object that inverts elements including the current collection.
	/// </summary>
	/// <returns>An instance as the result.</returns>
	public abstract TSelf Negate();
}
