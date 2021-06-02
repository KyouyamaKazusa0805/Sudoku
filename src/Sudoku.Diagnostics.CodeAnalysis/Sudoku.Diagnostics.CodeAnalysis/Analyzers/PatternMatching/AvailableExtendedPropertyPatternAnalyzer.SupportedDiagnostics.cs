using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class AvailableExtendedPropertyPatternAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4071684&amp;doc_id=633030">
		/// SS0624
		/// </a>
		/// diagnostic result (Available simplification for extended property patterns).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0624 = new(
			DiagnosticIds.SS0624, Titles.SS0624, Messages.SS0624, Categories.SS0624,
			DiagnosticSeverities.SS0624, true, helpLinkUri: HelpLinks.SS0624
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0624
		);
	}
}
