using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class CountIsEqualToZeroAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3625575&amp;doc_id=633030">
		/// SD0302
		/// </a>
		/// diagnostic result (Replace <c>Count == 0</c> with <c>IsEmpty</c>).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0302 = new(
			DiagnosticIds.SD0302, Titles.SD0302, Messages.SD0302, Categories.SD0302,
			DiagnosticSeverities.SD0302, true, helpLinkUri: HelpLinks.SD0302
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SD0302
		);
	}
}
