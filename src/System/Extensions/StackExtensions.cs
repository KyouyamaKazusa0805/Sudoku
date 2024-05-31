namespace System.Collections.Generic;

/// <summary>
/// Provides with extension methods on <see cref="Stack{T}"/> instances.
/// </summary>
/// <seealso cref="Stack{T}"/>
public static class StackExtensions
{
	/// <summary>
	/// <inheritdoc cref="Enumerable.Reverse{TSource}(IEnumerable{TSource})" path="/summary"/>
	/// </summary>
	/// <param name="this"><inheritdoc cref="Enumerable.Reverse{TSource}(IEnumerable{TSource})" path="/param[@name='source']"/></param>
	/// <returns><inheritdoc cref="Enumerable.Reverse{TSource}(IEnumerable{TSource})" path="/returns"/></returns>
	public static Stack<T> Reverse<T>(this Stack<T> @this)
	{
		var result = new Stack<T>(@this.Count);
		foreach (var element in @this)
		{
			result.Push(element);
		}
		return result;
	}
}
