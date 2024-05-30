namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Represents a collection that stores a list of branches, grouped by its key specified as type parameter <typeparamref name="TKey"/>.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
/// <typeparam name="TKey">The type of the distinction key.</typeparam>
[GetHashCode(GetHashCodeBehavior.MakeAbstract)]
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode)]
public abstract partial class DeathBlossomBranchCollection<TSelf, TKey> : Dictionary<TKey, AlmostLockedSet>, IEquatable<TSelf>
	where TSelf : DeathBlossomBranchCollection<TSelf, TKey>, IEquatable<TSelf>, IEqualityOperators<TSelf, TSelf, bool>, new()
	where TKey : notnull, IAdditiveIdentity<TKey, TKey>, IEquatable<TKey>, IEqualityOperators<TKey, TKey, bool>, new()
{
	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] TSelf? other);
}
