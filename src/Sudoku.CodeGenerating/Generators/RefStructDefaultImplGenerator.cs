using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Indicates the generator that generates the default overriden methods in a <see langword="ref struct"/>.
	/// </summary>
	[Generator]
	public sealed partial class RefStructDefaultImplGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
			var nameDic = new Dictionary<string, int>();
			var compilation = context.Compilation;
			foreach (var classSymbol in
				from candidateStruct in receiver.CandidateRefStructs
				let model = compilation.GetSemanticModel(candidateStruct.SyntaxTree)
				select (INamedTypeSymbol)model.GetDeclaredSymbol(candidateStruct)!)
			{
				_ = nameDic.TryGetValue(classSymbol.Name, out int i);
				string name = i == 0 ? classSymbol.Name : $"{classSymbol.Name}{(i + 1).ToString()}";
				nameDic[classSymbol.Name] = i + 1;

				if (getCode(classSymbol, context.Compilation) is { } c)
				{
					context.AddSource($"{name}.RefStructDefaults.g.cs", c);
				}
			}


			static string? getCode(INamedTypeSymbol symbol, Compilation compilation)
			{
				symbol.DeconstructInfo(
					false, out string fullTypeName, out string namespaceName, out string genericParametersList,
					out _, out _, out string readonlyKeyword, out _
				);

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
		public override {readonlyKeyword}bool Equals(object? other) => throw new NotSupportedException();";

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
		public override {readonlyKeyword}int GetHashCode() => throw new NotSupportedException();";

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
		public override {readonlyKeyword}string? ToString() => throw new NotSupportedException();";

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
