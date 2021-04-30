#pragma warning disable IDE0057

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGen.Equality.Extensions;

namespace Sudoku.CodeGen.Equality
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
	public sealed partial class AutoEqualsMethodGenerator : ISourceGenerator
	{
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

				if (getEqualityMethodsCode(context, classSymbol) is { } c)
				{
					context.AddSource($"{name}.Equality.g.cs", c);
				}
			}


			static IEnumerable<INamedTypeSymbol> g(in GeneratorExecutionContext context, SyntaxReceiver receiver)
			{
				var compilation = context.Compilation;

				return
					from candidate in receiver.Candidates
					let model = compilation.GetSemanticModel(candidate.SyntaxTree)
					select model.GetDeclaredSymbol(candidate)! into symbol
					where symbol.Marks<AutoEqualityAttribute>()
					select (INamedTypeSymbol)symbol;
			}

			static string? getEqualityMethodsCode(in GeneratorExecutionContext context, INamedTypeSymbol symbol)
			{
				string attributeStr = (
					from attribute in symbol.GetAttributes()
					where attribute.AttributeClass?.Name == nameof(AutoEqualityAttribute)
					select attribute
				).First().ToString();
				int tokenStartIndex = attributeStr.IndexOf("({");
				if (tokenStartIndex == -1)
				{
					return null;
				}

				string[] members = (
					from parameterValue in attributeStr.Substring(
						tokenStartIndex,
						attributeStr.Length - tokenStartIndex - 2
					).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
					select parameterValue.Substring(1, parameterValue.Length - 2)
				).ToArray(); // Remove quote token '"'.
				if (members is not { Length: not 0 })
				{
					return null;
				}

				members[0] = members[0].Substring(2); // Remove token '{"'.

				string namespaceName = symbol.ContainingNamespace.ToDisplayString();
				string fullTypeName = symbol.ToDisplayString(FormatOptions.TypeFormat);
				int i = fullTypeName.IndexOf('<');
				string genericParametersList = i == -1 ? string.Empty : fullTypeName.Substring(i);
				int j = fullTypeName.IndexOf('>');
				string genericParametersListWithoutConstraint = i == -1 ? string.Empty : fullTypeName.Substring(i, j - i + 1);

				string typeKind = symbol switch
				{
					{ IsRecord: true } => "record",
					{ TypeKind: TypeKind.Class } => "class",
					{ TypeKind: TypeKind.Struct } => "struct"
				};

				string readonlyKeyword = symbol is { TypeKind: TypeKind.Struct, IsReadOnly: false } ? "readonly " : string.Empty;
				string inKeyword = symbol.TypeKind == TypeKind.Struct ? "in " : string.Empty;
				string nullableAnnotation = symbol.TypeKind == TypeKind.Class ? "?" : string.Empty;
				string nullCheck = symbol.TypeKind == TypeKind.Class ? "other is not null && " : string.Empty;
				string memberCheck = string.Join(" && ", from member in members select $"{member} == other.{member}");

				string objectEqualsMethod = symbol.IsRefLikeType
					? "// This type is a ref struct, so 'bool Equals(object?) is useless."
					: $@"[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override {readonlyKeyword}bool Equals(object? other) => other is {symbol.Name}{genericParametersList} comparer && Equals(comparer);";

				string specifyEqualsMethod = $@"[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public {readonlyKeyword}bool Equals({inKeyword}{symbol.Name}{genericParametersListWithoutConstraint}{nullableAnnotation} other) => {nullCheck}{memberCheck};";

				string opEqualityMethod = symbol.GetMembers().OfType<IMethodSymbol>().All(static m => m.Name != "op_Equality")
					? $@"[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==({inKeyword}{symbol.Name}{genericParametersListWithoutConstraint} left, {inKeyword}{symbol.Name}{genericParametersListWithoutConstraint} right) => left.Equals(right);"
					: "// 'operator ==' does exist in the type.";

				string opInequalityMethod = symbol.GetMembers().OfType<IMethodSymbol>().All(static m => m.Name != "op_Inequality")
					? $@"[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=({inKeyword}{symbol.Name}{genericParametersListWithoutConstraint} left, {inKeyword}{symbol.Name}{genericParametersListWithoutConstraint} right) => !(left == right);"
					: "// 'operator !=' does exist in the type.";

				return $@"#pragma warning disable 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial {typeKind} {symbol.Name}{genericParametersList}
	{{
		{objectEqualsMethod}

		{specifyEqualsMethod}


		{opEqualityMethod}

		{opInequalityMethod}
	}}
}}";
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) =>
			context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());


		/// <summary>
		/// Try to get all possible fields or properties in the specified <see langword="class"/> type.
		/// </summary>
		/// <param name="symbol">The specified class symbol.</param>
		/// <param name="handleRecursively">
		/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.</param>
		/// <returns>The result list that contains all member symbols.</returns>
		private static IReadOnlyList<string> GetMembers(INamedTypeSymbol symbol, bool handleRecursively)
		{
			var result = new List<string>(
				(
					from x in symbol.GetMembers().OfType<IFieldSymbol>()
					select x.Name
				).Concat(
					from x in symbol.GetMembers().OfType<IPropertySymbol>()
					select x.Name
				)
			);

			if (handleRecursively && symbol.BaseType is { } baseType && baseType.Marks<AutoEqualityAttribute>())
			{
				result.AddRange(GetMembers(baseType, handleRecursively: true));
			}

			return result;
		}
	}
}
