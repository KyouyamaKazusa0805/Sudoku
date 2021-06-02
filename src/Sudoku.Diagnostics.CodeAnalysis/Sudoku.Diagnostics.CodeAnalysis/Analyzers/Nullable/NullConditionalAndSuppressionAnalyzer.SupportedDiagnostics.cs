using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class NullConditionalAndSuppressionAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4053883&amp;doc_id=633030">
		/// SS0703
		/// </a>
		/// diagnostic result (Unncessary null-conditional operator <c>?</c>).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0703 = new(
			DiagnosticIds.SS0703, Titles.SS0703, Messages.SS0703, Categories.SS0703,
			DiagnosticSeverities.SS0703, true, helpLinkUri: HelpLinks.SS0703
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4053867&amp;doc_id=633030">
		/// SS0704
		/// </a>
		/// diagnostic result (Unncessary null-forgiving operator <c>!</c>).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0704 = new(
			DiagnosticIds.SS0704, Titles.SS0704, Messages.SS0704, Categories.SS0704,
			DiagnosticSeverities.SS0704, true, helpLinkUri: HelpLinks.SS0704
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0703, SS0704
		);
	}
}
