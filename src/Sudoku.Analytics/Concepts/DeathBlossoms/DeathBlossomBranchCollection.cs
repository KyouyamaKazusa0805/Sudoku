namespace Sudoku.Concepts;

/// <summary>
/// Represents a collection that stores a list of branches, grouped by its key specified as type parameter <typeparamref name="TKey"/>.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
/// <typeparam name="TKey">The type of the distinction key.</typeparam>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode, GetHashCodeBehavior = GetHashCodeBehavior.MakeAbstract)]
public abstract partial class DeathBlossomBranchCollection<TSelf, TKey> :
	Dictionary<TKey, AlmostLockedSet>,
	IEquatable<TSelf>,
	ISelectMethod<TSelf, KeyValuePair<TKey, AlmostLockedSet>>
	where TSelf : DeathBlossomBranchCollection<TSelf, TKey>, IEquatable<TSelf>, IEqualityOperators<TSelf, TSelf, bool>, new()
	where TKey : notnull, IAdditiveIdentity<TKey, TKey>, IEquatable<TKey>, IEqualityOperators<TKey, TKey, bool>, new()
{
	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] TSelf? other);

	/// <inheritdoc/>
	IEnumerable<TResult> ISelectMethod<TSelf, KeyValuePair<TKey, AlmostLockedSet>>.Select<TResult>(Func<KeyValuePair<TKey, AlmostLockedSet>, TResult> selector)
		=> this.Select(selector).ToArray();
}
