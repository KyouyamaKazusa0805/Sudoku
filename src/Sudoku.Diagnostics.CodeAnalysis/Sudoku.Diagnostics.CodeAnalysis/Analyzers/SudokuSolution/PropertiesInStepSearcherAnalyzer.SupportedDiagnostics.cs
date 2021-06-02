using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class PropertiesInStepSearcherAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599824&amp;doc_id=633030">
		/// SD0101
		/// </a>
		/// diagnostic result (A property named <c>Properties</c> expected).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0101 = new(
			DiagnosticIds.SD0101, Titles.SD0101, Messages.SD0101, Categories.SD0101,
			DiagnosticSeverities.SD0101, true, helpLinkUri: HelpLinks.SD0101
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599808&amp;doc_id=633030">
		/// SD0102
		/// </a>
		/// diagnostic result (The property <c>Properties</c> must be <see langword="public"/>).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0102 = new(
			DiagnosticIds.SD0102, Titles.SD0102, Messages.SD0102, Categories.SD0102,
			DiagnosticSeverities.SD0102, true, helpLinkUri: HelpLinks.SD0102
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3621783&amp;doc_id=633030">
		/// SD0103
		/// </a>
		/// diagnostic result (The property <c>Properties</c> must be <see langword="static"/>).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0103 = new(
			DiagnosticIds.SD0103, Titles.SD0103, Messages.SD0103, Categories.SD0103,
			DiagnosticSeverities.SD0103, true, helpLinkUri: HelpLinks.SD0103
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599816&amp;doc_id=633030">
		/// SD0104
		/// </a>
		/// diagnostic result (The property <c>Properties</c> must be <see langword="readonly"/>).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0104 = new(
			DiagnosticIds.SD0104, Titles.SD0104, Messages.SD0104, Categories.SD0104,
			DiagnosticSeverities.SD0104, true, helpLinkUri: HelpLinks.SD0104
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599818&amp;doc_id=633030">
		/// SD0105
		/// </a>
		/// diagnostic result (The property <c>Properties</c> has a wrong type).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0105 = new(
			DiagnosticIds.SD0105, Titles.SD0105, Messages.SD0105, Categories.SD0105,
			DiagnosticSeverities.SD0105, true, helpLinkUri: HelpLinks.SD0105
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599826&amp;doc_id=633030">
		/// SD0106
		/// </a>
		/// diagnostic result (The property <c>Properties</c> can't be <see langword="null"/>).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0106 = new(
			DiagnosticIds.SD0106, Titles.SD0106, Messages.SD0106, Categories.SD0106,
			DiagnosticSeverities.SD0106, true, helpLinkUri: HelpLinks.SD0106
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3602787&amp;doc_id=633030">
		/// SD0107
		/// </a>
		/// diagnostic result (The property <c>Properties</c> must contain an initializer).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0107 = new(
			DiagnosticIds.SD0107, Titles.SD0107, Messages.SD0107, Categories.SD0107,
			DiagnosticSeverities.SD0107, true, helpLinkUri: HelpLinks.SD0107
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3607697&amp;doc_id=633030">
		/// SD0108
		/// </a>
		/// diagnostic result (The property <c>Properties</c> must be initialized by a new clause).
		/// </summary>
		private static readonly DiagnosticDescriptor SD0108 = new(
			DiagnosticIds.SD0108, Titles.SD0108, Messages.SD0108, Categories.SD0108,
			DiagnosticSeverities.SD0108, true, helpLinkUri: HelpLinks.SD0108
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SD0101, SD0102, SD0103, SD0104, SD0105, SD0106, SD0107, SD0108
		);
	}
}
