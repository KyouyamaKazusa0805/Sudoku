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
			DiagnosticIds.SS0701, Titles.SS0701, Messages.SS0701, Categories.Design,
			DiagnosticSeverity.Info, true, helpLinkUri: HelpLinks.SS0701
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0701
		);
	}
}
