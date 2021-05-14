using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	partial class LinqAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3625073&amp;doc_id=633030">
		/// SUDOKU019
		/// </a>
		/// diagnostic result (Replace <c>Count() >= n</c> with <c>Take(n).Count() >= n</c>).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku019 = new(
			DiagnosticIds.Sudoku019, Titles.Sudoku019, Messages.Sudoku019, Categories.Performance,
			DiagnosticSeverity.Info, true, helpLinkUri: HelpLinks.Sudoku019
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			Sudoku019
		);
	}
}
