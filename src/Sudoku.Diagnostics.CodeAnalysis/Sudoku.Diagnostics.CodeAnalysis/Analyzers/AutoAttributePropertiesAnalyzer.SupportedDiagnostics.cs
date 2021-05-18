using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class AutoAttributePropertiesAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4021986&amp;doc_id=633030">
		/// SD0401
		/// </a>
		/// diagnostic result (Please use nameof expression instead of string literal).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0401 = new(
			DiagnosticIds.SD0401, Titles.SD0401, Messages.SD0401, Categories.Usage,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SD0401
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4024928&amp;doc_id=633030">
		/// SD0402
		/// </a>
		/// diagnostic result (This attribute must contain the specified number of parameters).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0402 = new(
			DiagnosticIds.SD0402, Titles.SD0402, Messages.SD0402, Categories.Usage,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SD0402
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SD0401, SD0402
		);
	}
}
