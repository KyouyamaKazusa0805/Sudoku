using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class RepeatedPropertyPathAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4071853&amp;doc_id=633030">
		/// SS0623
		/// </a>
		/// diagnostic result (Repeated property path).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0623 = new(
			DiagnosticIds.SS0623, Titles.SS0623, Messages.SS0623, Categories.SS0623,
			DiagnosticSeverities.SS0623, true, helpLinkUri: HelpLinks.SS0623
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4071856&amp;doc_id=633030">
		/// SS0625
		/// </a>
		/// diagnostic result (Repeated property path in extended property pattern).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0625 = new(
			DiagnosticIds.SS0625, Titles.SS0625, Messages.SS0625, Categories.SS0625,
			DiagnosticSeverities.SS0625, true, helpLinkUri: HelpLinks.SS0625
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0623, SS0625
		);
	}
}
