using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	partial class CellsOrCandidatesCountIsEqualToZeroAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3625575&amp;doc_id=633030">
		/// SUDOKU018
		/// </a>
		/// diagnostic result (Replace <c>Count == 0</c> with <c>IsEmpty</c>).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku018 = new(
			DiagnosticIds.Sudoku018, Titles.Sudoku018, Messages.Sudoku018, Categories.Usage,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.Sudoku018
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			Sudoku018
		);
	}
}
