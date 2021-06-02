using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class NotNullAndAndNullOrAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4070577&amp;doc_id=633030">
		/// SS0618
		/// </a>
		/// diagnostic result (Pattern <c><see langword="not null"/></c> is redundant; please remove it).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0618 = new(
			DiagnosticIds.SS0618, Titles.SS0618, Messages.SS0618, Categories.SS0618,
			DiagnosticSeverities.SS0618, true, helpLinkUri: HelpLinks.SS0618
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4070578&amp;doc_id=633030">
		/// SS0619
		/// </a>
		/// diagnostic result (Combination constant pattern <c><see langword="null"/></c> and
		/// the keyword <see langword="or"/> is redundant; please remove it).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0619 = new(
			DiagnosticIds.SS0619, Titles.SS0619, Messages.SS0619, Categories.SS0619,
			DiagnosticSeverities.SS0619, true, helpLinkUri: HelpLinks.SS0619
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0618, SS0619
		);
	}
}
