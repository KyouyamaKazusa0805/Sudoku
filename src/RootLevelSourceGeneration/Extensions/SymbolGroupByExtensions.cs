namespace System.Linq;

/// <summary>
/// Represents extension methods on <see cref="Enumerable.GroupBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
/// with symbol checking.
/// </summary>
/// <seealso cref="Enumerable.GroupBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
public static class SymbolGroupByExtensions
{
	/// <inheritdoc cref="GroupBy{TSource}(IEnumerable{TSource}, Func{TSource, INamedTypeSymbol}, bool)"/>
	public static IEnumerable<IGrouping<INamedTypeSymbol, TSource>> GroupBy<TSource>(
		this ImmutableArray<TSource> source,
		Func<TSource, INamedTypeSymbol> keySelector,
		bool checkNullability = false
	) => GroupBy(source.AsEnumerable(), keySelector, checkNullability);

	/// <inheritdoc cref="Enumerable.GroupBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IEqualityComparer{TKey})"/>
	public static IEnumerable<IGrouping<INamedTypeSymbol, TSource>> GroupBy<TSource>(
		this IEnumerable<TSource> source,
		Func<TSource, INamedTypeSymbol> keySelector,
		bool checkNullability = false
	) => source.GroupBy<TSource, INamedTypeSymbol>(keySelector, SymbolEqualityComparer.Default);
}
