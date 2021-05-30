using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class DiscardInVarPatternAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4068043&amp;doc_id=633030">
		/// SS0611
		/// </a>
		/// diagnostic result (The discard <see langword="_"/> in the <see langword="var"/> pattern
		/// may not take any effects).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0611 = new(
			DiagnosticIds.SS0611, Titles.SS0611, Messages.SS0611, Categories.Design,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SS0611
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0611
		);
	}
}
