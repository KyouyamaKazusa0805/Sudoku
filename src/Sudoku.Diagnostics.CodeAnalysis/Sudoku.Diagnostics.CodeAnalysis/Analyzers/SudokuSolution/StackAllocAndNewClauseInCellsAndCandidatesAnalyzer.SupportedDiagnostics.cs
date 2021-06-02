using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class StackAllocAndNewClauseInCellsAndCandidatesAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4041633&amp;doc_id=633030">
		/// SD0309
		/// </a>
		/// diagnostic result (<see langword="stackalloc"/> or <see langword="new"/> expression is unnecessary).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0309 = new(
			DiagnosticIds.SD0309, Titles.SD0309, Messages.SD0309, Categories.SD0309,
			DiagnosticSeverities.SD0309, true, helpLinkUri: HelpLinks.SD0309
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SD0309
		);
	}
}
