using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
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
		private const string CsvTableName = "DiagnosticResults.csv";


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.IsNotInProject(ProjectNames.CodeAnalysis))
			{
				return;
			}

			if (context.AdditionalFiles is not { Length: not 0 } additionalFiles)
			{
				return;
			}

			string csvTableFilePath = additionalFiles.First(static f => f.Path.Contains(CsvTableName)).Path;
			string[] list = File.ReadAllLines(csvTableFilePath);
			string[] withoutHeader = new Memory<string>(list, 1, list.Length - 1).ToArray();
			string[][] info = (from line in withoutHeader select line.SplitInfo()).ToArray();
			string diagnosticIdClause = string.Join(
				"\r\n\t\t",
				from line in info
				let diagnosticId = line[0]
				select $"public const string {diagnosticId} = nameof({diagnosticId});"
			);
			string severitiesClause = string.Join(
				"\r\n\t\t",
				from line in info
				let pair = (Id: line[0], Severity: line[1])
				select $@"public const DiagnosticSeverity {pair.Id} = DiagnosticSeverity.{pair.Severity};"
			);
			string categoryClause = string.Join(
				"\r\n\t\t",
				from line in info
				let pair = (Id: line[0], Category: line[2])
				select $@"public const string {pair.Id} = ""{pair.Category}"";"
			);
			string helpLinkClause = string.Join(
				"\r\n\t\t",
				from line in info
				let id = line[0]
				select $@"public const string {id} = ""https://github.com/SunnieShine/Sudoku/wiki/Rule-{id}"";"
			);
			string titleClause = string.Join(
				"\r\n\t\t",
				from line in info
				let pair = (Id: line[0], Title: line[3])
				select $@"public const string {pair.Id} = ""{pair.Title}"";"
			);
			string messageClause = string.Join(
				"\r\n\t\t",
				from line in info
				let pair = (Id: line[0], Title: line[3], Message: line[4])
				select $@"public const string {pair.Id} = ""{(pair.Message == string.Empty ? pair.Title : pair.Message)}"";"
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
				"DiagnosticSeverities.g.cs",
				$@"#pragma warning disable 1591

using Microsoft.CodeAnalysis;

#nullable enable

namespace Sudoku.Diagnostics.CodeAnalysis
{{
	internal static class DiagnosticSeverities
	{{
		{severitiesClause}
	}}
}}"
			);

			context.AddSource(
				"Categories.g.cs",
				$@"#pragma warning disable 1591

#nullable enable

namespace Sudoku.Diagnostics.CodeAnalysis
{{
	internal static class Categories
	{{
		{categoryClause}
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

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}
	}
}
