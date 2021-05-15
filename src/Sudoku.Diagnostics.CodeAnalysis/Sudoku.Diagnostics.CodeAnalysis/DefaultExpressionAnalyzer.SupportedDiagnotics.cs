using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	partial class DefaultExpressionAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3630107&amp;doc_id=633030">
		/// SUDOKU021
		/// </a>
		/// diagnostic result (Please use the field <c>Empty</c> or <c>Undefined</c> to avoid instantiation).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku021 = new(
			DiagnosticIds.Sudoku021, Titles.Sudoku021, Messages.Sudoku021, Categories.Performance,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.Sudoku021
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			Sudoku021
		);
	}
}
