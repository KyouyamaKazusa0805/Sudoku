using System.Collections.Generic;
using System.Linq;
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
		/// <summary>
		/// Indicates the full type name of the attribute <see cref="AutoEqualityAttribute"/>.
		/// </summary>
		/// <seealso cref="AutoEqualityAttribute"/>
		private static readonly string AttributeFullTypeName = typeof(AutoEqualityAttribute).FullName;


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
			var nameDic = new Dictionary<string, int>();
			var compilation = context.Compilation;
			foreach (var classSymbol in
				from candidate in receiver.Candidates
				let model = compilation.GetSemanticModel(candidate.SyntaxTree)
				select (INamedTypeSymbol)model.GetDeclaredSymbol(candidate)! into symbol
				where symbol.Marks<AutoEqualityAttribute>()
				select symbol)
			{
				_ = nameDic.TryGetValue(classSymbol.Name, out int i);
				string name = i == 0 ? classSymbol.Name : $"{classSymbol.Name}{(i + 1).ToString()}";
				nameDic[classSymbol.Name] = i + 1;

				if (getEqualityMethodsCode(context, classSymbol) is { } c)
				{
					context.AddSource($"{name}.Equality.g.cs", c);
				}
			}


			string? getEqualityMethodsCode(in GeneratorExecutionContext context, INamedTypeSymbol symbol)
			{
				var attributeSymbol = compilation.GetTypeByMetadataName(AttributeFullTypeName);
				if (symbol.GetAttributeString(attributeSymbol) is not { } attributeStr)
				{
					return null;
				}

				if (attributeStr.IndexOf("({") is var tokenStartIndex && tokenStartIndex == -1)
				{
					return null;
				}

				if (attributeStr.GetMemberValues(tokenStartIndex) is not { Length: not 0 } members)
				{
					return null;
				}

				symbol.DeconstructInfo(
					false, out string fullTypeName, out string namespaceName, out string genericParametersList,
					out string genericParametersListWithoutConstraint, out string typeKind,
					out string readonlyKeyword, out _
				);
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

				var memberSymbols = symbol.GetMembers();
				string opEqualityMethod = memberSymbols.OfType<IMethodSymbol>().All(static m => m.Name != "op_Equality")
					? $@"[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==({inKeyword}{symbol.Name}{genericParametersListWithoutConstraint} left, {inKeyword}{symbol.Name}{genericParametersListWithoutConstraint} right) => left.Equals(right);"
					: "// 'operator ==' does exist in the type.";

				string opInequalityMethod = memberSymbols.OfType<IMethodSymbol>().All(static m => m.Name != "op_Inequality")
					? $@"[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=({inKeyword}{symbol.Name}{genericParametersListWithoutConstraint} left, {inKeyword}{symbol.Name}{genericParametersListWithoutConstraint} right) => !(left == right);"
					: "// 'operator !=' does exist in the type.";

				return $@"#pragma warning disable 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial {typeKind}{symbol.Name}{genericParametersList}
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
		public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();


		/// <summary>
		/// Try to get all possible fields or properties in the specified <see langword="class"/> type.
		/// </summary>
		/// <param name="symbol">The specified class symbol.</param>
		/// <param name="handleRecursively">
		/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.</param>
		/// <returns>The result list that contains all member symbols.</returns>
		private static IReadOnlyList<string> GetMembers(INamedTypeSymbol symbol, bool handleRecursively)
		{
			var fieldMembers = from x in symbol.GetMembers().OfType<IFieldSymbol>() select x.Name;
			var propertyMembers = from x in symbol.GetMembers().OfType<IPropertySymbol>() select x.Name;
			var result = new List<string>(fieldMembers.Concat(propertyMembers));

			if (handleRecursively && symbol.BaseType is { } baseType && baseType.Marks<AutoEqualityAttribute>())
			{
				result.AddRange(GetMembers(baseType, handleRecursively: true));
			}

			return result;
		}
	}
}
