using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using XmlNodeTriplet = System.ValueTuple<
	string,
	Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlNodeSyntax>?,
	Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax>?
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
		/// Group all XML nodes.
		/// </summary>
		/// <param name="docRoot">The root node of the documentation comments.</param>
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
		/// <param name="paramNodeVisitor">
		/// A delegated method that invokes while the param node is visiting.
		/// </param>
		/// <param name="typeParamNodeVisitor">
		/// A delegated method that invokes while the typeparam node is visiting.
		/// </param>
		/// <param name="seeAlsoNodeVisitor">
		/// A delegated method that invokes while the seealso node is visiting.
		/// </param>
		/// <param name="inheritDocNodeVisitor">
		/// A delegated method that invokes while the inheritdoc node is visiting.
		/// </param>
		/// <returns>
		/// All XML nodes grouped by it markup. This list stores a triplet, where:
		/// <list type="table">
		/// <item>
		/// <term>The first element</term>
		/// <description>The markup of the XML node.</description>
		/// </item>
		/// <item>
		/// <term>The second element</term>
		/// <description>The descendant XML nodes of this node.</description>
		/// </item>
		/// <item>
		/// <term>The third element</term>
		/// <description>The attributes of this XML node.</description>
		/// </item>
		/// </list>
		/// If the node doesn't contain any descendants, the value will be an empty list.
		/// </returns>
		public static IReadOnlyList<XmlNodeTriplet> VisitDescedants(
			this SyntaxNode docRoot,
			Action<XmlElementSyntax, SyntaxList<XmlNodeSyntax>>? summaryNodeVisitor,
			Action<XmlElementSyntax, SyntaxList<XmlNodeSyntax>>? remarksNodeVisitor,
			Action<XmlElementSyntax, SyntaxList<XmlNodeSyntax>>? returnsNodeVisitor,
			Action<XmlElementSyntax, SyntaxList<XmlNodeSyntax>>? valueNodeVisitor,
			Action<XmlElementSyntax, SyntaxList<XmlNodeSyntax>>? paramNodeVisitor,
			Action<XmlElementSyntax, SyntaxList<XmlNodeSyntax>>? typeParamNodeVisitor,
			Action<XmlElementSyntax, SyntaxList<XmlNodeSyntax>>? seeAlsoNodeVisitor,
			Action<XmlEmptyElementSyntax, SyntaxList<XmlAttributeSyntax>>? inheritDocNodeVisitor)
		{
			var result = new List<XmlNodeTriplet>();

			foreach (var markup in docRoot.DescendantNodes())
			{
				switch (markup)
				{
					case XmlElementSyntax
					{
						StartTag: { Name: { LocalName: { ValueText: var tagName } } },
						Content: var contentNodes
					} element:
					{
						switch (tagName)
						{
							case DocCommentBlocks.Summary:
							{
								summaryNodeVisitor?.Invoke(element, contentNodes);
								result.Add((tagName, contentNodes, null));
								break;
							}
							case DocCommentBlocks.Remarks:
							{
								remarksNodeVisitor?.Invoke(element, contentNodes);
								result.Add((tagName, contentNodes, null));
								break;
							}
							case DocCommentBlocks.Returns:
							{
								returnsNodeVisitor?.Invoke(element, contentNodes);
								result.Add((tagName, contentNodes, null));
								break;
							}
							case DocCommentBlocks.Value:
							{
								valueNodeVisitor?.Invoke(element, contentNodes);
								result.Add((tagName, contentNodes, null));
								break;
							}
							case DocCommentBlocks.Param:
							{
								paramNodeVisitor?.Invoke(element, contentNodes);
								result.Add((tagName, contentNodes, null));
								break;
							}
							case DocCommentBlocks.TypeParam:
							{
								typeParamNodeVisitor?.Invoke(element, contentNodes);
								result.Add((tagName, contentNodes, null));
								break;
							}
							case DocCommentBlocks.SeeAlso:
							{
								seeAlsoNodeVisitor?.Invoke(element, contentNodes);
								result.Add((tagName, contentNodes, null));
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
								result.Add((tagName, null, attributes));
								break;
							}
						}

						break;
					}
				}
			}

			return result;
		}
	}
}
