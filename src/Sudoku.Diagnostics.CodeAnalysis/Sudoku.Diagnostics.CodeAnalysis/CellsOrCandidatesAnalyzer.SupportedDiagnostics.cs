using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	partial class CellsOrCandidatesAnalyzer
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

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3630107&amp;doc_id=633030">
		/// SUDOKU021
		/// </a>
		/// diagnostic result (Please use the field <c>Empty</c> to avoid instantiation).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku021 = new(
			DiagnosticIds.Sudoku021, Titles.Sudoku021, Messages.Sudoku021, Categories.Performance,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.Sudoku021
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			Sudoku018, Sudoku021
		);
	}
}
