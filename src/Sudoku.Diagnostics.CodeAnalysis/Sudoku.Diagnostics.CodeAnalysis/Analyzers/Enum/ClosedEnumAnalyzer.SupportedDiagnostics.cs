using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class ClosedEnumAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4019214&amp;doc_id=633030">
		/// SS0401
		/// </a>
		/// diagnostic result (Can't apply the operator here because the type is closed enum).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0401 = new(
			DiagnosticIds.SS0401, Titles.SS0401, Messages.SS0401, Categories.SS0401,
			DiagnosticSeverities.SS0401, true, helpLinkUri: HelpLinks.SS0401
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0401
		);
	}
}
