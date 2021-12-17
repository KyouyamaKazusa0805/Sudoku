namespace Sudoku.Diagnostics.CodeGen.Data;

/// <summary>
/// Provides and encapsulates a data structure that holds the detail of a diagnostic,
/// extracted by the markdown file <c>DiagnosticList.md</c>.
/// </summary>
/// <param name="Id">
/// Indicates the compiler diagnostic ID used always starting with "SCA" indicating "Solution-wide Code Analysis".
/// </param>
/// <param name="Category">Indicates the category.</param>
/// <param name="Severity">Indicates the severity to report.</param>
/// <param name="Title">Indicates the title displaying on "Error List" window.</param>
/// <param name="MessageFormat">
/// Indicates the message format. The message can contain include placeholders such as <c>{0}</c> and so on
/// to insert whatever you want.
/// </param>
/// <param name="ContainsPlaceholders">
/// Indicates whether the property <see cref="MessageFormat"/> includes any placeholders.
/// The default value is <see langword="false"/>.
/// </param>
/// <param name="HelpLinkUri">Indicates the URL site of the diagnostic introduction.</param>
internal sealed record DiagnosticDetail(
	string Id,
	string Category,
	DiagnosticSeverity Severity,
	string Title,
	string MessageFormat,
	bool ContainsPlaceholders = false,
	string? HelpLinkUri = null
)
{
	/// <summary>
	/// Converts the current instance into a <see cref="DiagnosticDescriptor"/>.
	/// </summary>
	/// <returns>The <see cref="DiagnosticDescriptor"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DiagnosticDescriptor ToDescriptor() =>
		new(Id, Title, MessageFormat, Category, Severity, true, helpLinkUri: HelpLinkUri);
}