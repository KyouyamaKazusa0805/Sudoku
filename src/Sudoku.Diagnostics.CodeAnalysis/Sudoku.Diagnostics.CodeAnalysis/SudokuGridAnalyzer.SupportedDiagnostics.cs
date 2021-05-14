using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	partial class SudokuGridAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3614979&amp;doc_id=633030">
		/// SUDOKU014
		/// </a>
		/// diagnostic result (The member can't be invoked because they are reserved).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku014 = new(
			DiagnosticIds.Sudoku014, Titles.Sudoku014, Messages.Sudoku014, Categories.Usage,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku014
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			Sudoku014
		);
	}
}
