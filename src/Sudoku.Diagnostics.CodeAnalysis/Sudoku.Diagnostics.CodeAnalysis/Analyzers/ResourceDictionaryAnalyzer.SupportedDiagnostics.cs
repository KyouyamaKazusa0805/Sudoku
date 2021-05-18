using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class ResourceDictionaryAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3608009&amp;doc_id=633030">
		/// SD0201
		/// </a>
		/// diagnostic result (The specified key can't be found in the resource dictionary).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0201 = new(
			DiagnosticIds.SD0201, Titles.SD0201, Messages.SD0201, Categories.ResourceDictionary,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.SD0201
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SD0201
		);
	}
}
