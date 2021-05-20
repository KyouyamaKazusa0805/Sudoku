using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class StackAllocAndNewClauseInCellsAndCandidatesAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4041633&amp;doc_id=633030">
		/// SD0308
		/// </a>
		/// diagnostic result (<see langword="stackalloc"/> or <see langword="new"/> expression is unnecessary).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0308 = new(
			DiagnosticIds.SD0308, Titles.SD0308, Messages.SD0308, Categories.Usage,
			DiagnosticSeverity.Info, true, helpLinkUri: HelpLinks.SD0308
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SD0308
		);
	}
}
