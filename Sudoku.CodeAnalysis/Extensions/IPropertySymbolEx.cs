using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeAnalysis.Extensions
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
		/// <param name="this">(<see langword="this"/> parameter) The symbol.</param>
		/// <param name="syntaxTree">The syntax tree.</param>
		/// <param name="semanticModel">The semantic model.</param>
		/// <returns>The property node.</returns>
		public static PropertyDeclarationSyntax? FindMatchingNode(
			this IPropertySymbol @this, SyntaxNode syntaxNode, SemanticModel semanticModel) =>
			syntaxNode.DescendantNodes().FirstOrDefault(
				node =>
				{
					var symbol = semanticModel.GetDeclaredSymbol(node);
					return SymbolEqualityComparer.Default.Equals(@this, symbol);
				}
			) as PropertyDeclarationSyntax;
	}
}
