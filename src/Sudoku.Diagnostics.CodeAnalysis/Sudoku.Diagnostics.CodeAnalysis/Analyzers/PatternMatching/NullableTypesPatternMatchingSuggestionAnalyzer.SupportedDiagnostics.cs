using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class NullableTypesPatternMatchingSuggestionAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4068113&amp;doc_id=633030">
		/// SS0615
		/// </a>
		/// diagnostic result (Please use <c>{ }</c> instead of <c>!= <see langword="null"/></c>,
		/// <c><see langword="not null"/></c> or <c>HasValue</c> in nullable value types).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0615 = new(
			DiagnosticIds.SS0615, Titles.SS0615, Messages.SS0615, Categories.SS0615,
			DiagnosticSeverities.SS0615, true, helpLinkUri: HelpLinks.SS0615
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4068091&amp;doc_id=633030">
		/// SS0616
		/// </a>
		/// diagnostic result (Please use <c>{ }</c> or <c><see langword="not null"/></c> instead of
		/// <c>!= <see langword="null"/></c> in nullable reference types).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0616 = new(
			DiagnosticIds.SS0616, Titles.SS0616, Messages.SS0616, Categories.SS0616,
			DiagnosticSeverities.SS0616, true, helpLinkUri: HelpLinks.SS0616
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0615, SS0616
		);
	}
}
