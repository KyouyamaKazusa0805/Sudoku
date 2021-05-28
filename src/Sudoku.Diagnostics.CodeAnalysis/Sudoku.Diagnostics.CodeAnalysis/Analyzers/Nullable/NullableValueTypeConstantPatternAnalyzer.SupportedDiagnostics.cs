using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class NullableValueTypeConstantPatternAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4049882&amp;doc_id=633030">
		/// SS0605
		/// </a>
		/// diagnostic result (The nullable value type checking expression
		/// will be suggested to convert to constant pattern).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0605 = new(
			DiagnosticIds.SS0605, Titles.SS0605, Messages.SS0605, Categories.Design,
			DiagnosticSeverity.Info, true, helpLinkUri: HelpLinks.SS0605
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0605
		);
	}
}
