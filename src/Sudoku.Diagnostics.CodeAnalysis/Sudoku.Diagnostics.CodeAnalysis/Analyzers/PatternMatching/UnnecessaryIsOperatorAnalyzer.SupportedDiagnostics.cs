using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class UnnecessaryIsOperatorAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4070575&amp;doc_id=633030">
		/// SS0617
		/// </a>
		/// diagnostic result (Unncessary <c>operator <see langword="is"/></c> to determine the range
		/// of that value of same type).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0617 = new(
			DiagnosticIds.SS0617, Titles.SS0617, Messages.SS0617, Categories.SS0617,
			DiagnosticSeverities.SS0617, true, helpLinkUri: HelpLinks.SS0617
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0617
		);
	}
}
