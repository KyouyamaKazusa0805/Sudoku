using System.Text.Markdown;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.XmlDocs.Extensions;

namespace Sudoku.XmlDocs.SyntaxInfo
{
	/// <summary>
	/// Defines a primary constructor syntax information.
	/// </summary>
	/// <param name="SyntaxNode">The current syntax node.</param>
	/// <param name="CustomAccessibility">The accessibility.</param>
	/// <param name="ParamList">The <c>param</c> list.</param>
	/// <param name="ExceptionList">The <c>exception</c> list.</param>
	/// <param name="SeeAlsoList">The <c>seealso</c> list.</param>
	public sealed record PrimaryConstructorSyntaxInfo(
		SyntaxNode SyntaxNode, CustomAccessibility CustomAccessibility,
		(string ParamName, string Type, string? Description)[]? ParamList,
		(string Exception, string? Description)[]? ExceptionList,
		string[]? SeeAlsoList
	) : MemberSyntaxInfo(
		SyntaxNode, IdentifierName: null, CustomAccessibility, CustomModifier.None,
		Summary: null, Remarks: null, Example: null, ExceptionList, SeeAlsoList
	)
	{
		/// <inheritdoc/>
		public override string ToString() => Document.Create()
			.AppendTitle(MemberKind.PrimaryConstructor, string.Empty)
			.AppendSummary(Summary)
			.AppendRemarks(Remarks)
			.AppendParams(ParamList, ((RecordDeclarationSyntax)SyntaxNode).ParameterList?.Parameters)
			.AppendException(ExceptionList)
			.ToString();
	}
}
