using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.XmlDocs.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="TypeDeclarationSyntax"/>.
	/// </summary>
	/// <seealso cref="TypeDeclarationSyntax"/>
	public static class TypeDeclarationSyntaxEx
	{
		/// <summary>
		/// Like the method <see cref="Type.GetMembers()"/>,
		/// this method will get all members of this declared type, but from the syntax node view.
		/// </summary>
		/// <param name="this">The type declaration node.</param>
		/// <param name="checkNestedTypes">Indicates whether the method will check nested type.</param>
		/// <returns>All possible member declaration syntax nodes.</returns>
		/// <seealso cref="Type.GetMembers()"/>
		public static IEnumerable<MemberDeclarationSyntax> GetMembers(
			this TypeDeclarationSyntax @this, bool checkNestedTypes)
		{
			foreach (var member in @this.DescendantNodes().OfType<MemberDeclarationSyntax>())
			{
				if (checkNestedTypes && member is TypeDeclarationSyntax nestedTypeSyntax)
				{
					foreach (var nestedMembers in nestedTypeSyntax.GetMembers(checkNestedTypes))
					{
						yield return nestedMembers;
					}
				}

				yield return member;
			}
		}
	}
}
