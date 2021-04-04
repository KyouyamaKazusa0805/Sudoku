using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.XmlDocs.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="FieldDeclarationSyntax"/>.
	/// </summary>
	/// <seealso cref="FieldDeclarationSyntax"/>
	public static class FieldDeclarationSyntaxEx
	{
		/// <summary>
		/// Get summary content.
		/// </summary>
		/// <param name="this">The node.</param>
		/// <returns>The content.</returns>
		public static string? GetSummary(this FieldDeclarationSyntax @this)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get param content.
		/// </summary>
		/// <param name="this">The node.</param>
		/// <returns>The content.</returns>
		public static (string Param, string? Description)[]? GetParamList(this ParameterListSyntax? @this)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get typeparam content.
		/// </summary>
		/// <param name="this">The node.</param>
		/// <returns>The content.</returns>
		public static (string TypeParam, string? Description)[]? GetTypeParamList(
			this TypeParameterListSyntax? @this)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get remarks content.
		/// </summary>
		/// <param name="this">The node.</param>
		/// <returns>The content.</returns>
		public static string? GetRemarks(this FieldDeclarationSyntax @this)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get example content.
		/// </summary>
		/// <param name="this">The node.</param>
		/// <returns>The content.</returns>
		public static string? GetExample(this FieldDeclarationSyntax @this)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get exceptions content.
		/// </summary>
		/// <param name="this">The node.</param>
		/// <returns>The content.</returns>
		public static (string Exceptions, string? Descriptions)[]? GetExceptions(this FieldDeclarationSyntax @this)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get see-also content.
		/// </summary>
		/// <param name="this">The node.</param>
		/// <returns>The content.</returns>
		public static string[]? GetSeeAlsoList(this FieldDeclarationSyntax @this)
		{
			throw new NotImplementedException();
		}
	}
}
