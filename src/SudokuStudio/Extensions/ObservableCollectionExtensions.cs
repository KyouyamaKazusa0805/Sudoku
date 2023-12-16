namespace System.Collections.ObjectModel;

/// <summary>
/// Provides with extension methods on <see cref="ObservableCollection{T}"/>.
/// </summary>
/// <seealso cref="ObservableCollection{T}"/>
public static class ObservableCollectionExtensions
{
	/// <summary>
	/// Removes the specified element if the specified condition returns <see langword="true"/>.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The collection.</param>
	/// <param name="predicate">The condition that checks whether the element should be removed.</param>
	public static void RemoveWhen<T>(this ObservableCollection<T> @this, Func<T, bool> predicate)
	{
		for (var i = 0; i < @this.Count; i++)
		{
			if (predicate(@this[i]))
			{
				@this.RemoveAt(i);
				return;
			}
		}
	}
}
