using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Windows
{
	partial class Resources
	{
		/// <summary>
		/// The language source for the globalization string "<c>zh-cn</c>".
		/// </summary>
		/// <remarks>
		/// Here we use reflection to call and use this field, which cannot be recognized by
		/// Roslyn, so we should suppress the complier warning IDE0052.
		/// </remarks>
		[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
		private static readonly IReadOnlyDictionary<string, string> LangSourceZhCn = new Dictionary<string, string>
		{
			// GridProgressResult
			["UnsolvedCells"] = "剩余空格总数：",
			["UnsolvedCandidates"] = "，剩余候选数总数：",

			// StepFinder
			["Summary"] = "正在统计中…",
		};
	}
}
