using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// A generator that generates the code for code analyzer and fix defaults.
	/// </summary>
	[Generator]
	public sealed partial class CodeAnalyzerOrFixerDefaultsGenerator : ISourceGenerator
	{
		/// <summary>
		/// Indicates the name that stores the diagnostic information.
		/// </summary>
		private const string CsvTableName = "DiagnosticResults.csv";


		/// <summary>
		/// Indicates the regular expression for extraction of the information.
		/// </summary>
		private static readonly Regex InfoRegex = new(
			@"(?<=\("")[A-Za-z]{2}\d{4}(?=""\))",
			RegexOptions.ExplicitCapture,
			TimeSpan.FromSeconds(5)
		);


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.AdditionalFiles is not { Length: not 0 } additionalFiles)
			{
				return;
			}

			if (additionalFiles.GetPath(static f => f.Contains(CsvTableName)) is not { } csvTableFilePath)
			{
				return;
			}

			var compilation = context.Compilation;
			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
			var attributeAnalyzerSymbol = compilation.GetTypeByMetadataName<CodeAnalyzerAttribute>();
			var attributeFixerSymbol = compilation.GetTypeByMetadataName<CodeFixProviderAttribute>();
			Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
			string[] list = File.ReadAllLines(csvTableFilePath);
			string[] withoutHeader = new Memory<string>(list, 1, list.Length - 1).ToArray();
			string[][] info = (from line in withoutHeader select line.SplitInfo()).ToArray();

			foreach (var type in
				from type in receiver.Candidates
				let model = compilation.GetSemanticModel(type.SyntaxTree)
				select (INamedTypeSymbol)model.GetDeclaredSymbol(type)! into type
				where type.GetAttributes().Any(a => f(a.AttributeClass, attributeAnalyzerSymbol))
				select type)
			{
				type.DeconstructInfo(
					false, out string fullTypeName, out string namespaceName,
					out _, out _, out _, out _, out bool isGeneric
				);
				if (isGeneric)
				{
					continue;
				}

				var attributeSymbol = compilation.GetTypeByMetadataName(typeof(CodeAnalyzerAttribute).FullName);
				var selection =
					from attributeStr in type.GetAttributeStrings(attributeSymbol)
					let tokenStartIndex = attributeStr.IndexOf("({")
					where tokenStartIndex != -1
					select attributeStr.GetMemberValues(tokenStartIndex);
				if (!selection.Any())
				{
					continue;
				}

				string[] diagnosticIds = selection.First();
				string diagnosticResults = string.Join(
					"\r\n\t",
					from diagnosticId in diagnosticIds
					let id = Cut(diagnosticId)
					select $@"/// <item>
	/// <term><a href=""https://sunnieshine.github.io/Sudoku/rules/Rule-{id}"">{diagnosticId}</a></term>
	/// <description>{GetDescription(info, id)}</description>
	/// </item>"
				);
				string descriptors = string.Join(
					"\r\n\r\n\t\t",
					from diagnosticId in diagnosticIds
					let tags = GetWhetherFadingOutTag(diagnosticId)
					let id = Cut(diagnosticId)
					select $@"/// <summary>
		/// Indicates the <a href=""https://sunnieshine.github.io/Sudoku/rules/Rule-{id}"">{id}</a>
		/// diagnostic result ({GetDescription(info, id)}).
		/// </summary>
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""0.3"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		private static readonly DiagnosticDescriptor {id} = new(
			DiagnosticIds.{id}, Titles.{id}, Messages.{id}, Categories.{id},
			DiagnosticSeverities.{id}, true, helpLinkUri: HelpLinks.{id}{tags}
		);"
				);

				string supportedInstances = string.Join(
					", ",
					from diagnosticId in diagnosticIds select Cut(diagnosticId)
				);

				context.AddSource(
					type.ToFileName(),
					"Analyzer",
					$@"using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

#nullable enable

namespace {namespaceName}
{{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for the following diagnostic results:
	/// <list type=""table"">
	{diagnosticResults}
	/// </list>
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	partial class {type.Name}
	{{
		{descriptors}


		/// <inheritdoc/>
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""0.3"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			{supportedInstances}
		);
	}}
}}"
				);
			}

			foreach (var type in
				from type in receiver.Candidates
				let model = compilation.GetSemanticModel(type.SyntaxTree)
				select (INamedTypeSymbol)model.GetDeclaredSymbol(type)! into type
				where type.GetAttributes().Any(a => f(a.AttributeClass, attributeFixerSymbol))
				select type)
			{
				type.DeconstructInfo(
					false, out string fullTypeName, out string namespaceName, out _,
					out _, out _, out _, out bool isGeneric
				);
				if (isGeneric)
				{
					continue;
				}

				var attributeSymbol = compilation.GetTypeByMetadataName(typeof(CodeFixProviderAttribute).FullName);
				string typeName = type.Name;
				string id = (
					from attribute in type.GetAttributes()
					where SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol)
					select InfoRegex.Match(attribute.ToString()) into match
					where match.Success
					select match.Value
				).First();
				string description = GetDescription(info, id);

				context.AddSource(
					type.ToFileName(),
					"Fixer",
					$@"using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

#nullable enable

namespace {namespaceName}
{{
	/// <summary>
	/// Indicates the code fixer for solving the diagnostic result
	/// <a href=""https://sunnieshine.github.io/Sudoku/rules/Rule-{id}"">{id}</a>
	/// ({description}).
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof({typeName})), Shared]
	partial class {typeName}
	{{
		/// <inheritdoc/>
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""0.3"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
			DiagnosticIds.{id}
		);

		/// <inheritdoc/>
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""0.3"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
	}}
}}"
				);
			}
		}


		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();


		/// <summary>
		/// Cut the diagnostic ID and get the base ID part. This method will remove the suffix <c>"F"</c>
		/// if exists.
		/// </summary>
		/// <param name="id">The diagnostic ID.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string Cut(string id) => id.EndsWith('F') ? id.Substring(0, id.Length - 1) : id;

		/// <summary>
		/// Get the raw code when the ID contains the suffix <c>"F"</c>.
		/// </summary>
		/// <param name="id">The diagnostic ID.</param>
		/// <returns>The raw code for representing the option of fading out the code.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string GetWhetherFadingOutTag(string id) =>
			id.EndsWith('F') ? ", customTags: new[] { WellKnownDiagnosticTags.Unnecessary }" : string.Empty;

		/// <summary>
		/// Get the description of from the split result.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="id">The diagnostic ID.</param>
		/// <returns>The description.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string GetDescription(string[][] info, string id) => (
			from line in info select (Id: line[0], Title: line[3])
		).First(pair => pair.Id == id).Title.Replace("{", "&lt;").Replace("}", "&gt;");
	}
}
