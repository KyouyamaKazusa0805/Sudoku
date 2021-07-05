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
			var compilation = context.Compilation;
			foreach (var type in
				from candidateStruct in receiver.CandidateRefStructs
				let model = compilation.GetSemanticModel(candidateStruct.SyntaxTree)
				select (INamedTypeSymbol)model.GetDeclaredSymbol(candidateStruct)!)
			{
				type.DeconstructInfo(
					false, out string fullTypeName, out string namespaceName, out string genericParametersList,
					out _, out _, out string readonlyKeyword, out _
				);

				var intSymbol = compilation.GetSpecialType(SpecialType.System_Int32);
				var boolSymbol = compilation.GetSpecialType(SpecialType.System_Boolean);
				var stringSymbol = compilation.GetSpecialType(SpecialType.System_String);
				var nullableStringSymbol = stringSymbol.WithNullableAnnotation(NullableAnnotation.Annotated);
				var objectSymbol = compilation.GetSpecialType(SpecialType.System_Object);
				var nullableObjectSymbol = objectSymbol.WithNullableAnnotation(NullableAnnotation.Annotated);

				string equalsMethod = type.GetMembers().OfType<IMethodSymbol>().Any(symbol =>
					symbol is { Name: "Equals", Parameters: { Length: not 0 } parameters }
					&& (
						SymbolEqualityComparer.Default.Equals(parameters[0].Type, objectSymbol)
						|| SymbolEqualityComparer.Default.Equals(parameters[0].Type, nullableObjectSymbol)
					)
					&& SymbolEqualityComparer.Default.Equals(symbol.ReturnType, boolSymbol)
				) ? string.Empty : $@"/// <inheritdoc cref=""object.Equals(object?)""/>
		/// <exception cref=""NotSupportedException"">Always throws.</exception>
		[CompilerGenerated, DoesNotReturn, EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete(""You can't use or call this method."", true, DiagnosticId = ""BAN"")]
		public override {readonlyKeyword}bool Equals(object? other) => throw new NotSupportedException();";

				string getHashCodeMethod = type.GetMembers().OfType<IMethodSymbol>().Any(symbol =>
					symbol is { Name: "GetHashCode", Parameters: { Length: 0 } parameters }
					&& SymbolEqualityComparer.Default.Equals(symbol.ReturnType, intSymbol)
				) ? string.Empty : $@"/// <inheritdoc cref=""object.GetHashCode""/>
		/// <exception cref=""NotSupportedException"">Always throws.</exception>
		[CompilerGenerated, DoesNotReturn, EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete(""You can't use or call this method."", true, DiagnosticId = ""BAN"")]
		public override {readonlyKeyword}int GetHashCode() => throw new NotSupportedException();";

				string toStringMethod = type.GetMembers().OfType<IMethodSymbol>().Any(symbol =>
					symbol is { Name: "ToString", Parameters: { Length: 0 } parameters }
					&& (
						SymbolEqualityComparer.Default.Equals(symbol.ReturnType, stringSymbol)
						|| SymbolEqualityComparer.Default.Equals(symbol.ReturnType, nullableStringSymbol)
					)
				) ? string.Empty : $@"/// <inheritdoc cref=""object.ToString""/>
		/// <exception cref=""NotSupportedException"">Always throws.</exception>
		[CompilerGenerated, DoesNotReturn, EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete(""You can't use or call this method."", true, DiagnosticId = ""BAN"")]
		public override {readonlyKeyword}string? ToString() => throw new NotSupportedException();";

				context.AddSource(
					type.ToFileName(),
					"RefStructDefaults",
					$@"#pragma warning disable 809, IDE0005

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial struct {type.Name}{genericParametersList}
	{{
#line hidden
{equalsMethod}{getHashCodeMethod}{toStringMethod}
#line default
	}}
}}"
				);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
	}
}
