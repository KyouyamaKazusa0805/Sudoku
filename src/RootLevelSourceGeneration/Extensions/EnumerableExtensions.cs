namespace System.Collections.Generic;

/// <summary>
/// Provides extension methods on <see cref="IEnumerable{T}"/>.
/// </summary>
/// <seealso cref="IEnumerable{T}"/>
internal static class EnumerableExtensions
{
	/// <summary>
	/// The method to replace with <see langword="foreach"/> loop.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The collection to iterate.</param>
	/// <param name="action">The action that is executed while iterating.</param>
	public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
		=> (
			@this switch
			{
				T[] array => action => Array.ForEach(array, action),
				List<T> list => new Action<Action<T>>(list.ForEach),
				_ => action => { foreach (var element in @this) { action(element); } }
			}
		)(action);
}
