namespace System.Linq;

/// <summary>
/// Represents with LINQ methods for <see cref="List{T}"/>.
/// </summary>
/// <seealso cref="List{T}"/>
public static class ListEnumerable
{
	/// <inheritdoc cref="ArrayEnumerable.Count{T}(T[], Func{T, bool})"/>
	public static int Count<T>(this List<T> @this, Func<T, bool> predicate)
	{
		var result = 0;
		foreach (var element in @this)
		{
			if (predicate(element))
			{
				result++;
			}
		}

		return result;
	}
}
