#pragma warning disable IDE0057

using System;
using System.Collections.Generic;
using System.Linq;
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
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxReceiver is not SyntaxReceiver receiver)
			{
				return;
			}

			var nameDic = new Dictionary<string, int>();
			foreach (var typeSymbol in analyzerAttributeCheck(context, receiver))
			{
				_ = nameDic.TryGetValue(typeSymbol.Name, out int i);
				string name = i == 0 ? typeSymbol.Name : $"{typeSymbol.Name}{(i + 1).ToString()}";
				nameDic[typeSymbol.Name] = i + 1;

				if (getAnalyzerCode(typeSymbol) is { } generatedCode)
				{
					context.AddSource($"{name}.SupportedDiagnostics.g.cs", generatedCode);
				}
			}


			static IEnumerable<INamedTypeSymbol> analyzerAttributeCheck(
				in GeneratorExecutionContext context, SyntaxReceiver receiver)
			{
				var compilation = context.Compilation;

				return
					from candidateType in receiver.Candidates
					let model = compilation.GetSemanticModel(candidateType.SyntaxTree)
					select (INamedTypeSymbol)model.GetDeclaredSymbol(candidateType)! into typeSymbol
					where typeSymbol.Marks<CodeAnalyzerAttribute>()
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

				var diagnosticIds =
					from attribute in symbol.GetAttributes()
					where attribute.AttributeClass?.Name == nameof(CodeAnalyzerAttribute)
					let attributeStr = attribute.ToString()
					let tokenStartIndex = attributeStr.IndexOf("({")
					where tokenStartIndex != -1
					select GetMemberValues(attributeStr, tokenStartIndex);

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
