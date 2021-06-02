using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class SudokuGridFormatStringAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4050196&amp;doc_id=633030">
		/// SD0310
		/// </a>
		/// diagnostic result (Please add the format string as the argument into the method invocation
		/// of <c>SudokuGrid</c>, such as <c>"."</c> for default case, or <c>"#"</c> for
		/// intelligent-handling case).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0310 = new(
			DiagnosticIds.SD0310, Titles.SD0310, Messages.SD0310, Categories.SD0310,
			DiagnosticSeverities.SD0310, true, helpLinkUri: HelpLinks.SD0310
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4050414&amp;doc_id=633030">
		/// SD0311
		/// </a>
		/// diagnostic result (Invalid format string in <c>SudokuGrid.ToString</c>).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0311 = new(
			DiagnosticIds.SD0311, Titles.SD0311, Messages.SD0311, Categories.SD0311,
			DiagnosticSeverities.SD0311, true, helpLinkUri: HelpLinks.SD0311
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SD0310, SD0311
		);
	}
}
