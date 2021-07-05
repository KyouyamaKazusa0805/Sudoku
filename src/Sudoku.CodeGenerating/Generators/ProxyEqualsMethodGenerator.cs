using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Indicates a generator that generates the code about the equality method.
	/// </summary>
	[Generator]
	public sealed partial class ProxyEqualsMethodGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
			var processedList = new List<INamedTypeSymbol>();
			var compilation = context.Compilation;
			foreach (var symbol in
				from candidate in receiver.Candidates
				let model = compilation.GetSemanticModel(candidate.SyntaxTree)
				select (INamedTypeSymbol)model.GetDeclaredSymbol(candidate)! into symbol
				from member in symbol.GetMembers().OfType<IMethodSymbol>()
				where member.Marks<ProxyEqualityAttribute>()
				let boolSymbol = compilation.GetSpecialType(SpecialType.System_Boolean)
				let returnTypeSymbol = member.ReturnType
				where SymbolEqualityComparer.Default.Equals(returnTypeSymbol, boolSymbol)
				let parameters = member.Parameters
				where parameters.Length == 2 && parameters.All(p => SymbolEqualityComparer.Default.Equals(p.Type, symbol))
				select symbol)
			{
				if (processedList.Contains(symbol, SymbolEqualityComparer.Default))
				{
					continue;
				}

				var attributeSymbol = compilation.GetTypeByMetadataName(typeof(ProxyEqualityAttribute).FullName);
				var methods = (
					from member in symbol.GetMembers().OfType<IMethodSymbol>()
					from attribute in member.GetAttributes()
					where SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol)
					select member
				).First();

				/*slice-pattern*/
				if (
					symbol.IsReferenceType
					&& !methods.Parameters.NullableMatches(NullableAnnotation.Annotated, NullableAnnotation.Annotated)
				)
				{
					continue;
				}

				symbol.DeconstructInfo(
					false, out string fullTypeName, out string namespaceName, out string genericParametersList,
					out string genericParametersListWithoutConstraint, out string typeKind,
					out string readonlyKeyword, out _
				);
				string methodName = methods.Name;
				string inModifier = symbol.MemberShouldAppendIn() ? "in " : string.Empty;
				string nullableMark = symbol.TypeKind == TypeKind.Class || symbol.IsRecord ? "?" : string.Empty;
				string objectEqualityMethod = symbol.IsRefLikeType
					? "// This type is a ref struct, so 'bool Equals(object?) is useless."
					: $@"[CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override {readonlyKeyword}bool Equals(object? obj) => obj is {symbol.Name}{genericParametersListWithoutConstraint} comparer && {methodName}(this, comparer);";

				context.AddSource(
					symbol.ToFileName(),
					"ProxyEquality",
					$@"#pragma warning disable 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial {typeKind}{symbol.Name}{genericParametersList}
	{{
		{objectEqualityMethod}

		[CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals({inModifier}{symbol.Name}{genericParametersListWithoutConstraint}{nullableMark} other) => {methodName}(this, other);


		[CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==({inModifier}{symbol.Name}{genericParametersListWithoutConstraint} left, {inModifier}{symbol.Name}{genericParametersListWithoutConstraint} right) => {methodName}(left, right);

		[CompilerGenerated, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=({inModifier}{symbol.Name}{genericParametersListWithoutConstraint} left, {inModifier}{symbol.Name}{genericParametersListWithoutConstraint} right) => !(left == right);
	}}
}}");

				processedList.Add(symbol);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
	}
}
