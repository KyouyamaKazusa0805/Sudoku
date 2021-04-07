#pragma warning disable IDE1006

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.XmlDocs
{
	/// <summary>
	/// Indicates the delegated methods that will be invoked while the specified node type is visiting.
	/// </summary>
	/// <param name="descendants">All descendants.</param>
	public delegate void SyntaxVisitor(SyntaxList<XmlNodeSyntax> descendants);
}
