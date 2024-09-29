namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Represents a collection that stores a list of branches, grouped by its key specified as type parameter <typeparamref name="TKey"/>.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
/// <typeparam name="TKey">The type of the distinction key.</typeparam>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode, GetHashCodeBehavior = GetHashCodeBehavior.MakeAbstract)]
public abstract partial class DeathBlossomBranchCollection<TSelf, TKey> :
	Dictionary<TKey, AlmostLockedSetPattern>,
	IEquatable<TSelf>,
	ISelectMethod<TSelf, KeyValuePair<TKey, AlmostLockedSetPattern>>
	where TSelf : DeathBlossomBranchCollection<TSelf, TKey>, IEquatable<TSelf>, IEqualityOperators<TSelf, TSelf, bool>, new()
	where TKey : notnull, IAdditiveIdentity<TKey, TKey>, IEquatable<TKey>, IEqualityOperators<TKey, TKey, bool>, new()
{
	/// <summary>
	/// Indicates all digits used.
	/// </summary>
	public Mask DigitsMask
	{
		get
		{
			var result = (Mask)0;
			foreach (var branch in Values)
			{
				result |= branch.DigitsMask;
			}
			return result;
		}
	}


	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] TSelf? other);

	/// <inheritdoc/>
	IEnumerable<TResult> ISelectMethod<TSelf, KeyValuePair<TKey, AlmostLockedSetPattern>>.Select<TResult>(Func<KeyValuePair<TKey, AlmostLockedSetPattern>, TResult> selector)
		=> this.Select(selector).ToArray();
}
