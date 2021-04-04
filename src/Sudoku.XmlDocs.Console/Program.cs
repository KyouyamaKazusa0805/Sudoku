//using Sudoku.XmlDocs;

//await new OutputService().ExecuteAsync();

using System;
using System.Linq;
using System.Text;
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

foreach (var decl in root.DescendantNodes().OfType<TypeDeclarationSyntax>())
{
	decl.VisitDocDescendants(
		summaryNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) =>
		{
			foreach (var descendant in descendants)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write(descendant.GetType().Name);
				Console.ResetColor();
				Console.Write(": \"");
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.Write(descendant);
				Console.ResetColor();
				Console.WriteLine("\"");
				Console.WriteLine();
			}
		}
	);
}
