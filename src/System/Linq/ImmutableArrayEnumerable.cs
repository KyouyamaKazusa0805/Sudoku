namespace System.Linq;

/// <summary>
/// Provides with LINQ methods on <see cref="ImmutableArray{T}"/>.
/// </summary>
public static class ImmutableArrayEnumerable
{
	/// <inheritdoc cref="Enumerable.Zip{TFirst, TSecond}(IEnumerable{TFirst}, IEnumerable{TSecond})"/>
	public static ImmutableArray<(T1 First, T2 Second)> Zip<T1, T2>(this ImmutableArray<T1> first, ImmutableArray<T2> second)
	{
		if (first.Length != second.Length)
		{
			throw new ArgumentException("Two arrays must hold a same length.");
		}

		if (first.Length == 0)
		{
			return ImmutableArray.Create<(T1, T2)>();
		}

		var result = new (T1, T2)[first.Length];
		for (int i = 0; i < first.Length; i++)
		{
			result[i] = (first[i], second[i]);
		}

		return ImmutableArray.Create(result);
	}
}
