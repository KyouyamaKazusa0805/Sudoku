using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class DynamicallyInvocationOfCurrentAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3610020&amp;doc_id=633030">
		/// SUDOKU010
		/// </a>
		/// diagnostic result (The specified method can't be found and called).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku010 = new(
			DiagnosticIds.Sudoku010, Titles.Sudoku010, Messages.Sudoku010, Categories.Usage,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku010
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3610022&amp;doc_id=633030">
		/// SUDOKU011
		/// </a>
		/// diagnostic result (The number of arguments dismatched in this dynamically invocation).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku011 = new(
			DiagnosticIds.Sudoku011, Titles.Sudoku011, Messages.Sudoku011, Categories.ResourceDictionary,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku011
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3610347&amp;doc_id=633030">
		/// SUDOKU012
		/// </a>
		/// diagnostic result (The argument type dismatched in this dynamically invocation).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku012 = new(
			DiagnosticIds.Sudoku012, Titles.Sudoku012, Messages.Sudoku012, Categories.Usage,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku012
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3610364&amp;doc_id=633030">
		/// SUDOKU013
		/// </a>
		/// diagnostic result (The method returns void, but you make it an rvalue expression).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku013 = new(
			DiagnosticIds.Sudoku013, Titles.Sudoku013, Messages.Sudoku013, Categories.Usage,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku013
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4018391&amp;doc_id=633030">
		/// SUDOKU015
		/// </a>
		/// diagnostic result (The specified key can't be found in the resource dictionary).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku015 = new(
			DiagnosticIds.Sudoku015, Titles.Sudoku015, Messages.Sudoku015, Categories.ResourceDictionary,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku015
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			Sudoku010, Sudoku011, Sudoku012, Sudoku013, Sudoku015
		);
	}
}
