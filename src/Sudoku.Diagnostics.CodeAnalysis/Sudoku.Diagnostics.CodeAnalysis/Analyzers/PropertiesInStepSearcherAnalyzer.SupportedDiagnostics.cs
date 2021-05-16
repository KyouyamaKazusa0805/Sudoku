using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class PropertiesInStepSearcherAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599824&amp;doc_id=633030">
		/// SUDOKU001
		/// </a>
		/// diagnostic result (A property named 'Properties' expected).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku001 = new(
			DiagnosticIds.Sudoku001, Titles.Sudoku001, Messages.Sudoku001, Categories.StaticTechniqueProperties,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku001
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599808&amp;doc_id=633030">
		/// SUDOKU002
		/// </a>
		/// diagnostic result (The property 'Properties' must be <see langword="public"/>).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku002 = new(
			DiagnosticIds.Sudoku002, Titles.Sudoku002, Messages.Sudoku002, Categories.StaticTechniqueProperties,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku002
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3621783&amp;doc_id=633030">
		/// SUDOKU003
		/// </a>
		/// diagnostic result (The property 'Properties' must be <see langword="static"/>).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku003 = new(
			DiagnosticIds.Sudoku003, Titles.Sudoku003, Messages.Sudoku003, Categories.StaticTechniqueProperties,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku003
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599816&amp;doc_id=633030">
		/// SUDOKU004
		/// </a>
		/// diagnostic result (The property 'Properties' must be <see langword="readonly"/>).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku004 = new(
			DiagnosticIds.Sudoku004, Titles.Sudoku004, Messages.Sudoku004, Categories.StaticTechniqueProperties,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku004
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599818&amp;doc_id=633030">
		/// SUDOKU005
		/// </a>
		/// diagnostic result (The property 'Properties' has a wrong type).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku005 = new(
			DiagnosticIds.Sudoku005, Titles.Sudoku005, Messages.Sudoku005, Categories.StaticTechniqueProperties,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku005
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599826&amp;doc_id=633030">
		/// SUDOKU006
		/// </a>
		/// diagnostic result (The property 'Properties' can't be <see langword="null"/>).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku006 = new(
			DiagnosticIds.Sudoku006, Titles.Sudoku006, Messages.Sudoku006, Categories.StaticTechniqueProperties,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku006
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3602787&amp;doc_id=633030">
		/// SUDOKU007
		/// </a>
		/// diagnostic result (The property 'Properties' must contain an initializer).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku007 = new(
			DiagnosticIds.Sudoku007, Titles.Sudoku007, Messages.Sudoku007, Categories.StaticTechniqueProperties,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku007
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3607697&amp;doc_id=633030">
		/// SUDOKU008
		/// </a>
		/// diagnostic result (The property 'Properties' must be initialized by a new clause).
		/// </summary>
		private static readonly DiagnosticDescriptor Sudoku008 = new(
			DiagnosticIds.Sudoku008, Titles.Sudoku008, Messages.Sudoku008, Categories.StaticTechniqueProperties,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.Sudoku008
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			Sudoku001, Sudoku002, Sudoku003, Sudoku004, Sudoku005, Sudoku006, Sudoku007, Sudoku008
		);
	}
}
