using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class LinqAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3625073&amp;doc_id=633030">
		/// SS0301
		/// </a>
		/// diagnostic result (Replace <c>Count() >= n</c> with <c>Take(n).Count() >= n</c>).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0301 = new(
			DiagnosticIds.SS0301, Titles.SS0301, Messages.SS0301, Categories.SS0301,
			DiagnosticSeverities.SS0301, true, helpLinkUri: HelpLinks.SS0301
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0301
		);
	}
}
