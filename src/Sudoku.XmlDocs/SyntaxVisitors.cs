#pragma warning disable IDE1006

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.XmlDocs
{
	/// <summary>
	/// Indicates the delegated methods that will be invoked while the specified node type is visiting.
	/// </summary>
	/// <param name="node">The doc comment node.</param>
	/// <param name="descendants">All descendants.</param>
	public delegate void SyntaxVisitor(XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants);

	/// <summary>
	/// Indicates the delegated methods that will be invoked while the specified node type is visiting.
	/// Different with <see cref="SyntaxVisitor"/>, this type contains a new parameter
	/// named <paramref name="attributes"/>, which belongs to the markup.
	/// </summary>
	/// <param name="node">The doc comment node.</param>
	/// <param name="attributes">The attributes of the node.</param>
	/// <param name="descendants">All descendants.</param>
	/// <seealso cref="SyntaxVisitor"/>
	public delegate void AttributedSyntaxVisitor(XmlElementSyntax node, in SyntaxList<XmlAttributeSyntax> attributes, in SyntaxList<XmlNodeSyntax> descendants);

	/// <summary>
	/// Indicates the delegated methods that will be invoked while the specified node type is visiting.
	/// Different with <see cref="AttributedSyntaxVisitor"/>, this type doesn't contain any descedants.
	/// </summary>
	/// <param name="node">The doc comment node.</param>
	/// <param name="attributes">The attributes of the node.</param>
	/// <seealso cref="AttributedSyntaxVisitor"/>
	public delegate void AttributedSyntaxVisitorWithoutDescendants(XmlEmptyElementSyntax node, in SyntaxList<XmlAttributeSyntax> attributes);
}
