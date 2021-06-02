#if DEBUG && (NET5_0 || NETSTANDARD2_0)

using System;
using System.IO;
using System.Text;
using static System.Environment;
#if NET5_0
using System.Threading.Tasks;
#endif

namespace Sudoku.Diagnostics.CodeAnalysis.IO
{
	/// <summary>
	/// Defines a diagnostic result output.
	/// </summary>
	/// <remarks>
	/// Please note that this class is only available in debug environment and .NET 5.0 or
	/// .NET standard 2.0 enviornment; otherwise, an compiler error.
	/// </remarks>
	public static class DiagnosticResultOutput
	{
		/// <summary>
		/// The type information instance.
		/// </summary>
		private static readonly Type HelpLinkType = typeof(HelpLinks), TitleType = typeof(Titles), MessageType = typeof(Messages);


#if NET5_0
		/// <summary>
		/// Output the diagnostic information and converts it to <c>*.csv</c> format.
		/// </summary>
		/// <param name="diagnosticResultIdPath">The diagnostic reusult ID path.</param>
		/// <returns>The task of the operation.</returns>
		/// <exception cref="ArgumentException">
		/// Throws when the <paramref name="outputPath"/> doesn't end with <c>csv</c>.
		/// </exception>
		public static async Task OutputAsync(string diagnosticResultIdPath)
#elif NETSTANDARD2_0
		/// <summary>
		/// Output the diagnostic information and converts it to <c>*.csv</c> format.
		/// </summary>
		/// <param name="diagnosticResultIdPath">The diagnostic reusult ID path.</param>
		/// <param name="outputPath">The path that stores the output file.</param>
		/// <exception cref="ArgumentException">
		/// Throws when the <paramref name="outputPath"/> doesn't end with <c>csv</c>.
		/// </exception>
		public static void Output(string diagnosticResultIdPath, string outputPath)
#endif
		{
			if (Path.GetExtension(outputPath) != ".csv")
			{
				throw new ArgumentException("The specified path is invalid.", nameof(outputPath));
			}

			const char falseMark = 'x', separator = ',';
			const string defaultString = "";

			var sb = new StringBuilder();
			string[] lines =
#if NET5_0
				await File.ReadAllLinesAsync(diagnosticResultIdPath)
#elif NETSTANDARD2_0
				File.ReadAllLines(diagnosticResultIdPath)
#endif
				;

			sb.AppendLine($"ID{separator}Level{separator}Category{separator}Has Code Fix{separator}Help Link{separator}Title{separator}Message Format");
			foreach (string line in lines)
			{
				if (string.IsNullOrWhiteSpace(line))
				{
					continue;
				}

				sb
					.Append(line).Append(separator)
					.Append(defaultString).Append(separator)
					.Append(defaultString).Append(separator)
					.Append(falseMark).Append(separator);

				string helpLink = (string)HelpLinkType.GetField(line)!.GetValue(null)!;
				sb.Append(helpLink).Append(separator);

				string title = (string)TitleType.GetField(line)!.GetValue(null)!;
				sb.Append(escapeComma(title)).Append('.').Append(separator);

				string messageFormat = (string)MessageType.GetField(line)!.GetValue(null)!;
				sb.Append(title == messageFormat ? defaultString : $"{escapeComma(messageFormat)}.").AppendLine();
			}

#if NET5_0
			await File.WriteAllTextAsync
#elif NETSTANDARD2_0
			File.WriteAllText
#endif
			(
				Path.Combine(GetFolderPath(SpecialFolder.Desktop), "DiagnosticResults.csv"),
				sb.ToString()
			);

			static string escapeComma(string input) =>
				input.Contains
				(
#if NET5_0
					','
#elif NETSTANDARD2_0
					","
#endif
				) ? $@"""{input}""" : input;
		}
	}
}
#else
#error The version is invalid.
#endif
