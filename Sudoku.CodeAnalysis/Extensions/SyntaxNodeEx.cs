using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharpExtensions;

namespace Sudoku.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SyntaxNode"/>.
	/// </summary>
	/// <seealso cref="SyntaxNode"/>
	public static class SyntaxNodeEx
	{
		/// <summary>
		/// To check whether the specified syntax node is the specified syntax kind in the list as the parameter
		/// <paramref name="syntaxKinds"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The syntax node to check.</param>
		/// <param name="syntaxKinds">The syntax kinds to check one by one.</param>
		/// <returns>The <see cref="bool"/> result indicating that.</returns>
		public static bool IsKind(this SyntaxNode @this, params SyntaxKind[] syntaxKinds) =>
			syntaxKinds.Any(syntaxKind => CSharpExtensions.IsKind(@this, syntaxKind));
	}
}
