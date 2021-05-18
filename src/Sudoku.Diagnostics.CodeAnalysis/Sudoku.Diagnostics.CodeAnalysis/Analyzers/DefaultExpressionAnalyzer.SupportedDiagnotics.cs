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
			DiagnosticIds.SD0303, Titles.SD0303, Messages.SD0303, Categories.Performance,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SD0303
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SD0303
		);
	}
}
