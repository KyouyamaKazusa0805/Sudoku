using Microsoft.CodeAnalysis;

namespace Sudoku.XmlDocs.SyntaxInfo
{
	/// <summary>
	/// Defines a member syntax information.
	/// </summary>
	/// <param name="SyntaxNode">The current syntax node.</param>
	/// <param name="SemanticModel">The current semantic model.</param>
	/// <param name="IdentifierName">The identifier name of this member.</param>
	/// <param name="CustomAccessibility">The accessibility.</param>
	/// <param name="CustomModifier">The custom modifier.</param>
	/// <param name="Summary">The documentation comment for the section "summary".</param>
	/// <param name="Remarks">The documentation comment for the section "remarks".</param>
	/// <param name="Example">The documentation comment for the section "example".</param>
	/// <param name="ExceptionList">The exception list.</param>
	public abstract record MemberSyntaxInfo(
		SyntaxNode SyntaxNode, SemanticModel SemanticModel, string IdentifierName,
		CustomAccessibility CustomAccessibility, CustomModifier CustomModifier,
		string? Summary, string? Remarks, string? Example,
		(string[]? Exceptions, string?[]? Descriptions) ExceptionList
	);
}
