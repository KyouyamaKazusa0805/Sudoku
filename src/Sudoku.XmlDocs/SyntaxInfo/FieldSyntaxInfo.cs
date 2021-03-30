using System.Text.Markdown;
using Sudoku.XmlDocs.Extensions;

namespace Sudoku.XmlDocs.SyntaxInfo
{
	/// <summary>
	/// Defines a field syntax information.
	/// </summary>
	/// <param name="IdentifierName">The identifier name of this member.</param>
	/// <param name="CustomAccessibility">The accessibility.</param>
	/// <param name="CustomModifier">The custom modifier.</param>
	/// <param name="Summary">The documentation comment for the section "summary".</param>
	/// <param name="Remarks">The documentation comment for the section "remarks".</param>
	public sealed record FieldSyntaxInfo(
		string IdentifierName, CustomAccessibility CustomAccessibility, CustomModifier CustomModifier,
		string? Summary, string? Remarks
	) : MemberSyntaxInfo(IdentifierName, CustomAccessibility, CustomModifier, Summary, Remarks)
	{
		/// <inheritdoc/>
		public override string ToString() => Document
			.Create()
			.AppendTitle(MemberKind.Field, IdentifierName)
			.AppendSummary(Summary)
			.AppendRemarks(Remarks)
			.ToString();
	}
}
