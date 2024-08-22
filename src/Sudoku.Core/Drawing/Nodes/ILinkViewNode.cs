namespace Sudoku.Drawing.Nodes;

/// <summary>
/// Represents a view node kind that is represented as a link.
/// This type is the base type
/// of types <see cref="ChainLinkViewNode"/>, <see cref="CellLinkViewNode"/> and <see cref="ConjugateLinkViewNode"/>.
/// </summary>
/// <seealso cref="ChainLinkViewNode"/>
/// <seealso cref="CellLinkViewNode"/>
/// <seealso cref="ConjugateLinkViewNode"/>
public interface ILinkViewNode
{
	/// <summary>
	/// Indicates the start element.
	/// </summary>
	public abstract object Start { get; }

	/// <summary>
	/// Indicates the end element.
	/// </summary>
	public abstract object End { get; }

	/// <summary>
	/// Indicates the color identifier.
	/// </summary>
	public abstract ColorIdentifier Identifier { get; }

	/// <summary>
	/// Indicates the element type.
	/// </summary>
	/// <remarks>
	/// The result value may not match property type of <see cref="Start"/> and <see cref="End"/>.
	/// This property is only used as distinct link types.
	/// </remarks>
	/// <seealso cref="Start"/>
	/// <seealso cref="End"/>
	public abstract Type ElementType { get; }


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	public sealed void Deconstruct(out ColorIdentifier identifier, out object start, out object end)
		=> (identifier, start, end) = (Identifier, Start, End);
}
