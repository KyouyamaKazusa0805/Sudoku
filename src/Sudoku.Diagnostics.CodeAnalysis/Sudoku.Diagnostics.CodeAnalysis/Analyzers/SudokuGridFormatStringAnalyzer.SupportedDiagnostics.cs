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
			DiagnosticIds.SD0310, Titles.SD0310, Messages.SD0310, Categories.Usage,
			DiagnosticSeverity.Info, true, helpLinkUri: HelpLinks.SD0310
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SD0310
		);
	}
}
