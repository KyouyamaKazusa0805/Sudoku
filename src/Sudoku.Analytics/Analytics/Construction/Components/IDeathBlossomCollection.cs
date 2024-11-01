namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents the basic data for a death blossom.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
/// <typeparam name="TKey">The type of the distinction key.</typeparam>
public interface IDeathBlossomCollection<TSelf, TKey> : IComponent
	where TSelf : DeathBlossomBranchCollection<TSelf, TKey>, IEquatable<TSelf>, IEqualityOperators<TSelf, TSelf, bool>, new()
	where TKey : notnull, IAdditiveIdentity<TKey, TKey>, IEquatable<TKey>, IEqualityOperators<TKey, TKey, bool>, new()
{
	/// <summary>
	/// Indicates the branches used.
	/// </summary>
	public abstract TSelf Branches { get; }

	/// <inheritdoc/>
	ComponentType IComponent.Type => ComponentType.DeathBlossomBranch;
}
