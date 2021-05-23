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
			DiagnosticIds.SS0601, Titles.SS0601, Messages.SS0601, Categories.Design,
			DiagnosticSeverity.Info, true, helpLinkUri: HelpLinks.SS0601
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0601
		);
	}
}
