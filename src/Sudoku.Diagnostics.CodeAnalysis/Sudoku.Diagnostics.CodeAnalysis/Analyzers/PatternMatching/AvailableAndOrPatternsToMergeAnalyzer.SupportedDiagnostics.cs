using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class AvailableAndOrPatternsToMergeAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4070766&amp;doc_id=633030">
		/// SS0621
		/// </a>
		/// diagnostic result (Available simplification for property patterns connected
		/// with keyword <see langword="and"/> to a single property pattern).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0621 = new(
			DiagnosticIds.SS0621, Titles.SS0621, Messages.SS0621, Categories.SS0621,
			DiagnosticSeverities.SS0621, true, helpLinkUri: HelpLinks.SS0621
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4070772&amp;doc_id=633030">
		/// SS0622
		/// </a>
		/// diagnostic result (Available simplification for property patterns connected
		/// with keyword <see langword="or"/> to a single property pattern).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0622 = new(
			DiagnosticIds.SS0622, Titles.SS0622, Messages.SS0622, Categories.SS0622,
			DiagnosticSeverities.SS0622, true, helpLinkUri: HelpLinks.SS0622
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0621, SS0622
		);
	}
}
