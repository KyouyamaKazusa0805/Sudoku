using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IPropertySymbol"/>.
	/// </summary>
	/// <seealso cref="IPropertySymbol"/>
	public static class IPropertySymbolEx
	{
		/// <summary>
		/// To find the node by the specified property symbol.
		/// </summary>
		/// <param name="this">The symbol.</param>
		/// <param name="syntaxNode">The syntax node.</param>
		/// <param name="semanticModel">The semantic model.</param>
		/// <returns>The property node.</returns>
		public static PropertyDeclarationSyntax? FindMatchingNode(
			this IPropertySymbol @this, SyntaxNode syntaxNode, SemanticModel semanticModel) =>
			syntaxNode.DescendantNodes().FirstOrDefault(
				node => SymbolEqualityComparer.Default.Equals(@this, semanticModel.GetDeclaredSymbol(node))
			) as PropertyDeclarationSyntax;
	}
}
