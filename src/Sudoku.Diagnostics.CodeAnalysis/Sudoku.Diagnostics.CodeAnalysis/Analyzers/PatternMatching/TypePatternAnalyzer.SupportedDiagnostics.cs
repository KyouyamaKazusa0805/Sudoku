using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class TypePatternAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4049130&amp;doc_id=633030">
		/// SS0601
		/// </a>
		/// diagnostic result (Unnecessary explicit type pattern).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0601 = new(
			DiagnosticIds.SS0601, Titles.SS0601, Messages.SS0601, Categories.SS0601,
			DiagnosticSeverities.SS0601, true, helpLinkUri: HelpLinks.SS0601
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4049132&amp;doc_id=633030">
		/// SS0602
		/// </a>
		/// diagnostic result (The pattern can be simplify to <c><see langword="is null"/></c>).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0602 = new(
			DiagnosticIds.SS0602, Titles.SS0602, Messages.SS0602, Categories.SS0602,
			DiagnosticSeverities.SS0602, true, helpLinkUri: HelpLinks.SS0602
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4049133&amp;doc_id=633030">
		/// SS0603
		/// </a>
		/// diagnostic result (The pattern can be simplify to <c><see langword="is not null"/></c>).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0603 = new(
			DiagnosticIds.SS0603, Titles.SS0603, Messages.SS0603, Categories.SS0603,
			DiagnosticSeverities.SS0603, true, helpLinkUri: HelpLinks.SS0603
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0601, SS0602, SS0603
		);
	}
}
