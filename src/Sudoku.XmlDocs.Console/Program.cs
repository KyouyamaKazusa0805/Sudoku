//using Sudoku.XmlDocs;

//await new OutputService().ExecuteAsync();

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.XmlDocs;
using Sudoku.XmlDocs.Extensions;

const string langWordAttributeText = "langword";
const string testCode = @"
/// <summary>
/// <para>
/// This is an xml doc comment. <see cref=""T(ref int)""/>
/// <code>
/// foreach (var item in list)
/// {
///     Console.WriteLine(item.ToString());
/// }
/// </code>
/// This is a new line.
/// </para>
/// </summary>
/// <remarks>
/// Remarks.
/// </remarks>
class C
{
    /// <summary>
    /// <para><paramref name=""p""/> is a.</para>
    /// <para><paramref name=""p""/> is b.</para>
    /// <para><paramref name=""p""/> is c.</para>
    /// </summary>
	/// <typeparam name=""TArg"">The type argument.</typeparam>
	/// <example>Examples.</example>
    public void T<TArg>(ref int p) where TArg : struct
    {
        p += 2;
    }
}";
var root = CSharpSyntaxTree.ParseText(testCode).GetRoot();
var sb = new StringBuilder();
var emptyCharsRegex = new Regex(
	pattern: @"\s*\r\n\s*///\s*",
	options: RegexOptions.Compiled | RegexOptions.ExplicitCapture,
	matchTimeout: TimeSpan.FromSeconds(5)
);

bool isWhiteOrTripleSlashOnly(XmlNodeSyntax node)
{
	string s = node.ToString();
	var match = emptyCharsRegex.Match(s);
	return match.Success && match.Value == s;
}

foreach (var decl in root.DescendantNodes().OfType<TypeDeclarationSyntax>())
{
	// Get its docs.
	decl.VisitDocDescendants(
		summaryNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) =>
		{
			foreach (var descendant in descendants)
			{
				traverse(descendant);
			}
		}
	);

	// Get all member docs.
	foreach (var member in decl.GetMembers(checkNestedTypes: true))
	{
		Console.Write("Member ");
		Console.WriteLine();

		member.VisitDocDescendants(
			summaryNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) =>
			{
				foreach (var descendant in descendants)
				{
					traverse(descendant);
				}
			}
		);
	}
}

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine(sb.ToString());
Console.ResetColor();

bool traverse(XmlNodeSyntax descendant)
{
	if (isWhiteOrTripleSlashOnly(descendant))
	{
		return false;
	}

	switch (descendant)
	{
		case XmlTextSyntax { TextTokens: var tokens }:
		{
			foreach (var token in tokens)
			{
				string text = token.ValueText;
				if (!string.IsNullOrWhiteSpace(text) && text != Environment.NewLine)
				{
					sb.Append(text.TrimStart());
				}
			}

			break;
		}
		case XmlEmptyElementSyntax
		{
			Name: { LocalName: { ValueText: var markup } },
			Attributes: var attributes
		} when attributes[0] is { Name: { LocalName: { ValueText: var xmlPrefixName } } } firstAttribute:
		{
			string attributeValueText = (SyntaxKind)firstAttribute.RawKind switch
			{
				SyntaxKind.XmlCrefAttribute when firstAttribute is XmlCrefAttributeSyntax
				{
					Cref: var crefNode
				} => crefNode.ToString(),
				SyntaxKind.XmlNameAttribute when firstAttribute is XmlNameAttributeSyntax
				{
					Identifier: { Identifier: { ValueText: var identifier } }
				} => identifier,
				SyntaxKind.XmlTextAttribute when firstAttribute is XmlTextAttributeSyntax
				{
					Name: { LocalName: { ValueText: langWordAttributeText } },
					TextTokens: { Count: not 0 } tokenList
				} && tokenList[0] is { ValueText: var firstTokenText } => firstTokenText
			};

			switch (markup)
			{
				case DocCommentBlocks.See:
				{
					attributeValueText = attributeValueText.Replace('{', '<').Replace('}', '>');
					if (!char.IsPunctuation(sb[^2]) || !char.IsWhiteSpace(sb[^1]))
					{
						sb.Append(' ');
					}

					sb.Append($"`{attributeValueText}` ");
					break;
				}
				case DocCommentBlocks.ParamRef:
				{
					if (!char.IsPunctuation(sb[^2]) || !char.IsWhiteSpace(sb[^1]))
					{
						sb.Append(' ');
					}

					sb.Append($"`{attributeValueText}` ");
					break;
				}
				case DocCommentBlocks.TypeParamRef:
				{
					if (!char.IsPunctuation(sb[^2]) || !char.IsWhiteSpace(sb[^1]))
					{
						sb.Append(' ');
					}

					sb.Append($"`{attributeValueText}` ");
					break;
				}
			}

			break;
		}
		case XmlElementSyntax
		{
			StartTag:
			{
				Name: { LocalName: { ValueText: var markup } },
				Attributes: { Count: 0 }
			},
			Content: var content
		} when content.ToString() is var contentText:
		{
			switch (markup)
			{
				case DocCommentBlocks.Para:
				{
					foreach (var descendantInner in content)
					{
						// Handle it recursively.
						traverse(descendantInner);
					}

					sb.AppendLine().AppendLine();

					break;
				}
				case DocCommentBlocks.C:
				{
					if (!char.IsPunctuation(sb[^2]) || !char.IsWhiteSpace(sb[^1]))
					{
						sb.Append(' ');
					}

					sb.Append($"`{contentText}` ");

					break;
				}
				case DocCommentBlocks.Code:
				{
					// Trimming. We should remove all unncessary text.
					contentText = Regex
						.Replace(contentText, @"(?<=\r\n)\s*(///\s+?)", static _ => string.Empty)
						.Trim(new[] { '\r', '\n', ' ' });

					if (sb.ToString() != string.Empty)
					{
						// If the context contains any characters, we should turn to a new line
						// to output the code block.
						sb.AppendLine().AppendLine();
					}

					sb
						.AppendLine("```csharp")    // Code block start
						.AppendLine(contentText)    // Code content
						.AppendLine("```")          // Code block end
						.AppendLine().AppendLine(); // New line

					break;
				}
			}

			break;
		}
	}

#if DEBUG
	Console.ForegroundColor = ConsoleColor.Red;
	Console.Write(descendant.GetType().Name);
	Console.ResetColor();
	Console.Write(": \"");
	Console.ForegroundColor = ConsoleColor.Blue;
	Console.Write(descendant);
	Console.ResetColor();
	Console.WriteLine("\"");
	Console.WriteLine();
#endif

	return true;
}