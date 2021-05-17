using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class AutoAttributePropertiesAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4021986&amp;doc_id=633030">
		/// SUDOKU023
		/// </a>
		/// diagnostic result (Please use nameof expression instead of string literal).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku023 = new(
			DiagnosticIds.Sudoku023, Titles.Sudoku023, Messages.Sudoku023, Categories.Usage,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.Sudoku023
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4024928&amp;doc_id=633030">
		/// SUDOKU024
		/// </a>
		/// diagnostic result (This attribute must contain the specified number of parameters).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku024 = new(
			DiagnosticIds.Sudoku024, Titles.Sudoku024, Messages.Sudoku024, Categories.Usage,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.Sudoku024
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			Sudoku023, Sudoku024
		);
	}
}
