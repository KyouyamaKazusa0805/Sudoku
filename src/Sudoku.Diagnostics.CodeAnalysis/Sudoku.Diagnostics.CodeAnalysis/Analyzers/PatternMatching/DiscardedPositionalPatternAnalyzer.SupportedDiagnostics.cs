using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class DiscardedPositionalPatternAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4067685&amp;doc_id=633030">
		/// SS0610
		/// </a>
		/// diagnostic result (The positional pattern judges nothing. If you want to judge whether the
		/// object is not <see langword="null"/>, please use empty property pattern <c>{ }</c>
		/// or <c><see langword="not null"/></c> instead).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0610 = new(
			DiagnosticIds.SS0610, Titles.SS0610, Messages.SS0610, Categories.SS0610,
			DiagnosticSeverities.SS0610, true, helpLinkUri: HelpLinks.SS0610
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0610
		);
	}
}
