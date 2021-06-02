using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class DefaultExpressionAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3630107&amp;doc_id=633030">
		/// SD0303
		/// </a>
		/// diagnostic result (Please use the field <c>Empty</c> or <c>Undefined</c> to avoid instantiation).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0303 = new(
			DiagnosticIds.SD0303, Titles.SD0303, Messages.SD0303, Categories.SD0303,
			DiagnosticSeverities.SD0303, true, helpLinkUri: HelpLinks.SD0303
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4035455&amp;doc_id=633030">
		/// SD0304
		/// </a>
		/// diagnostic result (Please use the specific property to simplify the invocation).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0304 = new(
			DiagnosticIds.SD0304, Titles.SD0304, Messages.SD0304, Categories.SD0304,
			DiagnosticSeverities.SD0304, true, helpLinkUri: HelpLinks.SD0304
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SD0303, SD0304
		);
	}
}
