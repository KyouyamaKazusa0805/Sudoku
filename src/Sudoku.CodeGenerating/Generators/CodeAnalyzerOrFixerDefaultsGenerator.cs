using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

			var compilation = context.Compilation;
			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
			string csvTableFilePath = additionalFiles.First(static f => f.Path.Contains(CsvTableName)).Path;
			string[] list = File.ReadAllLines(csvTableFilePath);
			string[] withoutHeader = new Memory<string>(list, 1, list.Length - 1).ToArray();
			string[][] info = (from line in withoutHeader select splitInfo(line)).ToArray();

			var analyzerNameDic = new Dictionary<string, int>();
			foreach (var typeSymbol in attributeCheck<CodeAnalyzerAttribute>(context, receiver))
			{
				_ = analyzerNameDic.TryGetValue(typeSymbol.Name, out int i);
				string name = i == 0 ? typeSymbol.Name : $"{typeSymbol.Name}{(i + 1).ToString()}";
				analyzerNameDic[typeSymbol.Name] = i + 1;

				if (getAnalyzerCode(typeSymbol) is { } generatedCode)
				{
					context.AddSource($"Analyzer.{name}.SupportedDiagnostics.g.cs", generatedCode);
				}
			}

			var fixerNameDic = new Dictionary<string, int>();
			foreach (var typeSymbol in attributeCheck<CodeFixProviderAttribute>(context, receiver))
			{
				_ = fixerNameDic.TryGetValue(typeSymbol.Name, out int i);
				string name = i == 0 ? typeSymbol.Name : $"{typeSymbol.Name}{(i + 1).ToString()}";
				fixerNameDic[typeSymbol.Name] = i + 1;

				if (getFixerCode(typeSymbol) is { } generatedCode)
				{
					context.AddSource($"Fixer.{name}.SupportedDiagnostics.g.cs", generatedCode);
				}
			}


			static IEnumerable<INamedTypeSymbol> attributeCheck<TAttribute>(
				in GeneratorExecutionContext context, SyntaxReceiver receiver)
				where TAttribute : Attribute
			{
				var compilation = context.Compilation;
				var attributeSymbol = compilation.GetTypeByMetadataName(typeof(TAttribute).FullName);
				return
					from candidateType in receiver.Candidates
					let model = compilation.GetSemanticModel(candidateType.SyntaxTree)
					select (INamedTypeSymbol)model.GetDeclaredSymbol(candidateType)! into typeSymbol
					where typeSymbol.GetAttributes().Any(
						a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)
					)
					select typeSymbol;
			}

			string? getAnalyzerCode(INamedTypeSymbol symbol)
			{
				symbol.DeconstructInfo(
					false, out string fullTypeName, out string namespaceName,
					out _, out _, out _, out _, out bool isGeneric
				);
				if (isGeneric)
				{
					return null;
				}

				var attributeSymbol = compilation.GetTypeByMetadataName(typeof(CodeAnalyzerAttribute).FullName);
				var selection =
					from attributeStr in symbol.GetAttributeStrings(attributeSymbol)
					let tokenStartIndex = attributeStr.IndexOf("({")
					where tokenStartIndex != -1
					select attributeStr.GetMemberValues(tokenStartIndex);
				if (!selection.Any())
				{
					return null;
				}

				string[] diagnosticIds = selection.First();
				string diagnosticResults = string.Join(
					"\r\n\t",
					from diagnosticId in diagnosticIds
					let id = cut(diagnosticId)
					select $@"/// <item>
	/// <term><a href=""https://sunnieshine.github.io/Sudoku/rules/Rule-{id}"">{diagnosticId}</a></term>
	/// <description>{getDescription(id)}</description>
	/// </item>"
				);
				string descriptors = string.Join(
					"\r\n\r\n\t\t",
					from diagnosticId in diagnosticIds
					let tags = getWhetherFadingOutTag(diagnosticId)
					let id = cut(diagnosticId)
					select $@"/// <summary>
		/// Indicates the <a href=""https://sunnieshine.github.io/Sudoku/rules/Rule-{id}"">{id}</a>
		/// diagnostic result ({getDescription(id)}).
		/// </summary>
		[CompilerGenerated]
		private static readonly DiagnosticDescriptor {id} = new(
			DiagnosticIds.{id}, Titles.{id}, Messages.{id}, Categories.{id},
			DiagnosticSeverities.{id}, true, helpLinkUri: HelpLinks.{id}{tags}
		);"
				);

				string supportedInstances = string.Join(
					", ",
					from diagnosticId in diagnosticIds select cut(diagnosticId)
				);
				string typeName = symbol.Name;

				return $@"#pragma warning disable 1591

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
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
	partial class {typeName}
	{{
		{descriptors}


		/// <inheritdoc/>
		[CompilerGenerated]
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			{supportedInstances}
		);
	}}
}}";


				static string getWhetherFadingOutTag(string id) =>
					id.EndsWith("F")
					? ", customTags: new[] { WellKnownDiagnosticTags.Unnecessary }"
					: string.Empty;

				static string cut(string id) => id.EndsWith("F") ? id.Substring(0, id.Length - 1) : id;
			}

			string? getFixerCode(INamedTypeSymbol symbol)
			{
				symbol.DeconstructInfo(
					false, out string fullTypeName, out string namespaceName, out _,
					out _, out _, out _, out bool isGeneric
				);
				if (isGeneric)
				{
					return null;
				}

				var attributeSymbol = compilation.GetTypeByMetadataName(typeof(CodeFixProviderAttribute).FullName);
				string typeName = symbol.Name;
				string id = (
					from attribute in symbol.GetAttributes()
					where SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol)
					let match = InfoRegex.Match(attribute.ToString())
					where match.Success
					select match.Value
				).First();
				string description = getDescription(id);

				return $@"#pragma warning disable 1591

using System.Collections.Immutable;
using System.Composition;
using System.Runtime.CompilerServices;
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
		[CompilerGenerated]
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
			DiagnosticIds.{id}
		);

		/// <inheritdoc/>
		[CompilerGenerated]
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
	}}
}}";
			}

			string getDescription(string id) => (
				from line in info select (Id: line[0], Title: line[3])
			).First(pair => pair.Id == id).Title;

			static unsafe string[] splitInfo(string line)
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

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
	}
}
