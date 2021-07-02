using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Indicates the generator that generates the code that overrides <see cref="object.GetHashCode"/>.
	/// </summary>
	/// <seealso cref="object.GetHashCode"/>
	[Generator]
	public sealed partial class AutoGetHashCodeGenerator : ISourceGenerator
	{
		/// <summary>
		/// Indicates the full type name of the attribute <see cref="AutoHashCodeAttribute"/>.
		/// </summary>
		/// <seealso cref="AutoHashCodeAttribute"/>
		private static readonly string AttributeFullTypeName = typeof(AutoHashCodeAttribute).FullName;


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;

			var nameDic = new Dictionary<string, int>();
			var compilation = context.Compilation;
			foreach (var classSymbol in
				from candidateClass in receiver.Candidates
				let model = compilation.GetSemanticModel(candidateClass.SyntaxTree)
				select (INamedTypeSymbol)model.GetDeclaredSymbol(candidateClass)! into symbol
				where symbol.Marks<AutoHashCodeAttribute>()
				select symbol)
			{
				_ = nameDic.TryGetValue(classSymbol.Name, out int i);
				string name = i == 0 ? classSymbol.Name : $"{classSymbol.Name}{(i + 1).ToString()}";
				nameDic[classSymbol.Name] = i + 1;

				if (getGetHashCodeCode(classSymbol) is { } c)
				{
					context.AddSource($"{name}.GetHashCode.g.cs", c);
				}
			}

			string? getGetHashCodeCode(INamedTypeSymbol symbol)
			{
				var attributeSymbol = compilation.GetTypeByMetadataName(AttributeFullTypeName)!;

				string attributeStr = (
					from attribute in symbol.GetAttributes()
					where SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol)
					select attribute
				).ToArray()[0].ToString();
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

				symbol.DeconstructInfo(
					true, out string fullTypeName, out string namespaceName, out string genericParametersList,
					out _, out string typeKind, out string readonlyKeyword, out _
				);
				string hashCodeStr = string.Join(" ^ ", from member in members select $"{member}.GetHashCode()");

				return $@"using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial {typeKind}{symbol.Name}{genericParametersList}
	{{
		/// <inheritdoc cref=""object.GetHashCode""/>
		[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override {readonlyKeyword}int GetHashCode() => {hashCodeStr};
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
			var members = symbol.GetMembers();
			var fieldElements = from x in members.OfType<IFieldSymbol>() select x.Name;
			var propertyElements = from x in members.OfType<IPropertySymbol>() select x.Name;
			var result = new List<string>(fieldElements.Concat(propertyElements));

			if (handleRecursively && symbol.BaseType is { } baseType && baseType.Marks<AutoHashCodeAttribute>())
			{
				result.AddRange(GetMembers(baseType, true));
			}

			return result;
		}
	}
}
