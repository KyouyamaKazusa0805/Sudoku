using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class BuiltInTypesConstantPatternAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4049877&amp;doc_id=633030">
		/// SS0604
		/// </a>
		/// diagnostic result (Unnecessary constant pattern for the variable of the same type).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0604 = new(
			DiagnosticIds.SS0604, Titles.SS0604, Messages.SS0604, Categories.SS0604,
			DiagnosticSeverities.SS0604, true, helpLinkUri: HelpLinks.SS0604
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0604
		);
	}
}
