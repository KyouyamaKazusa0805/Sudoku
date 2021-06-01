using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class RecordToStringAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4052320&amp;doc_id=633030">
		/// SS9004
		/// </a>
		/// diagnostic result (Due to recursive member in the <see langword="record"/> type,
		/// invoking synthesized method ToString will cause stack overflowing).
		/// </summary>
		private static readonly DiagnosticDescriptor SS9004 = new(
			DiagnosticIds.SS9004, Titles.SS9004, Messages.SS9004, Categories.Design,
			DiagnosticSeverity.Error, true, helpLinkUri: HelpLinks.SS9004
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS9004
		);
	}
}
