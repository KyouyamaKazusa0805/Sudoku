using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class SpanOrReadOnlySpanAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3622127&amp;doc_id=633030">
		/// SS0102
		/// </a>
		/// diagnostic result (The result of the expression '.ctor(<see langword="void"/>*, <see cref="int"/>)'
		/// can't be the return value as any methods).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0102 = new(
			DiagnosticIds.SS0102, Titles.SS0102, Messages.SS0102, Categories.Performance,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SS0102
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0102
		);
	}
}
