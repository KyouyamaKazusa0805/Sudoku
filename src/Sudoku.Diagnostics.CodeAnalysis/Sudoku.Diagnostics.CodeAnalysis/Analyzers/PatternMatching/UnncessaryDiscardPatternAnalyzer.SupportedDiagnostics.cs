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
			DiagnosticIds.SS0607, Titles.SS0607, Messages.SS0607, Categories.SS0607,
			DiagnosticSeverities.SS0607, true, helpLinkUri: HelpLinks.SS0607
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4067865&amp;doc_id=633030">
		/// SS0613
		/// </a>
		/// diagnostic result (This discard can be omitted in the current positional pattern).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0613 = new(
			DiagnosticIds.SS0613, Titles.SS0613, Messages.SS0613, Categories.SS0613,
			DiagnosticSeverities.SS0613, true, helpLinkUri: HelpLinks.SS0613
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0607, SS0613
		);
	}
}
