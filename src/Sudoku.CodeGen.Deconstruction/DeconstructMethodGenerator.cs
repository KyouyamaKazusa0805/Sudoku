#pragma warning disable IDE0057

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sudoku.CodeGen.Deconstruction.Annotations;
using Sudoku.CodeGen.Deconstruction.Extensions;
using GenericsOptions = Microsoft.CodeAnalysis.SymbolDisplayGenericsOptions;
using GlobalNamespaceStyle = Microsoft.CodeAnalysis.SymbolDisplayGlobalNamespaceStyle;
using MiscellaneousOptions = Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions;
using TypeQualificationStyle = Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

namespace Sudoku.CodeGen.Deconstruction
{
	/// <summary>
	/// Provides a generator that generates the deconstruction methods.
	/// </summary>
	[Generator]
	public sealed partial class DeconstructMethodGenerator : ISourceGenerator
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
			),
			PropertyTypeFormat = new(
				globalNamespaceStyle: GlobalNamespaceStyle.OmittedAsContaining,
				typeQualificationStyle: TypeQualificationStyle.NameAndContainingTypesAndNamespaces,
				genericsOptions: GenericsOptions.IncludeTypeParameters,
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
			foreach (var typeSymbol in g(context, receiver))
			{
				_ = nameDic.TryGetValue(typeSymbol.Name, out int i);
				string name = i == 0 ? typeSymbol.Name : $"{typeSymbol.Name}{(i + 1).ToString()}";
				nameDic[typeSymbol.Name] = i + 1;
				context.AddSource($"{name}.DeconstructionMethods.g.cs", getDeconstructionCode(typeSymbol));
			}


			static IEnumerable<INamedTypeSymbol> g(in GeneratorExecutionContext context, SyntaxReceiver receiver)
			{
				var compilation = context.Compilation;

				return
					from candidateType in receiver.Candidates
					let model = compilation.GetSemanticModel(candidateType.SyntaxTree)
					select model.GetDeclaredSymbol(candidateType)! into typeSymbol
					where typeSymbol.Marks<AutoDeconstructAttribute>()
					select typeSymbol;
			}

			static string getDeconstructionCode(INamedTypeSymbol symbol)
			{
				string namespaceName = symbol.ContainingNamespace.ToDisplayString();
				var possibleArgs =
					from x in GetMembers(symbol, handleRecursively: false)
					select (Info: x, Param: $"out {x.Type} {x.ParameterName}");
				string fullTypeName = symbol.ToDisplayString(TypeFormat);
				int i = fullTypeName.IndexOf('<');
				string genericParametersList = i == -1 ? string.Empty : fullTypeName.Substring(i);
				string typeKind = symbol switch
				{
					{ IsRecord: true } => "record ",
					{ TypeKind: TypeKind.Class } => "class ",
					{ TypeKind: TypeKind.Struct } => "struct "
				};

				string methods = string.Join(
					"\r\n\r\n\t\t",
					from attribute in symbol.GetAttributes()
					where attribute.AttributeClass?.Name == nameof(AutoDeconstructAttribute)
					let attributeStr = attribute.ToString()
					let tokenStartIndex = attributeStr.IndexOf("({")
					where tokenStartIndex != -1
					let memberStrs = getMemberValues(attributeStr, tokenStartIndex)
					where !memberStrs.Any(member => possibleArgs.All(pair => pair.Info.Name != member))
					let readonlyKeyword = isNonReadonlyStruct(symbol) ? "readonly " : string.Empty
					let parameterList = string.Join(
						", ",
						from memberStr in memberStrs
						select possibleArgs.First(p => p.Info.Name == memberStr).Param
					)
					let assignments = string.Join(
						"\r\n\t\t\t",
						from member in memberStrs
						let paramName = possibleArgs.First(p => p.Info.Name == member).Info.ParameterName
						select $"{paramName} = {member};"
					)
					select $@"[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public {readonlyKeyword}void Deconstruct({parameterList})
		{{
			{assignments}
		}}"
				);

				return $@"#pragma warning disable 618
#pragma warning disable 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial {typeKind}{symbol.Name}{genericParametersList}
	{{
		{methods}
	}}
}}";

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				static bool isNonReadonlyStruct(INamedTypeSymbol symbol) =>
					symbol is { TypeKind: TypeKind.Struct, IsReadOnly: false };

				static string[] getMemberValues(string attributeStr, int tokenStartIndex)
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

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) =>
			context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());

		/// <summary>
		/// Try to get all possible fields or properties in the specified type.
		/// </summary>
		/// <param name="symbol">The specified symbol.</param>
		/// <param name="handleRecursively">
		/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.
		/// </param>
		/// <returns>The result list that contains all member symbols.</returns>
		private static IReadOnlyList<(string Type, string ParameterName, string Name, ImmutableArray<AttributeData> Attributes)> GetMembers(INamedTypeSymbol symbol, bool handleRecursively)
		{
			var result = new List<(string, string, string, ImmutableArray<AttributeData>)>(
				(
					from x in symbol.GetMembers().OfType<IFieldSymbol>()
					select (
						x.Type.ToDisplayString(PropertyTypeFormat),
						toCamelCase(x.Name),
						x.Name,
						x.GetAttributes()
					)
				).Concat(
					from x in symbol.GetMembers().OfType<IPropertySymbol>()
					select (
						x.Type.ToDisplayString(PropertyTypeFormat),
						toCamelCase(x.Name),
						x.Name,
						x.GetAttributes()
					)
				)
			);

			if (handleRecursively
				&& symbol.BaseType is { } baseType && baseType.Marks<AutoDeconstructAttribute>())
			{
				result.AddRange(GetMembers(baseType, true));
			}

			return result;


			static string toCamelCase(string name)
			{
				name = name.TrimStart('_');
				return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
			}
		}
	}
}
