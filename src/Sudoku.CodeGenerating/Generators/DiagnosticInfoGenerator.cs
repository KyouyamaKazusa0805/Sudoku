using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;
using static Sudoku.CodeGenerating.Constants;

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

			if (additionalFiles.GetPath(static f => f.Contains(CsvTableName)) is not { } csvTableFilePath)
			{
				return;
			}

			const string separator = "\r\n\t\t";
			string[] list = File.ReadAllLines(csvTableFilePath);
			string[] withoutHeader = new Memory<string>(list, 1, list.Length - 1).ToArray();
			string[][] info = (from line in withoutHeader select line.SplitInfo()).ToArray();
			string diagnosticIdClause = f(
				from line in info
				select line[0] into diagnosticId
				select $"public const string {diagnosticId} = nameof({diagnosticId});"
			);
			string severitiesClause = f(
				from line in info
				select (Id: line[0], Severity: line[1]) into pair
				select $@"public const DiagnosticSeverity {pair.Id} = DiagnosticSeverity.{pair.Severity};"
			);
			string categoryClause = f(
				from line in info
				select (Id: line[0], Category: line[2]) into pair
				select $@"public const string {pair.Id} = ""{pair.Category}"";"
			);
			string helpLinkClause = f(
				from line in info
				select line[0] into id
				select $@"public const string {id} = ""https://sunnieshine.github.io/Sudoku/code-analysis/rules/Rule-{id}"";"
			);
			string titleClause = f(
				from line in info
				select (Id: line[0], Title: line[3]) into pair
				select $@"public const string {pair.Id} = ""{pair.Title}"";"
			);
			string messageClause = f(
				from line in info
				select (Id: line[0], Title: line[3], Message: line[4]) into triplet
				select $@"public const string {triplet.Id} = ""{(
					triplet.Message == string.Empty ? triplet.Title : triplet.Message
				)}"";"
			);

			context.AddSource(
				"DiagnosticIds",
				null,
				$@"#pragma warning disable 1591

#nullable enable

namespace Sudoku.Diagnostics.CodeAnalysis
{{
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	internal static class DiagnosticIds
	{{
		{diagnosticIdClause}
	}}
}}"
			);

			context.AddSource(
				"DiagnosticSeverities",
				null,
				$@"#pragma warning disable 1591

using Microsoft.CodeAnalysis;

#nullable enable

namespace Sudoku.Diagnostics.CodeAnalysis
{{
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	internal static class DiagnosticSeverities
	{{
		{severitiesClause}
	}}
}}"
			);

			context.AddSource(
				"Categories",
				null,
				$@"#pragma warning disable 1591

#nullable enable

namespace Sudoku.Diagnostics.CodeAnalysis
{{
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	internal static class Categories
	{{
		{categoryClause}
	}}
}}"
			);

			context.AddSource(
				"HelpLinks",
				null,
				$@"#pragma warning disable 1591

#nullable enable

namespace Sudoku.Diagnostics.CodeAnalysis
{{
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	internal static class HelpLinks
	{{
		{helpLinkClause}
	}}
}}"
			);

			context.AddSource(
				"Titles",
				null,
				$@"#pragma warning disable 1591

#nullable enable

namespace Sudoku.Diagnostics.CodeAnalysis
{{
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	internal static class Titles
	{{
		{titleClause}
	}}
}}"
			);

			context.AddSource(
				"Messages",
				null,
				$@"#pragma warning disable 1591

#nullable enable

namespace Sudoku.Diagnostics.CodeAnalysis
{{
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	internal static class Messages
	{{
		{messageClause}
	}}
}}"
			);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static string f(IEnumerable<string> linqExpression) => string.Join(separator, linqExpression);
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}
	}
}
