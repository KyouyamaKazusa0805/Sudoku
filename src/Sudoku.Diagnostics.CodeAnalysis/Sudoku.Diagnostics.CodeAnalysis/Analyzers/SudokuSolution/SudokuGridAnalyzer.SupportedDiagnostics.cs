using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class SudokuGridAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3614979&amp;doc_id=633030">
		/// SD0301
		/// </a>
		/// diagnostic result (The member can't be invoked because they are reserved).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0301 = new(
			DiagnosticIds.SD0301, Titles.SD0301, Messages.SD0301, Categories.SD0301,
			DiagnosticSeverities.SD0301, true, helpLinkUri: HelpLinks.SD0301
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SD0301
		);
	}
}
