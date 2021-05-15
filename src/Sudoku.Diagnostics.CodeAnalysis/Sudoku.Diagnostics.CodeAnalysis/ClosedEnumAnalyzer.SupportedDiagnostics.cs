using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	partial class ClosedEnumAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4019214&amp;doc_id=633030">
		/// SUDOKU022
		/// </a>
		/// diagnostic result (Can't apply the operator here because the type is closed enum).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku022 = new(
			DiagnosticIds.Sudoku022, Titles.Sudoku022, Messages.Sudoku022, Categories.Usage,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku022
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			Sudoku022
		);
	}
}
