using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class CellsAndCandidatesInitializerAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4041611&amp;doc_id=633030">
		/// SD0305
		/// </a>
		/// diagnostic result (The input value in this initializer is invalid).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0305 = new(
			DiagnosticIds.SD0305, Titles.SD0305, Messages.SD0305, Categories.Usage,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.SD0305
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4041904&amp;doc_id=633030">
		/// SD0306
		/// </a>
		/// diagnostic result (The remove expression in the initializer
		/// following with the constructor has no effect).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0306 = new(
			DiagnosticIds.SD0306, Titles.SD0306, Messages.SD0306, Categories.Usage,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SD0306
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4042004&amp;doc_id=633030">
		/// SD0307
		/// </a>
		/// diagnostic result (The expression can be simplified).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0307 = new(
			DiagnosticIds.SD0307, Titles.SD0307, Messages.SD0307, Categories.Usage,
			DiagnosticSeverity.Info, true, helpLinkUri: HelpLinks.SD0307
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4042333&amp;doc_id=633030">
		/// SD0308
		/// </a>
		/// diagnostic result (The initializer contains the duplicate value).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0308 = new(
			DiagnosticIds.SD0308, Titles.SD0308, Messages.SD0308, Categories.Usage,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SD0308
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SD0305, SD0306, SD0307, SD0308
		);
	}
}
