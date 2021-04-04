using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Visitor = System.Action<
	Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax,
	Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax>
>;
using VisitorWithAttributes = System.Action<
	Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax,
	Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax>,
	Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax>
>;
using VisitorWithoutValue = System.Action<
	Microsoft.CodeAnalysis.CSharp.Syntax.XmlEmptyElementSyntax,
	Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax>
>;

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
		/// A delegated method that invokes while the summary node is visiting.
		/// </param>
		/// <param name="remarksNodeVisitor">
		/// A delegated method that invokes while the remarks node is visiting.
		/// </param>
		/// <param name="returnsNodeVisitor">
		/// A delegated method that invokes while the returns node is visiting.
		/// </param>
		/// <param name="valueNodeVisitor">
		/// A delegated method that invokes while the value node is visiting.
		/// </param>
		/// <param name="exampleNodeVisitor">
		/// A delegated method that invokes while the example node is visiting.
		/// </param>
		/// <param name="paramNodeVisitor">
		/// A delegated method that invokes while the param node is visiting.
		/// </param>
		/// <param name="typeParamNodeVisitor">
		/// A delegated method that invokes while the typeparam node is visiting.
		/// </param>
		/// <param name="seeAlsoNodeVisitor">
		/// A delegated method that invokes while the seealso node is visiting.
		/// </param>
		/// <param name="exceptionNodeVisitor">
		/// A delegated method that invokes while the exception node is visiting.
		/// </param>
		/// <param name="inheritDocNodeVisitor">
		/// A delegated method that invokes while the inheritdoc node is visiting.
		/// </param>
		public static void VisitDocDescendants(
			this SyntaxNode @this, Visitor? summaryNodeVisitor = null, Visitor? remarksNodeVisitor = null,
			Visitor? returnsNodeVisitor = null, Visitor? valueNodeVisitor = null,
			Visitor? exampleNodeVisitor = null, Visitor? paramNodeVisitor = null,
			Visitor? typeParamNodeVisitor = null, Visitor? seeAlsoNodeVisitor = null,
			VisitorWithAttributes? exceptionNodeVisitor = null,
			VisitorWithoutValue? inheritDocNodeVisitor = null)
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
				SyntaxNode docRoot, Visitor? summaryNodeVisitor, Visitor? remarksNodeVisitor,
				Visitor? returnsNodeVisitor, Visitor? valueNodeVisitor, Visitor? exampleNodeVisitor,
				Visitor? paramNodeVisitor, Visitor? typeParamNodeVisitor, Visitor? seeAlsoNodeVisitor,
				VisitorWithAttributes? exceptionNodeVisitor, VisitorWithoutValue? inheritDocNodeVisitor)
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
