using Sudoku.Diagnostics.CodeGen.Data;

namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Provides with a Markdown handler that handles for a diagnostic list Markdown file.
/// </summary>
internal static class MarkdownHandler
{
	/// <summary>
	/// To split the whole markdown table.
	/// </summary>
	/// <param name="markdown">The markdown code.</param>
	/// <returns>The result list.</returns>
	/// <exception cref="FormatException">Throws when any wrong operation has encountered.</exception>
	public static DiagnosticDetail[] SplitTable(string markdown)
	{
		string[] lines = markdown.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		lines = new ArraySegment<string>(lines, 2, lines.Length - 2).ToArray();

		var diagnostics = new DiagnosticDetail[lines.Length];
		for (int i = 0, length = diagnostics.Length; i < length; i++)
		{
#nullable disable
			if (splitLine(lines[i]) is not [var id, var category, var severity, var title, var messageFormat])
			{
				throw new FormatException();
			}

			bool containsPlaceholders = !string.IsNullOrEmpty(messageFormat);
			diagnostics[i] = new(
				id,
				category,
				(DiagnosticSeverity)Enum.Parse(typeof(DiagnosticSeverity), severity),
				title,
				containsPlaceholders ? messageFormat : title,
				containsPlaceholders
			);
#nullable restore
		}

		return diagnostics;


		static string[] splitLine(string line)
		{
			string[] temp = line.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			string[] result = new string[temp.Length];
			for (int i = 0, length = temp.Length; i < length; i++)
			{
				result[i] = temp[i].Trim();
			}

			return result;
		}
	}
}
