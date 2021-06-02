using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class CompoundNullCoalescingAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4053857&amp;doc_id=633030">
		/// SS0701
		/// </a>
		/// diagnostic result (The expression can be simplified to using
		/// compound null-coalesce operator <c>??=</c>).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0701 = new(
			DiagnosticIds.SS0701, Titles.SS0701, Messages.SS0701, Categories.SS0701,
			DiagnosticSeverities.SS0701, true, helpLinkUri: HelpLinks.SS0701
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4077126&amp;doc_id=633030">
		/// SS0705
		/// </a>
		/// diagnostic result (Using compound null-coalesce operator <c>??=</c> is unncessary).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0705 = new(
			DiagnosticIds.SS0705, Titles.SS0705, Messages.SS0705, Categories.SS0705,
			DiagnosticSeverities.SS0705, true, helpLinkUri: HelpLinks.SS0705
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0701, SS0705
		);
	}
}
