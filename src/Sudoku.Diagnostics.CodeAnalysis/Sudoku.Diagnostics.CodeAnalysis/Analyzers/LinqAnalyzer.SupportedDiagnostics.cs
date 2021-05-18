using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class LinqAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3625073&amp;doc_id=633030">
		/// SS0201
		/// </a>
		/// diagnostic result (Replace <c>Count() >= n</c> with <c>Take(n).Count() >= n</c>).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0201 = new(
			DiagnosticIds.SS0201, Titles.SS0201, Messages.SS0201, Categories.Performance,
			DiagnosticSeverity.Info, true, helpLinkUri: HelpLinks.SS0201
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0201
		);
	}
}
