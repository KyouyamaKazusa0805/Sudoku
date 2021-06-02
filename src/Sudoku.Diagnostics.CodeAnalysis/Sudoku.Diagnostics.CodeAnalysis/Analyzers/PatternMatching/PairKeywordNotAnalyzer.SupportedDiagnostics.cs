using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class PairKeywordNotAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4079250&amp;doc_id=633030">
		/// SS0626
		/// </a>
		/// diagnostic result (Keyword <see langword="not"/> repeats; A pair of keyword <see langword="not"/>
		/// can be omitted).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0626 = new(
			DiagnosticIds.SS0626, Titles.SS0626, Messages.SS0626, Categories.Design,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SS0626
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0626
		);
	}
}
