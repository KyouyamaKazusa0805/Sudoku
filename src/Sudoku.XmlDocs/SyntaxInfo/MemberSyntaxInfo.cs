namespace Sudoku.XmlDocs.SyntaxInfo
{
	/// <summary>
	/// Defines a member syntax information.
	/// </summary>
	/// <param name="IdentifierName">The identifier name of this member.</param>
	/// <param name="CustomAccessibility">The accessibility.</param>
	/// <param name="CustomModifier">The custom modifier.</param>
	/// <param name="Summary">The documentation comment for the section "summary".</param>
	/// <param name="Remarks">The documentation comment for the section "remarks".</param>
	public abstract record MemberSyntaxInfo(
		string IdentifierName, CustomAccessibility CustomAccessibility, CustomModifier CustomModifier,
		string? Summary, string? Remarks
	);
}
