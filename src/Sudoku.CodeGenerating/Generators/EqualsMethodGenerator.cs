using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Indicates the generator that generates the methods about the equality checking. The methods below
	/// will be generated:
	/// <list type="bullet">
	/// <item><c>bool Equals(object? obj)</c></item>
	/// <item><c>bool Equals(T comparer)</c></item>
	/// <item><c>bool ==(T left, T right)</c></item>
	/// <item><c>bool !=(T left, T right)</c></item>
	/// </list>
	/// </summary>
	/// <remarks>
	/// Please note that if the type is a <see langword="ref struct"/>, the first one won't be generated
	/// because this method is useless in the by-ref-like types.
	/// </remarks>
	[Generator]
	public sealed partial class EqualsMethodGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
			Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
			var compilation = context.Compilation;
			var attributeSymbol = compilation.GetTypeByMetadataName<AutoEqualityAttribute>();
			foreach (var type in
				from candidate in receiver.Candidates
				let model = compilation.GetSemanticModel(candidate.SyntaxTree)
				select (INamedTypeSymbol)model.GetDeclaredSymbol(candidate)! into type
				where type.GetAttributes().Any(a => f(a.AttributeClass, attributeSymbol))
				select type)
			{
				if (type.GetAttributeString(attributeSymbol) is not { } attributeStr)
				{
					continue;
				}

				int tokenStartIndex = attributeStr.IndexOf("({");
				if (tokenStartIndex == -1)
				{
					continue;
				}

				if (attributeStr.GetMemberValues(tokenStartIndex) is not { Length: not 0 } members)
				{
					continue;
				}

				type.DeconstructInfo(
					false, out string fullTypeName, out string namespaceName, out string genericParametersList,
					out string genericParametersListWithoutConstraint, out string typeKind,
					out string readonlyKeyword, out _
				);
				string inKeyword = type.TypeKind == TypeKind.Struct ? "in " : string.Empty;
				string nullableAnnotation = type.TypeKind == TypeKind.Class ? "?" : string.Empty;
				string nullCheck = type.TypeKind == TypeKind.Class ? "other is not null && " : string.Empty;
				string memberCheck = string.Join(" && ", from member in members select $"{member} == other.{member}");

				string typeName = type.Name;
				string objectEqualsMethod = type.IsRefLikeType
					? "// This type is a ref struct, so 'bool Equals(object?) is useless."
					: $@"[CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override {readonlyKeyword}bool Equals(object? other) => other is {typeName}{genericParametersList} comparer && Equals(comparer);";

				string specifyEqualsMethod = $@"[CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public {readonlyKeyword}bool Equals({inKeyword}{typeName}{genericParametersListWithoutConstraint}{nullableAnnotation} other) => {nullCheck}{memberCheck};";

				var memberSymbols = type.GetMembers().OfType<IMethodSymbol>();
				string opEquality = isOp(memberSymbols, OperatorNames.Equality)
					? $@"[CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==({inKeyword}{typeName}{genericParametersListWithoutConstraint} left, {inKeyword}{typeName}{genericParametersListWithoutConstraint} right) => left.Equals(right);"
					: "// 'operator ==' does exist in the type.";

				string opInequality = isOp(memberSymbols, OperatorNames.Inequality)
					? $@"[CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=({inKeyword}{typeName}{genericParametersListWithoutConstraint} left, {inKeyword}{typeName}{genericParametersListWithoutConstraint} right) => !(left == right);"
					: "// 'operator !=' does exist in the type.";

				context.AddSource(
					type.ToFileName(),
					"Equality",
					$@"#pragma warning disable 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial {typeKind}{typeName}{genericParametersList}
	{{
		{objectEqualsMethod}

		{specifyEqualsMethod}


		{opEquality}

		{opInequality}
	}}
}}"
				);


				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				static bool isOp(IEnumerable<IMethodSymbol> methods, string operatorName) =>
					methods.All(method => method.Name != operatorName);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
	}
}
