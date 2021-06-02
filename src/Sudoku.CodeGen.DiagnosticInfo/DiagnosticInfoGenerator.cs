#pragma warning disable IDE0057

using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Sudoku.CodeGen.DiagnosticInfo
{
	/// <summary>
	/// Indicates the generator that generates the diagnostic information used in code analysis.
	/// </summary>
	[Generator]
	public sealed class DiagnosticInfoGenerator : ISourceGenerator
	{
		/// <summary>
		/// Indicates the name that stores the diagnostic information.
		/// </summary>
		private const string CsvTableName = "DiagnosticResults";

		/// <summary>
		/// Indicates the name that stores the diagnostic categories.
		/// </summary>
		private const string CategoriesTableName = "SupportedDiagnosticCategories";


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var additionalFiles = context.AdditionalFiles;

			string
				csvTableFilePath = additionalFiles.First(
					static file => file.Path.Contains(CsvTableName)
				).Path,
				categoriesTableFilePath = additionalFiles.First(
					static file => file.Path.Contains(CategoriesTableName)
				).Path;

			GenerateDiagnosticInfoList(context, csvTableFilePath);
			GenerateDiagnosticCategories(context, categoriesTableFilePath);
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}


		private static void GenerateDiagnosticInfoList(GeneratorExecutionContext context, string path)
		{
			string[] list = File.ReadAllLines(path);
			string[] withoutHeader = new Memory<string>(list, 1, list.Length - 1).ToArray();

			string[][] info = (from line in withoutHeader select SplitInfo(line)).ToArray();
			string diagnosticIdClause = string.Join(
				"\r\n\t\t",
				from line in info
				let diagnosticId = line[0]
				select $"public const string {diagnosticId} = nameof({diagnosticId});"
			);
			string helpLinkClause = string.Join(
				"\r\n\t\t",
				from line in info
				let pair = (Id: line[0], HelpLink: line[4])
				select $@"public const string {pair.Id} = ""{pair.HelpLink}"";"
			);
			string titleClause = string.Join(
				"\r\n\t\t",
				from line in info
				let pair = (Id: line[0], Title: line[5])
				select $@"public const string {pair.Id} = ""{pair.Title}"";"
			);
			string messageClause = string.Join(
				"\r\n\t\t",
				from line in info
				let pair = (Id: line[0], Title: line[5], Message: line[6])
				select $@"public const string {pair.Id} = ""{
					(pair.Message == string.Empty ? pair.Title : pair.Message)
				}"";"
			);

			context.AddSource(
				"DiagnosticIds.g.cs",
				$@"#pragma warning disable 1591

#nullable enable

namespace Sudoku.Diagnostics.CodeAnalysis
{{
	internal static class DiagnosticIds
	{{
		{diagnosticIdClause}
	}}
}}"
			);

			context.AddSource(
				"HelpLinks.g.cs",
				$@"#pragma warning disable 1591

#nullable enable

namespace Sudoku.Diagnostics.CodeAnalysis
{{
	internal static class HelpLinks
	{{
		{helpLinkClause}
	}}
}}"
			);

			context.AddSource(
				"Titles.g.cs",
				$@"#pragma warning disable 1591

#nullable enable

namespace Sudoku.Diagnostics.CodeAnalysis
{{
	internal static class Titles
	{{
		{titleClause}
	}}
}}"
			);

			context.AddSource(
				"Messages.g.cs",
				$@"#pragma warning disable 1591

#nullable enable

namespace Sudoku.Diagnostics.CodeAnalysis
{{
	internal static class Messages
	{{
		{messageClause}
	}}
}}"
			);
		}

		private static void GenerateDiagnosticCategories(GeneratorExecutionContext context, string path)
		{
			string[] categoryOrEmptyLines = File.ReadAllLines(path);
			string resultList = string.Join(
				"\r\n\t\t",
				from categoryOrEmptyLine in categoryOrEmptyLines
				select string.IsNullOrWhiteSpace(categoryOrEmptyLine)
					? string.Empty
					: $"public const string {categoryOrEmptyLine} = nameof({categoryOrEmptyLine});"
			);

			context.AddSource(
				"Categories.g.cs",
				$@"#pragma warning disable 1591

#nullable enable

namespace Sudoku.Diagnostics.CodeAnalysis
{{
	internal static class Categories
	{{
		{resultList}
	}}
}}"
			);
		}

		private static unsafe string[] SplitInfo(string line)
		{
			if ((line.Count(static c => c == '"') & 1) != 0)
			{
				throw new ArgumentException("The specified string is invalid to split.", nameof(line));
			}

			fixed (char* pLine = line)
			{
				for (int i = 0; i < line.Length - 1;)
				{
					if (pLine[i++] != '"')
					{
						continue;
					}

					for (int j = i + 1; j < line.Length; j++)
					{
						if (pLine[j] != '"')
						{
							continue;
						}

						for (int p = i + 1; p <= j - 1; p++)
						{
							if (pLine[p] == ',')
							{
								// Change to the temporary character.
								pLine[p] = '，';
							}
						}

						i = j + 1 + 1;
						break;
					}
				}
			}

			string[] result = line.Split(',');
			for (int i = 0; i < result.Length; i++)
			{
				string temp = result[i].Replace(@"""", string.Empty).Replace('，', ',');

				result[i] = i == result.Length - 1 || i == result.Length - 2
					? string.IsNullOrEmpty(temp) ? string.Empty : temp.Substring(0, temp.Length - 1)
					: temp;
			}

			return result;
		}
	}
}
