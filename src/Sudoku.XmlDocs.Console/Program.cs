//using Sudoku.XmlDocs;

//await new OutputService().ExecuteAsync();

using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.XmlDocs;

var z = CSharpSyntaxTree.ParseText(@"/// <summary>
/// This is an xml doc comment. <see cref=""T(ref int)""/>
/// This is a new line.
/// </summary>
class C
{
	/// <summary>
	/// <para><paramref name=""p""/> is a.</para>
	/// <para><paramref name=""p""/> is b.</para>
	/// <para><paramref name=""p""/> is c.</para>
	/// </summary>
	public void T(ref int p)
	{
		p += 2;
	}
}");
var root = z.GetRoot();

foreach (var node in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
{
	foreach (var trivia in node.GetLeadingTrivia())
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

		int rangeMin = summaryMark.GetLocation().SourceSpan.End;
		int rangeMax = endSummaryMark.GetLocation().SourceSpan.Start;
		foreach (var possibleNode in allPossibleNodes)
		{
			switch (possibleNode)
			{
				case XmlTextSyntax { TextTokens: var textTokens } xmlText:
				{
					foreach (var textToken in textTokens)
					{
						if (textToken.RawKind == (int)SyntaxKind.XmlTextLiteralNewLineToken)
						{
							continue;
						}

						var text = textToken.ValueText.Trim();
						if (string.IsNullOrEmpty(text))
						{
							continue;
						}

#if DEBUG
						Console.WriteLine(text);
#endif
					}

					break;
				}
				case XmlEmptyElementSyntax refOrSee:
				{
					foreach (var crefNode in refOrSee.DescendantNodes())
					{
						switch (crefNode)
						{
							case XmlNameAttributeSyntax { Identifier: { Identifier: { ValueText: var text } } }:
							{
#if DEBUG
								Console.WriteLine($" `{text}` ");
#endif

								break;
							}
							case QualifiedCrefSyntax:
							{
#if DEBUG
								Console.WriteLine($" `{crefNode}` ");
#endif

								break;
							}
							case NameMemberCrefSyntax:
							{
#if DEBUG
								Console.WriteLine($" `{crefNode}` ");
#endif

								break;
							}
						}
					}

					break;
				}
				default:
					break;
			}
		}
	}
}

static bool p(XmlElementStartTagSyntax node) =>
	node.Name.LocalName.ValueText.Equals(
		DocumentationBlockTitles.Summary,
		StringComparison.OrdinalIgnoreCase
	);

static bool q(XmlElementEndTagSyntax node) =>
	node.Name.LocalName.ValueText.Equals(
		DocumentationBlockTitles.Summary,
		StringComparison.OrdinalIgnoreCase
	);