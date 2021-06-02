using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class DiscardInVarPatternAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4068043&amp;doc_id=633030">
		/// SS0611
		/// </a>
		/// diagnostic result (The discard <see langword="_"/> in the <see langword="var"/> pattern
		/// may not take any effects).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0611 = new(
			DiagnosticIds.SS0611, Titles.SS0611, Messages.SS0611, Categories.SS0611,
			DiagnosticSeverities.SS0611, true, helpLinkUri: HelpLinks.SS0611
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4068075&amp;doc_id=633030">
		/// SS0612
		/// </a>
		/// diagnostic result (It doesn't take any effects that all subpatterns use
		/// discards <see langword="_"/> in a <see langword="var"/> deconstruction pattern).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0612 = new(
			DiagnosticIds.SS0612, Titles.SS0612, Messages.SS0612, Categories.SS0612,
			DiagnosticSeverities.SS0612, true, helpLinkUri: HelpLinks.SS0612
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0611, SS0612
		);
	}
}
