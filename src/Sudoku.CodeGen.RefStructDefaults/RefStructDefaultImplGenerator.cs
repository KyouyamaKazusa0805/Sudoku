#pragma warning disable IDE0057

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using GenericsOptions = Microsoft.CodeAnalysis.SymbolDisplayGenericsOptions;
using GlobalNamespaceStyle = Microsoft.CodeAnalysis.SymbolDisplayGlobalNamespaceStyle;
using MiscellaneousOptions = Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions;
using TypeQualificationStyle = Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

namespace Sudoku.CodeGen.RefStructDefaults
{
	/// <summary>
	/// Indicates the generator that generates the default overriden methods in a <see langword="ref struct"/>.
	/// </summary>
	[Generator]
	public sealed partial class RefStructDefaultImplGenerator : ISourceGenerator
	{
		/// <summary>
		/// Indicates the type format, and the property type format.
		/// </summary>
		private static readonly SymbolDisplayFormat
			TypeFormat = new(
				globalNamespaceStyle: GlobalNamespaceStyle.OmittedAsContaining,
				typeQualificationStyle: TypeQualificationStyle.NameAndContainingTypesAndNamespaces,
				genericsOptions: GenericsOptions.IncludeTypeParameters | GenericsOptions.IncludeTypeConstraints,
				miscellaneousOptions:
					MiscellaneousOptions.UseSpecialTypes
					| MiscellaneousOptions.EscapeKeywordIdentifiers
					| MiscellaneousOptions.IncludeNullableReferenceTypeModifier
			);


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxReceiver is not SyntaxReceiver receiver)
			{
				return;
			}

			var nameDic = new Dictionary<string, int>();
			foreach (var classSymbol in g(context, receiver))
			{
				_ = nameDic.TryGetValue(classSymbol.Name, out int i);
				string name = i == 0 ? classSymbol.Name : $"{classSymbol.Name}{(i + 1).ToString()}";
				nameDic[classSymbol.Name] = i + 1;

				if (getCode(classSymbol, context.Compilation) is { } c)
				{
					context.AddSource($"{name}.RefStructDefaults.g.cs", c);
				}
			}

			static IEnumerable<INamedTypeSymbol> g(in GeneratorExecutionContext context, SyntaxReceiver receiver)
			{
				var compilation = context.Compilation;

				return
					from candidateStruct in receiver.CandidateRefStructs
					let model = compilation.GetSemanticModel(candidateStruct.SyntaxTree)
					select (INamedTypeSymbol)model.GetDeclaredSymbol(candidateStruct)!;
			}

			static string? getCode(INamedTypeSymbol symbol, Compilation compilation)
			{
				string namespaceName = symbol.ContainingNamespace.ToDisplayString();
				string fullTypeName = symbol.ToDisplayString(TypeFormat);
				int i = fullTypeName.IndexOf('<');
				string genericParametersList = i == -1 ? string.Empty : fullTypeName.Substring(i);
				string readonlyKeywordIfWorth = symbol.IsReadOnly ? string.Empty : "readonly ";
				string equalsMethod = symbol.GetMembers().OfType<IMethodSymbol>().Any(symbol =>
					symbol is { Name: "Equals", Parameters: { Length: not 0 } parameters }
					&& (
						SymbolEqualityComparer.Default.Equals(
							parameters[0].Type,
							compilation.GetSpecialType(SpecialType.System_Object)
						)
						|| SymbolEqualityComparer.Default.Equals(
							parameters[0].Type,
							compilation
								.GetSpecialType(SpecialType.System_Object)
								.WithNullableAnnotation(NullableAnnotation.Annotated)
						)
					)
					&& SymbolEqualityComparer.Default.Equals(
						symbol.ReturnType,
						compilation.GetSpecialType(SpecialType.System_Boolean)
					)
				) ? string.Empty : $@"/// <inheritdoc cref=""object.Equals(object?)""/>
		/// <exception cref=""NotSupportedException"">Always throws.</exception>
		[CompilerGenerated, DoesNotReturn]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete(""You can't use or call this method."", true, DiagnosticId = ""BAN"")]
		public override {readonlyKeywordIfWorth}bool Equals(object? other) => throw new NotSupportedException();";

				string getHashCodeMethod = symbol.GetMembers().OfType<IMethodSymbol>().Any(symbol =>
					symbol is { Name: "GetHashCode", Parameters: { Length: 0 } parameters }
					&& SymbolEqualityComparer.Default.Equals(
						symbol.ReturnType,
						compilation.GetSpecialType(SpecialType.System_Int32)
					)
				) ? string.Empty : $@"/// <inheritdoc cref=""object.GetHashCode""/>
		/// <exception cref=""NotSupportedException"">Always throws.</exception>
		[CompilerGenerated, DoesNotReturn]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete(""You can't use or call this method."", true, DiagnosticId = ""BAN"")]
		public override {readonlyKeywordIfWorth}int GetHashCode() => throw new NotSupportedException();";

				string toStringMethod = symbol.GetMembers().OfType<IMethodSymbol>().Any(symbol =>
					symbol is { Name: "ToString", Parameters: { Length: 0 } parameters }
					&& (
						SymbolEqualityComparer.Default.Equals(
							symbol.ReturnType,
							compilation.GetSpecialType(SpecialType.System_String)
						)
						|| SymbolEqualityComparer.Default.Equals(
							symbol.ReturnType,
							compilation
								.GetSpecialType(SpecialType.System_String)
								.WithNullableAnnotation(NullableAnnotation.Annotated)
						)
					)
				) ? string.Empty : $@"/// <inheritdoc cref=""object.ToString""/>
		/// <exception cref=""NotSupportedException"">Always throws.</exception>
		[CompilerGenerated, DoesNotReturn]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete(""You can't use or call this method."", true, DiagnosticId = ""BAN"")]
		public override {readonlyKeywordIfWorth}string? ToString() => throw new NotSupportedException();";

				return $@"#pragma warning disable 809, IDE0005

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial struct {symbol.Name}{genericParametersList}
	{{
{equalsMethod}{getHashCodeMethod}{toStringMethod}
	}}
}}";
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) =>
			context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());
	}
}
