using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class DeconstructionMethodAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025282&amp;doc_id=633030">
		/// SS0501
		/// </a>
		/// diagnostic result (Deconstruction methods should contain at least 2 parameters).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0501 = new(
			DiagnosticIds.SS0501, Titles.SS0501, Messages.SS0501, Categories.Design,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SS0501
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025301&amp;doc_id=633030">
		/// SS0502
		/// </a>
		/// diagnostic result (Deconstruction methods must be instance ones).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0502 = new(
			DiagnosticIds.SS0502, Titles.SS0502, Messages.SS0502, Categories.Design,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SS0502
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025305&amp;doc_id=633030">
		/// SS0503
		/// </a>
		/// diagnostic result (Deconstruction methods must return void).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0503 = new(
			DiagnosticIds.SS0503, Titles.SS0503, Messages.SS0503, Categories.Design,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SS0503
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025793&amp;doc_id=633030">
		/// SS0504
		/// </a>
		/// diagnostic result (Deconstruction methods must be public).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0504 = new(
			DiagnosticIds.SS0504, Titles.SS0504, Messages.SS0504, Categories.Design,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SS0504
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025794&amp;doc_id=633030">
		/// SS0505
		/// </a>
		/// diagnostic result (All parameters in deconstruction methods should be out parameters).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0505 = new(
			DiagnosticIds.SS0505, Titles.SS0505, Messages.SS0505, Categories.Design,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SS0505
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0501, SS0502, SS0503, SS0504, SS0505
		);
	}
}
