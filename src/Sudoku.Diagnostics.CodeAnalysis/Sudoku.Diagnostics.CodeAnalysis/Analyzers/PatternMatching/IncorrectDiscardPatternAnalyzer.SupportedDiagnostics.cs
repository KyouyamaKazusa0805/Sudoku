using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class IncorrectDiscardPatternAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4066334&amp;doc_id=633030">
		/// SS0608
		/// </a>
		/// diagnostic result (The positional pattern may not allow because bounded deconstruction method is
		/// a parameterless or single-parameter one).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0608 = new(
			DiagnosticIds.SS0608, Titles.SS0608, Messages.SS0608, Categories.SS0608,
			DiagnosticSeverities.SS0608, true, helpLinkUri: HelpLinks.SS0608
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0608
		);
	}
}
