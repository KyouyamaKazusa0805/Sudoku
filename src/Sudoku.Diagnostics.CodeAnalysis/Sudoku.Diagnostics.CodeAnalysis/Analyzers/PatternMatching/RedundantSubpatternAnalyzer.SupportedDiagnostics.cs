using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class RedundantSubpatternAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4067681&amp;doc_id=633030">
		/// SS0609
		/// </a>
		/// diagnostic result (The positional subpattern is redundant).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0609 = new(
			DiagnosticIds.SS0609, Titles.SS0609, Messages.SS0609, Categories.SS0609,
			DiagnosticSeverities.SS0609, true, helpLinkUri: HelpLinks.SS0609
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0609
		);
	}
}
