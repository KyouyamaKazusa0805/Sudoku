using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class UnncessaryEmptyBracePatternAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4068077&amp;doc_id=633030">
		/// SS0614
		/// </a>
		/// diagnostic result (The empty-brace property pattern <see langword="{ }"/> will take effects
		/// only in nullable types, so we don't suggest you use the pattern to judge non-nullable types;
		/// please change the clause to var pattern or other valid patterns, or just remove it).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0614 = new(
			DiagnosticIds.SS0614, Titles.SS0614, Messages.SS0614, Categories.SS0614,
			DiagnosticSeverities.SS0614, true, helpLinkUri: HelpLinks.SS0614
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0614
		);
	}
}
