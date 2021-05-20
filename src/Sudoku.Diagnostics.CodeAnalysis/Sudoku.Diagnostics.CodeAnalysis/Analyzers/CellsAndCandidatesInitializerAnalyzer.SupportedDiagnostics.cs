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


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SD0305
		);
	}
}
