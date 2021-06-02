using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class NullCoalescingAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4059064&amp;doc_id=633030">
		/// SS0702
		/// </a>
		/// diagnostic result (The expression can be simplified to using null-coalescing expression <c>??</c>).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0702 = new(
			DiagnosticIds.SS0702, Titles.SS0702, Messages.SS0702, Categories.SS0702,
			DiagnosticSeverities.SS0702, true, helpLinkUri: HelpLinks.SS0702
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0702
		);
	}
}
