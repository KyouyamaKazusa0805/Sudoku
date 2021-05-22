using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class ArrayCreationClauseAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4047679&amp;doc_id=633030">
		/// SS9002
		/// </a>
		/// diagnostic result (Redundant array creation statement).
		/// </summary>
		private static readonly DiagnosticDescriptor SS9002 = new(
			DiagnosticIds.SS9002, Titles.SS9002, Messages.SS9002, Categories.Usage,
			DiagnosticSeverity.Info, true, helpLinkUri: HelpLinks.SS9002
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS9002
		);
	}
}
