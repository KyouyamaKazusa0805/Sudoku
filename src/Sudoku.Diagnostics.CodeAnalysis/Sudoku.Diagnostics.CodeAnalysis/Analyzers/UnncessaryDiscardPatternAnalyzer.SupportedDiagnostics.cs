using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class UnncessaryDiscardPatternAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4060446&amp;doc_id=633030">
		/// SS0607
		/// </a>
		/// diagnostic result (This discard can be omitted in the current positional pattern).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0607 = new(
			DiagnosticIds.SS0607, Titles.SS0607, Messages.SS0607, Categories.Design,
			DiagnosticSeverity.Info, true, helpLinkUri: HelpLinks.SS0607
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0607
		);
	}
}
