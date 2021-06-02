using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class SpanOrReadOnlySpanAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3622127&amp;doc_id=633030">
		/// SS0201
		/// </a>
		/// diagnostic result (The result of the expression '.ctor(<see langword="void"/>*, <see cref="int"/>)'
		/// can't be the return value as any methods).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0201 = new(
			DiagnosticIds.SS0201, Titles.SS0201, Messages.SS0201, Categories.Performance,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.SS0201
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0201
		);
	}
}
