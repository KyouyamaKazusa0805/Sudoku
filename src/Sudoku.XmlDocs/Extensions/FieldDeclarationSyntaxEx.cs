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
			foreach (var trivia in @this.GetLeadingTrivia())
			{
				if (trivia.RawKind != (int)SyntaxKind.SingleLineDocumentationCommentTrivia)
				{
					continue;
				}

				var docNodes = trivia.GetStructure();
				if (docNodes is null)
				{
					continue;
				}

				var allPossibleNodes = docNodes.DescendantNodes();
				var summaryMark = allPossibleNodes.OfType<XmlElementStartTagSyntax>().FirstOrDefault(p);
				if (summaryMark is null)
				{
					continue;
				}

				var endSummaryMark = allPossibleNodes.OfType<XmlElementEndTagSyntax>().FirstOrDefault(q);
				if (endSummaryMark is null)
				{
					continue;
				}

				var (rangeMin, rangeMax) = summaryMark.GetLocation().SourceSpan;
				foreach (var possibleNode in allPossibleNodes)
				{
					switch (possibleNode)
					{
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

			return sb.ToString();


			static bool p(XmlElementStartTagSyntax node) =>
				node.Name.LocalName.ValueText.Equals(
					DocumentationBlockTitles.Summary, StringComparison.OrdinalIgnoreCase
				);

			static bool q(XmlElementEndTagSyntax node) =>
				node.Name.LocalName.ValueText.Equals(
					DocumentationBlockTitles.Summary, StringComparison.OrdinalIgnoreCase
				);
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
	}
}
