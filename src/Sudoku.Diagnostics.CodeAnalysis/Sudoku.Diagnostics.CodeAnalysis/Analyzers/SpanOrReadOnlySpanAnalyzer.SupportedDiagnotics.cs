using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class SpanOrReadOnlySpanAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3622127&amp;doc_id=633030">
		/// SUDOKU017
		/// </a>
		/// diagnostic result (The result of the expression '.ctor(<see langword="void"/>*, <see cref="int"/>)'
		/// can't be the return value as any methods).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku017 = new(
			DiagnosticIds.Sudoku017, Titles.Sudoku017, Messages.Sudoku017, Categories.Performance,
			DiagnosticSeverity.Warning, true, helpLinkUri: HelpLinks.Sudoku017
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			Sudoku017
		);
	}
}
