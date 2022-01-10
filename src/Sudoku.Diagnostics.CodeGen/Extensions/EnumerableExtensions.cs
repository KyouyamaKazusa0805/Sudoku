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
	{
		var type = @this.GetType();
		if (type == typeof(T[]))
		{
			Array.ForEach((T[])@this, action);
			return;
		}

		if (type == typeof(List<T>))
		{
			((List<T>)@this).ForEach(action);
			return;
		}

		foreach (var element in @this)
		{
			action(element);
		}
	}
}
