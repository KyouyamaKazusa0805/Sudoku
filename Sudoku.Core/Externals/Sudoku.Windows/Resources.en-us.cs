using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Windows
{
	partial class Resources
	{
		/// <summary>
		/// The language source for the globalization string "<c>en-us</c>".
		/// </summary>
		/// <remarks>
		/// Here we use reflection to call and use this field, which cannot be recognized by
		/// Roslyn, so we should suppress the complier warning IDE0052.
		/// </remarks>
		[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
		private static readonly IReadOnlyDictionary<string, string> LangSourceEnUs = new Dictionary<string, string>
		{
			// GridProgressResult
			["UnsolvedCells"] = "Unsolved cells: ",
			["UnsolvedCandidates"] = ", candidates: ",

			// StepFinder
			["Summary"] = "Summary...",
		};
	}
}
