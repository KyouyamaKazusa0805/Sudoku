using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class UnnecessaryNotInRelationPatternAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4070582&amp;doc_id=633030">
		/// SS0620
		/// </a>
		/// diagnostic result (Keyword <see langword="not"/> followed with relation pattern is redundant;
		/// please negate the operator directly instead).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0620 = new(
			DiagnosticIds.SS0620, Titles.SS0620, Messages.SS0620, Categories.Design,
			DiagnosticSeverity.Info, true, helpLinkUri: HelpLinks.SS0620
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0620
		);
	}
}
