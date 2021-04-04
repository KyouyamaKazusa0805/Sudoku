using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.XmlDocs.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SyntaxNode"/>.
	/// </summary>
	/// <seealso cref="SyntaxNode"/>
	public static class SyntaxNodeEx
	{
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
			this SyntaxNode @this,
			SyntaxVisitor? summaryNodeVisitor = null, SyntaxVisitor? remarksNodeVisitor = null,
			SyntaxVisitor? returnsNodeVisitor = null, SyntaxVisitor? valueNodeVisitor = null,
			SyntaxVisitor? exampleNodeVisitor = null, SyntaxVisitor? paramNodeVisitor = null,
			SyntaxVisitor? typeParamNodeVisitor = null, SyntaxVisitor? seeAlsoNodeVisitor = null,
			AttributedSyntaxVisitor? exceptionNodeVisitor = null,
			AttributedSyntaxVisitorWithoutDescendants? inheritDocNodeVisitor = null)
		{
			switch (@this)
			{
				case MemberDeclarationSyntax:
				{
					foreach (var (kind, structured) in @this.GetLeadingTrivia())
					{
						if (kind != SyntaxKind.SingleLineDocumentationCommentTrivia)
						{
							continue;
						}

						if (structured is null)
						{
							continue;
						}

						onVisiting(
							structured, summaryNodeVisitor, remarksNodeVisitor, returnsNodeVisitor,
							valueNodeVisitor, exampleNodeVisitor, paramNodeVisitor, typeParamNodeVisitor,
							seeAlsoNodeVisitor, exceptionNodeVisitor, inheritDocNodeVisitor
						);
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
				SyntaxNode docRoot, SyntaxVisitor? summaryNodeVisitor, SyntaxVisitor? remarksNodeVisitor,
				SyntaxVisitor? returnsNodeVisitor, SyntaxVisitor? valueNodeVisitor,
				SyntaxVisitor? exampleNodeVisitor, SyntaxVisitor? paramNodeVisitor,
				SyntaxVisitor? typeParamNodeVisitor, SyntaxVisitor? seeAlsoNodeVisitor,
				AttributedSyntaxVisitor? exceptionNodeVisitor,
				AttributedSyntaxVisitorWithoutDescendants? inheritDocNodeVisitor)
			{
				foreach (var markup in docRoot.DescendantNodes())
				{
					switch (markup)
					{
						case XmlElementSyntax
						{
							StartTag:
							{
								Name: { LocalName: { ValueText: var tagName } },
								Attributes: var attributes
							},
							Content: var contentNodes
						} element:
						{
							switch (tagName)
							{
								case DocCommentBlocks.Summary:
								{
									summaryNodeVisitor?.Invoke(element, contentNodes);
									break;
								}
								case DocCommentBlocks.Remarks:
								{
									remarksNodeVisitor?.Invoke(element, contentNodes);
									break;
								}
								case DocCommentBlocks.Returns:
								{
									returnsNodeVisitor?.Invoke(element, contentNodes);
									break;
								}
								case DocCommentBlocks.Example:
								{
									exampleNodeVisitor?.Invoke(element, contentNodes);
									break;
								}
								case DocCommentBlocks.Exception:
								{
									exceptionNodeVisitor?.Invoke(element, attributes, contentNodes);
									break;
								}
								case DocCommentBlocks.Value:
								{
									valueNodeVisitor?.Invoke(element, contentNodes);
									break;
								}
								case DocCommentBlocks.Param:
								{
									paramNodeVisitor?.Invoke(element, contentNodes);
									break;
								}
								case DocCommentBlocks.TypeParam:
								{
									typeParamNodeVisitor?.Invoke(element, contentNodes);
									break;
								}
								case DocCommentBlocks.SeeAlso:
								{
									seeAlsoNodeVisitor?.Invoke(element, contentNodes);
									break;
								}
							}

							break;
						}
						case XmlEmptyElementSyntax
						{
							Name: { LocalName: { ValueText: var tagName } },
							Attributes: var attributes
						} emptyElement:
						{
							switch (tagName)
							{
								case DocCommentBlocks.InheritDoc:
								{
									inheritDocNodeVisitor?.Invoke(emptyElement, attributes);
									break;
								}
							}

							break;
						}
					}
				}
			}
		}
	}
}
