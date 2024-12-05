namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a node that supports parent linking.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
internal interface IParentLinkedNode<TSelf> :
	IComponent,
	IEquatable<TSelf>,
	IFormattable,
	IEqualityOperators<TSelf, TSelf, bool>,
	IShiftOperators<TSelf, TSelf, TSelf>
	where TSelf : IParentLinkedNode<TSelf>
{
	/// <summary>
	/// Indicates the length of ancestors.
	/// </summary>
	public sealed int AncestorsLength
	{
		get
		{
			var result = 0;
			for (var node = this; node is not null; node = node.Parent)
			{
				result++;
			}
			return result;
		}
	}

	/// <summary>
	/// Indicates all ancestor nodes of the current node.
	/// </summary>
	public sealed ReadOnlySpan<TSelf> Ancestors
	{
		get
		{
			var (result, p) = (new List<TSelf> { (TSelf)this }, Parent);
			while (p is not null)
			{
				result.Add(p);
				p = p.Parent;
			}
			return result.AsSpan();
		}
	}

	/// <summary>
	/// Indicates the parent node.
	/// </summary>
	public abstract TSelf? Parent { get; }

	/// <summary>
	/// Indicates the root node.
	/// </summary>
	public abstract TSelf Root { get; }

	/// <inheritdoc/>
	DataStructureType IDataStructure.Type => DataStructureType.None;

	/// <inheritdoc/>
	DataStructureBase IDataStructure.Base => DataStructureBase.LinkedListBased;


	/// <summary>
	/// Determines whether the current node is an ancestor of the specified node. 
	/// </summary>
	/// <param name="childNode">The node to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public virtual bool IsAncestorOf(TSelf childNode)
	{
		for (var node = childNode; node is not null; node = node.Parent)
		{
			if (Equals(node))
			{
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public abstract string ToString(IFormatProvider? formatProvider);

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);


	/// <summary>
	/// Creates a <see cref="WhipNode"/> instance with parent node.
	/// </summary>
	/// <param name="current">The current node.</param>
	/// <param name="parent">The parent node.</param>
	/// <returns>The new node created.</returns>
	public static abstract TSelf operator >>(TSelf current, TSelf? parent);

	/// <inheritdoc cref="IShiftOperators{TSelf, TOther, TResult}.op_LeftShift(TSelf, TOther)"/>
	static TSelf IShiftOperators<TSelf, TSelf, TSelf>.operator <<(TSelf? parent, TSelf current) => current >> parent;

	/// <inheritdoc/>
	static TSelf IShiftOperators<TSelf, TSelf, TSelf>.operator >>>(TSelf value, TSelf shiftAmount) => value >> shiftAmount;
}
