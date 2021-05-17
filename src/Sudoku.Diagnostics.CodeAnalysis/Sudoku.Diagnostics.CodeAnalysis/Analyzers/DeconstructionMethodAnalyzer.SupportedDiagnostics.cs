using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class DeconstructionMethodAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025282&amp;doc_id=633030">
		/// SUDOKU025
		/// </a>
		/// diagnostic result (Deconstruction methods should contain at least 2 parameters).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku025 = new(
			DiagnosticIds.Sudoku025, Titles.Sudoku025, Messages.Sudoku025, Categories.Design,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.Sudoku025
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025301&amp;doc_id=633030">
		/// SUDOKU026
		/// </a>
		/// diagnostic result (Deconstruction methods must be instance ones).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku026 = new(
			DiagnosticIds.Sudoku026, Titles.Sudoku026, Messages.Sudoku026, Categories.Design,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.Sudoku026
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025305&amp;doc_id=633030">
		/// SUDOKU027
		/// </a>
		/// diagnostic result (Deconstruction methods must return void).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku027 = new(
			DiagnosticIds.Sudoku027, Titles.Sudoku027, Messages.Sudoku027, Categories.Design,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.Sudoku027
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025793&amp;doc_id=633030">
		/// SUDOKU028
		/// </a>
		/// diagnostic result (Deconstruction methods must be public).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku028 = new(
			DiagnosticIds.Sudoku028, Titles.Sudoku028, Messages.Sudoku028, Categories.Design,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.Sudoku028
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025794&amp;doc_id=633030">
		/// SUDOKU029
		/// </a>
		/// diagnostic result (All parameters in deconstruction methods should be out parameters).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku029 = new(
			DiagnosticIds.Sudoku029, Titles.Sudoku029, Messages.Sudoku029, Categories.Design,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.Sudoku029
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			Sudoku025, Sudoku026, Sudoku027, Sudoku028, Sudoku029
		);
	}
}
