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
		/// Indicates the name that stores the diagnostic IDs.
		/// </summary>
		private const string IdTableName = "SupportedDiagnosticIds";

		/// <summary>
		/// Indicates the name that stores the diagnostic categories.
		/// </summary>
		private const string CategoriesTableName = "SupportedDiagnosticCategories";


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var additionalFiles = context.AdditionalFiles;

			string
				idTableFilePath = additionalFiles.First(
					static file => file.Path.Contains(IdTableName)
				).Path,
				categoriesTableFilePath = additionalFiles.First(
					static file => file.Path.Contains(CategoriesTableName)
				).Path;

			GenerateDiagnosticIds(context, idTableFilePath);
			GenerateDiagnosticCategories(context, categoriesTableFilePath);
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}


		private static void GenerateDiagnosticIds(GeneratorExecutionContext context, string path)
		{
			string[] idOrEmptyLines = File.ReadAllLines(path);
			string resultList = string.Join(
				"\r\n\t\t",
				from idOrEmptyLine in idOrEmptyLines
				select string.IsNullOrWhiteSpace(idOrEmptyLine)
					? "\r\n"
					: $"public const string {idOrEmptyLine} = nameof({idOrEmptyLine});"
			);

			context.AddSource(
				"DiagnosticIds.g.cs",
				$@"#pragma warning disable 1591

#nullable enable

namespace Sudoku.Diagnostics.CodeAnalysis
{{
	internal static class DiagnosticIds
	{{
		{resultList}
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
					? "\r\n"
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
	}
}
