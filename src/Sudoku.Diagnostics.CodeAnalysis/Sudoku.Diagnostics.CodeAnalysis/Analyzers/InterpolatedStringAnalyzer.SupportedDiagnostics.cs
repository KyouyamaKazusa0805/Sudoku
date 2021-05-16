using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class InterpolatedStringAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3622115&amp;doc_id=633030">
		/// SUDOKU016
		/// </a>
		/// diagnostic result (Please add 'ToString' method invocation to the interpolation part
		/// in order to prevent any box and unbox operations).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku016 = new(
			DiagnosticIds.Sudoku016, Titles.Sudoku016, Messages.Sudoku016, Categories.Performance,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.Sudoku016
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3629641&amp;doc_id=633030">
		/// SUDOKU020
		/// </a>
		/// diagnostic result (Unnecessary interpolation leading character <c>'$'</c>).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku020 = new(
			DiagnosticIds.Sudoku020, Titles.Sudoku020, Messages.Sudoku020, Categories.Usage,
			DiagnosticSeverity.Info, true, helpLinkUri: HelpLinks.Sudoku020
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			Sudoku016, Sudoku020
		);
	}
}
