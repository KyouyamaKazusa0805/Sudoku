//using Sudoku.XmlDocs;

//await new OutputService().ExecuteAsync();

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.XmlDocs.Extensions;

const string testCode = @"
/// <summary>
/// This is an xml doc comment. <see cref=""T(ref int)""/>
/// <code>
/// foreach (var item in list)
/// {
///     Console.WriteLine(item.ToString());
/// }
/// </code>
/// This is a new line.
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

bool traverse(XmlNodeSyntax descendant)
{
	if (isWhiteOrTripleSlashOnly(descendant))
	{
		return false;
	}

	Console.ForegroundColor = ConsoleColor.Red;
	Console.Write(descendant.GetType().Name);
	Console.ResetColor();
	Console.Write(": \"");
	Console.ForegroundColor = ConsoleColor.Blue;
	Console.Write(descendant);
	Console.ResetColor();
	Console.WriteLine("\"");
	Console.WriteLine();

	return true;
}