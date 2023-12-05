using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.SourceGeneration;

namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Represents a collection that stores a list of branches, grouped by its key specified as type parameter <typeparamref name="TKey"/>.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
/// <typeparam name="TKey">The type of the distinction key.</typeparam>
[Equals]
public abstract partial class DeathBlossomBranchCollection<TSelf, TKey> : Dictionary<TKey, AlmostLockedSet>, IEquatable<TSelf>
	where TSelf : DeathBlossomBranchCollection<TSelf, TKey>, IEquatable<TSelf>, IEqualityOperators<TSelf, TSelf, bool>, new()
	where TKey : notnull, IAdditiveIdentity<TKey, TKey>, IEquatable<TKey>, IEqualityOperators<TKey, TKey, bool>, new()
{
	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] TSelf? other);

	/// <inheritdoc/>
	public abstract override int GetHashCode();

	/// <summary>
	/// Transforms the current collection into another representation, using the specified function to transform.
	/// </summary>
	/// <typeparam name="TResult">The type of the results.</typeparam>
	/// <param name="selector">The selector to tranform elements.</param>
	/// <returns>The results.</returns>
	public ReadOnlySpan<TResult> Select<TResult>(Func<(TKey Key, AlmostLockedSet AlsPattern), TResult> selector)
	{
		var (result, i) = (new TResult[Count], 0);
		foreach (var (key, value) in this)
		{
			result[i++] = selector((key, value));
		}

		return result;
	}
}
