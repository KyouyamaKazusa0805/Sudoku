using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class FlaggedEnumFieldAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4038390&amp;doc_id=633030">
		/// SS0402
		/// </a>
		/// diagnostic result (The enum field must holds a flag value if the enum
		/// is marked <see cref="FlagsAttribute"/>).
		/// </summary>
		/// <seealso cref="FlagsAttribute"/>
		private static readonly DiagnosticDescriptor SS0402 = new(
			DiagnosticIds.SS0402, Titles.SS0402, Messages.SS0402, Categories.SS0402,
			DiagnosticSeverities.SS0402, true, helpLinkUri: HelpLinks.SS0402
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4038535&amp;doc_id=633030">
		/// SS0403
		/// </a>
		/// diagnostic result (The enum field must holds a explicitly-wroten value if the enum
		/// is marked <see cref="FlagsAttribute"/>).
		/// </summary>
		/// <seealso cref="FlagsAttribute"/>
		private static readonly DiagnosticDescriptor SS0403 = new(
			DiagnosticIds.SS0403, Titles.SS0403, Messages.SS0403, Categories.SS0403,
			DiagnosticSeverities.SS0403, true, helpLinkUri: HelpLinks.SS0403
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0402, SS0403
		);
	}
}
