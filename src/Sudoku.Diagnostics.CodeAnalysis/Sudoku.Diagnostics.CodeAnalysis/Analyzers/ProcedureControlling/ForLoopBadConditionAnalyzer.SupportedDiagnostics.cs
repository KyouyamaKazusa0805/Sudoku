using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class ForLoopBadConditionAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4042356&amp;doc_id=633030">
		/// SS9001
		/// </a>
		/// diagnostic result (Available prepositional iteration condition expression).
		/// </summary>
		private static readonly DiagnosticDescriptor SS9001 = new(
			DiagnosticIds.SS9001, Titles.SS9001, Messages.SS9001, Categories.SS9001,
			DiagnosticSeverities.SS9001, true, helpLinkUri: HelpLinks.SS9001
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS9001
		);
	}
}
