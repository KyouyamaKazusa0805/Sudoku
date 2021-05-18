using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class DynamicallyInvocationOfCurrentAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3610020&amp;doc_id=633030">
		/// SD0202
		/// </a>
		/// diagnostic result (The specified method can't be found and called).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0202 = new(
			DiagnosticIds.SD0202, Titles.SD0202, Messages.SD0202, Categories.Usage,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.SD0202
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3610022&amp;doc_id=633030">
		/// SD0203
		/// </a>
		/// diagnostic result (The number of arguments dismatched in this dynamically invocation).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0203 = new(
			DiagnosticIds.SD0203, Titles.SD0203, Messages.SD0203, Categories.ResourceDictionary,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.SD0203
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3610347&amp;doc_id=633030">
		/// SD0204
		/// </a>
		/// diagnostic result (The argument type dismatched in this dynamically invocation).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0204 = new(
			DiagnosticIds.SD0204, Titles.SD0204, Messages.SD0204, Categories.Usage,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.SD0204
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3610364&amp;doc_id=633030">
		/// SD0205
		/// </a>
		/// diagnostic result (The method returns void, but you make it an rvalue expression).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0205 = new(
			DiagnosticIds.SD0205, Titles.SD0205, Messages.SD0205, Categories.Usage,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.SD0205
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4018391&amp;doc_id=633030">
		/// SD0206
		/// </a>
		/// diagnostic result (The specified key can't be found in the resource dictionary).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0206 = new(
			DiagnosticIds.SD0206, Titles.SD0206, Messages.SD0206, Categories.ResourceDictionary,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.SD0206
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SD0202, SD0203, SD0204, SD0205, SD0206
		);
	}
}
