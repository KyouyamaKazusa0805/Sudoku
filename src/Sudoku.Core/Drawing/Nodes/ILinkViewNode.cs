namespace Sudoku.Drawing.Nodes;

/// <summary>
/// Represents a view node kind that is represented as a link.
/// This type is the base type of <see cref="ChainLinkViewNode"/> and <see cref="CellLinkViewNode"/>.
/// </summary>
/// <seealso cref="ChainLinkViewNode"/>
/// <seealso cref="CellLinkViewNode"/>
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
	public abstract Type ElementType { get; }


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	public sealed void Deconstruct(out ColorIdentifier identifier, out object start, out object end)
		=> (identifier, start, end) = (Identifier, Start, End);
}
