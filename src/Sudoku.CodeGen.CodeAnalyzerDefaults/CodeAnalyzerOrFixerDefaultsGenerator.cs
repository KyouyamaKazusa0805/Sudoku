#pragma warning disable IDE0057

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGen.CodeAnalyzerDefaults.Extensions;

namespace Sudoku.CodeGen.CodeAnalyzerDefaults
{
	/// <summary>
	/// A generator that generates the code for code analyzer and fix defaults.
	/// </summary>
	[Generator]
	public sealed partial class CodeAnalyzerOrFixerDefaultsGenerator : ISourceGenerator
	{
		/// <summary>
		/// Indicates the regular expression for extraction of the information.
		/// </summary>
		private static readonly Regex InfoRegex = new(@"(?<=\("")[A-Za-z]{2}\d{4}(?=""\))");


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxReceiver is not SyntaxReceiver receiver)
			{
				return;
			}

			var analyzerNameDic = new Dictionary<string, int>();
			foreach (var typeSymbol in attributeCheck<CodeAnalyzerAttribute>(context, receiver))
			{
				_ = analyzerNameDic.TryGetValue(typeSymbol.Name, out int i);
				string name = i == 0 ? typeSymbol.Name : $"{typeSymbol.Name}{(i + 1).ToString()}";
				analyzerNameDic[typeSymbol.Name] = i + 1;

				if (getAnalyzerCode(typeSymbol) is { } generatedCode)
				{
					context.AddSource($"{name}.SupportedDiagnostics.g.cs", generatedCode);
				}
			}

			var fixerNameDic = new Dictionary<string, int>();
			foreach (var typeSymbol in attributeCheck<CodeFixProviderAttribute>(context, receiver))
			{
				_ = analyzerNameDic.TryGetValue(typeSymbol.Name, out int i);
				string name = i == 0 ? typeSymbol.Name : $"{typeSymbol.Name}{(i + 1).ToString()}";
				analyzerNameDic[typeSymbol.Name] = i + 1;

				if (getFixerCode(typeSymbol) is { } generatedCode)
				{
					context.AddSource($"{name}.SupportedDiagnostics.g.cs", generatedCode);
				}
			}


			static IEnumerable<INamedTypeSymbol> attributeCheck<TAttribute>(
				in GeneratorExecutionContext context, SyntaxReceiver receiver)
				where TAttribute : Attribute
			{
				var compilation = context.Compilation;

				return
					from candidateType in receiver.Candidates
					let model = compilation.GetSemanticModel(candidateType.SyntaxTree)
					select (INamedTypeSymbol)model.GetDeclaredSymbol(candidateType)! into typeSymbol
					where typeSymbol.Marks<TAttribute>()
					select typeSymbol;
			}

			string? getAnalyzerCode(INamedTypeSymbol symbol)
			{
				string namespaceName = symbol.ContainingNamespace.ToDisplayString();
				string fullTypeName = symbol.ToDisplayString(FormatOptions.TypeFormat);
				int i = fullTypeName.IndexOf('<');
				if (i != -1)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							"SG0001", "SourceGenerator", "The type can't be generic one.",
							DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0,
							null, null, helpLink: null, location: symbol.Locations[0],
							additionalLocations: null, null, null
						)
					);

					return null;
				}

				string[] diagnosticIds = (
					from attribute in symbol.GetAttributes()
					where attribute.AttributeClass?.Name == nameof(CodeAnalyzerAttribute)
					let attributeStr = attribute.ToString()
					let tokenStartIndex = attributeStr.IndexOf("({")
					where tokenStartIndex != -1
					select GetMemberValues(attributeStr, tokenStartIndex)
				).First();

				string descriptors = string.Join(
					"\r\n\r\n\t\t",
					from id in diagnosticIds
					select $@"/// <summary>
		/// Indicates the <a href=""https://github.com/SunnieShine/Sudoku/wiki/Rule-{id}"">{id}</a>
		/// diagnostic result.
		/// </summary>
		[CompilerGenerated]
		private static readonly DiagnosticDescriptor {id} = new(
			DiagnosticIds.{id}, Titles.{id}, Messages.{id}, Categories.{id},
			DiagnosticSeverities.{id}, true, helpLinkUri: HelpLinks.{id}
		);"
				);

				string supportedInstances = string.Join(",", diagnosticIds);

				return $@"#pragma warning disable 1591

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

#nullable enable

namespace {namespaceName}
{{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	partial class {symbol.Name}
	{{
		{descriptors}


		/// <inheritdoc/>
		[CompilerGenerated]
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			{supportedInstances}
		);
	}}
}}";
			}

			string? getFixerCode(INamedTypeSymbol symbol)
			{
				string namespaceName = symbol.ContainingNamespace.ToDisplayString();
				string fullTypeName = symbol.ToDisplayString(FormatOptions.TypeFormat);
				int i = fullTypeName.IndexOf('<');
				if (i != -1)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							"SG0001", "SourceGenerator", "The type can't be generic one.",
							DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0,
							null, null, helpLink: null, location: symbol.Locations[0],
							additionalLocations: null, null, null
						)
					);

					return null;
				}

				string className = symbol.Name;
				string id = (
					from attribute in symbol.GetAttributes()
					where attribute.AttributeClass?.Name == nameof(CodeFixProviderAttribute)
					let match = InfoRegex.Match(attribute.ToString())
					where match.Success
					select match.Value
				).First();

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
	/// <a href=""https://github.com/SunnieShine/Sudoku/wiki/Rule-{id}"">{id}</a>.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof({className})), Shared]
	partial class {className}
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
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) =>
			context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());


		/// <summary>
		/// Get member values via attribute arguments.
		/// </summary>
		/// <param name="attributeStr">
		/// The <see cref="string"/> result that is called <c>ToString</c> from an attribute instance.
		/// </param>
		/// <param name="tokenStartIndex">A token start index.</param>
		/// <returns>The result list.</returns>
		private static string[] GetMemberValues(string attributeStr, int tokenStartIndex)
		{
			string[] result = (
				from parameterValue in attributeStr.Substring(
					tokenStartIndex,
					attributeStr.Length - tokenStartIndex - 2
				).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
				select parameterValue.Substring(1, parameterValue.Length - 2)
			).ToArray();

			result[0] = result[0].Substring(2);
			return result;
		}
	}
}
