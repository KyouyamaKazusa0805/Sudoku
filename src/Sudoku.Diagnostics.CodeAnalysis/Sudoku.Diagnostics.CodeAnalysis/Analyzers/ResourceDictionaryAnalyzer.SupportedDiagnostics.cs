using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class ResourceDictionaryAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3608009&amp;doc_id=633030">
		/// SUDOKU009
		/// </a>
		/// diagnostic result (The specified key can't be found in the resource dictionary).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku009 = new(
			DiagnosticIds.Sudoku009, Titles.Sudoku009, Messages.Sudoku009, Categories.ResourceDictionary,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku009
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			Sudoku009
		);
	}
}
