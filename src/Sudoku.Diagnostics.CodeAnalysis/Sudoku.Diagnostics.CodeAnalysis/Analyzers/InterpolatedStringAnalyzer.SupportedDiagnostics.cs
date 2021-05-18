using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class InterpolatedStringAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3622115&amp;doc_id=633030">
		/// SS0101
		/// </a>
		/// diagnostic result (Please add 'ToString' method invocation to the interpolation part
		/// in order to prevent any box and unbox operations).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0101 = new(
			DiagnosticIds.SS0101, Titles.SS0101, Messages.SS0101, Categories.Performance,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SS0101
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3629641&amp;doc_id=633030">
		/// SS0102
		/// </a>
		/// diagnostic result (Unnecessary interpolation leading character <c>'$'</c>).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0102 = new(
			DiagnosticIds.SS0102, Titles.SS0102, Messages.SS0102, Categories.Usage,
			DiagnosticSeverity.Info, true, helpLinkUri: HelpLinks.SS0102
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0101, SS0102
		);
	}
}
