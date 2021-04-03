using Microsoft.CodeAnalysis;

namespace Sudoku.XmlDocs.SyntaxInfo
{
	/// <summary>
	/// Defines a member syntax information.
	/// </summary>
	/// <param name="SyntaxNode">The current syntax node.</param>
	/// <param name="IdentifierName">
	/// The identifier name of this member. <see langword="null"/> if the member is the
	/// <see langword="record"/>'s primary constructor; otherwise, not <see langword="null"/>.
	/// </param>
	/// <param name="CustomAccessibility">The accessibility.</param>
	/// <param name="CustomModifier">The custom modifier.</param>
	/// <param name="Summary">The documentation comment for the section "summary".</param>
	/// <param name="Remarks">The documentation comment for the section "remarks".</param>
	/// <param name="Example">The documentation comment for the section "example".</param>
	/// <param name="ExceptionList">The <c>exception</c> list.</param>
	/// <param name="SeeAlsoList">The <c>seealso</c> list.</param>
	public abstract record MemberSyntaxInfo(
		SyntaxNode SyntaxNode, string? IdentifierName,
		CustomAccessibility CustomAccessibility, CustomModifier CustomModifier,
		string? Summary, string? Remarks, string? Example,
		(string Exception, string? Description)[]? ExceptionList, string[]? SeeAlsoList
	);
}
