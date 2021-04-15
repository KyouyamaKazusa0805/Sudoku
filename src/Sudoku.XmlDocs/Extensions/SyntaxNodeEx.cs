using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.XmlDocs.Values;

namespace Sudoku.XmlDocs.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SyntaxNode"/>.
	/// </summary>
	/// <seealso cref="SyntaxNode"/>
	public static class SyntaxNodeEx
	{
		/// <summary>
		/// Indicates the syntax kind of the doc trivia.
		/// </summary>
		private const SyntaxKind DocTrivia = SyntaxKind.SingleLineDocumentationCommentTrivia;


		/// <summary>
		/// Visits all documentation comment XML nodes.
		/// </summary>
		/// <param name="this">The root node of the documentation comments.</param>
		/// <param name="summaryNodeVisitor">
		/// A delegated method that invokes while the "summary" node is visiting.
		/// </param>
		/// <param name="remarksNodeVisitor">
		/// A delegated method that invokes while the "remarks" node is visiting.
		/// </param>
		/// <param name="returnsNodeVisitor">
		/// A delegated method that invokes while the "returns" node is visiting.
		/// </param>
		/// <param name="valueNodeVisitor">
		/// A delegated method that invokes while the "value" node is visiting.
		/// </param>
		/// <param name="exampleNodeVisitor">
		/// A delegated method that invokes while the "example" node is visiting.
		/// </param>
		/// <param name="paramNodeVisitor">
		/// A delegated method that invokes while the "param" node is visiting.
		/// </param>
		/// <param name="typeParamNodeVisitor">
		/// A delegated method that invokes while the "typeparam" node is visiting.
		/// </param>
		/// <param name="seeAlsoNodeVisitor">
		/// A delegated method that invokes while the "seealso" node is visiting.
		/// </param>
		/// <param name="exceptionNodeVisitor">
		/// A delegated method that invokes while the "exception" node is visiting.
		/// </param>
		/// <param name="inheritDocNodeVisitor">
		/// A delegated method that invokes while the "inheritdoc" node is visiting.
		/// </param>
		public static void VisitDocDescendants(
			this SyntaxNode @this, SyntaxVisitor? summaryNodeVisitor,
			SyntaxVisitor? remarksNodeVisitor, SyntaxVisitor? returnsNodeVisitor,
			SyntaxVisitor? valueNodeVisitor, SyntaxVisitor? exampleNodeVisitor,
			SyntaxVisitor? paramNodeVisitor, SyntaxVisitor? typeParamNodeVisitor,
			SyntaxVisitor? seeAlsoNodeVisitor, SyntaxVisitor? exceptionNodeVisitor,
			SyntaxVisitor? inheritDocNodeVisitor)
		{
			switch (@this)
			{
				case MemberDeclarationSyntax:
				{
					foreach (var trivia in @this.GetLeadingTrivia())
					{
						if (trivia is (kind: DocTrivia, structure: { } structured))
						{
							onVisiting(
								structured, summaryNodeVisitor, remarksNodeVisitor, returnsNodeVisitor,
								valueNodeVisitor, exampleNodeVisitor, paramNodeVisitor, typeParamNodeVisitor,
								seeAlsoNodeVisitor, exceptionNodeVisitor, inheritDocNodeVisitor
							);
						}
					}

					break;
				}
				case XmlNodeSyntax:
				{
					onVisiting(
						@this, summaryNodeVisitor, remarksNodeVisitor, returnsNodeVisitor,
						valueNodeVisitor, exampleNodeVisitor, paramNodeVisitor, typeParamNodeVisitor,
						seeAlsoNodeVisitor, exceptionNodeVisitor, inheritDocNodeVisitor
					);

					break;
				}
			}


			static void onVisiting(
				SyntaxNode docRoot, SyntaxVisitor? summary, SyntaxVisitor? remarks, SyntaxVisitor? returns,
				SyntaxVisitor? value, SyntaxVisitor? example, SyntaxVisitor? param, SyntaxVisitor? typeParam,
				SyntaxVisitor? seeAlso, SyntaxVisitor? exception, SyntaxVisitor? inheritDoc)
			{
				foreach (var markup in docRoot.DescendantNodes())
				{
					switch (markup)
					{
						case XmlElementSyntax
						{
							StartTag: { Name: { LocalName: { ValueText: var tagName } } },
							Content: var contentNodes
						}:
						{
							(
								tagName switch
								{
									DocCommentBlocks.Summary => summary,
									DocCommentBlocks.Remarks => remarks,
									DocCommentBlocks.Returns => returns,
									DocCommentBlocks.Example => example,
									DocCommentBlocks.Exception => exception,
									DocCommentBlocks.Value => value,
									DocCommentBlocks.Param => param,
									DocCommentBlocks.TypeParam => typeParam,
									DocCommentBlocks.SeeAlso => seeAlso,
									_ => null
								}
							)?.Invoke(contentNodes);

							break;
						}
						case XmlEmptyElementSyntax { Name: { LocalName: { ValueText: var tagName } } }:
						{
							(
								tagName switch
								{
									DocCommentBlocks.InheritDoc => inheritDoc,
									_ => null
								}
							)?.Invoke(default);

							break;
						}
					}
				}
			}
		}
	}
}
