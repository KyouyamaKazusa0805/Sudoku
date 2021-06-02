using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class AvailablePositionalPatternAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4052320&amp;doc_id=633030">
		/// SS0606
		/// </a>
		/// diagnostic result (The expression can be simplifiy via using positional pattern matching).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0606 = new(
			DiagnosticIds.SS0606, Titles.SS0606, Messages.SS0606, Categories.SS0606,
			DiagnosticSeverities.SS0606, true, helpLinkUri: HelpLinks.SS0606
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0606
		);
	}
}
