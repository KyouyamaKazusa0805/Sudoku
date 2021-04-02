using System;
using System.Linq;
using System.Text;
using System.Text.Markdown;
using Microsoft.CodeAnalysis.CSharp;
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
			var sb = new StringBuilder();

			// Iterate on all trivia.
			foreach (var trivia in @this.GetLeadingTrivia())
			{
				// Check whether the curren trivia is the doc comment.
				if (trivia.RawKind != (int)SyntaxKind.SingleLineDocumentationCommentTrivia)
				{
					continue;
				}

				// Now converts it to the structure (syntax nodes).
				var docNodes = trivia.GetStructure();
				if (docNodes is null)
				{
					continue;
				}

				// Check whether the "summary" part exists.
				// If so, get the start tag location.
				var possibleNodes = docNodes.DescendantNodes();
				var summaryMark = possibleNodes.OfType<XmlElementStartTagSyntax>().FirstOrDefault(startIsSummary);
				if (summaryMark is null)
				{
					continue;
				}

				// Get the end tag location.
				var endSummaryMark = possibleNodes.OfType<XmlElementEndTagSyntax>().FirstOrDefault(endIsSummary);
				if (endSummaryMark is null)
				{
					continue;
				}

				// Creates the location range ('TextSpan' structure instance).
				var (rangeMin, rangeMax) = summaryMark.GetLocation().SourceSpan;

				// Iterate on each possible doc comment syntax node.
				foreach (var possibleNode in possibleNodes)
				{
					// Use pattern matching to check the syntax node type and its inner values.
					switch (possibleNode)
					{
						// XmlTextSyntax: Normal text.
						case XmlTextSyntax { TextTokens: var textTokens } xmlText
						when xmlText.GetLocation().IsInRange(rangeMin, rangeMax):
						{
							foreach (var textToken in textTokens)
							{
								if (
									textToken is
									{
										RawKind: not (int)SyntaxKind.XmlTextLiteralNewLineToken,
										ValueText: var valueText
									}
									&& valueText.Trim() is var text
									&& !string.IsNullOrEmpty(text)
								)
								{
									sb.Append(text);
								}
							}

							break;
						}

						// XmlEmptyElementSyntax: The inline XML markup, with no value,
						// such as '<seealso cref="A.B(ref int)" />'.
						case XmlEmptyElementSyntax refOrSee
						when refOrSee.GetLocation().IsInRange(rangeMin, rangeMax):
						{
							foreach (var crefNode in refOrSee.DescendantNodes())
							{
								switch (crefNode)
								{
									case XmlNameAttributeSyntax
									{
										Identifier: { Identifier: { ValueText: var text } }
									}:
									{
										sb.AppendInlineCodeBlock(text, true);
										break;
									}
									case QualifiedCrefSyntax:
									{
										sb.AppendInlineCodeBlock(crefNode.ToString(), true);
										break;
									}
									case NameMemberCrefSyntax:
									{
										sb.AppendInlineCodeBlock(crefNode.ToString(), true);
										break;
									}
								}
							}

							break;
						}
					}
				}
			}

			// Returns the value.
			return sb.ToString();


			static bool startIsSummary(XmlElementStartTagSyntax node) => node.IsMarkup(DocComments.Summary);
			static bool endIsSummary(XmlElementEndTagSyntax node) => node.IsMarkup(DocComments.Summary);
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
