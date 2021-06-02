using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class DiscardAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4067861&amp;doc_id=633030">
		/// SS9003
		/// </a>
		/// diagnostic result (Unnecessary discards).
		/// </summary>
		private static readonly DiagnosticDescriptor SS9003 = new(
			DiagnosticIds.SS9003, Titles.SS9003, Messages.SS9003, Categories.SS9003,
			DiagnosticSeverities.SS9003, true, helpLinkUri: HelpLinks.SS9003
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS9003
		);
	}
}
