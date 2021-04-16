#pragma warning disable IDE0057

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.CodeGen.PrimaryConstructor.Annotations;
using Sudoku.CodeGen.PrimaryConstructor.Extensions;
using GenericsOptions = Microsoft.CodeAnalysis.SymbolDisplayGenericsOptions;
using GlobalNamespaceStyle = Microsoft.CodeAnalysis.SymbolDisplayGlobalNamespaceStyle;
using MiscellaneousOptions = Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions;
using TypeQualificationStyle = Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

namespace Sudoku.CodeGen.PrimaryConstructor
{
	partial class PrimaryConstructorGenerator
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


		/// <summary>
		/// To determine whether the symbol has marked the specified attribute.
		/// </summary>
		/// <param name="symbol">The symbol.</param>
		/// <param name="name">The attribute name marked.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		private static partial bool HasMarked(ISymbol symbol, string name) =>
			symbol.GetAttributes().Any(x => x.AttributeClass?.Name == name);

		/// <summary>
		/// Try to get all possible fields or properties in the specified <see langword="class"/> type.
		/// </summary>
		/// <param name="classSymbol">The specified class symbol.</param>
		/// <param name="handleRecursively">
		/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.</param>
		/// <returns>The result list that contains all member symbols.</returns>
		private static partial IReadOnlyList<SymbolInfo> GetMembers(
			INamedTypeSymbol classSymbol, bool handleRecursively)
		{
			var result = new List<SymbolInfo>(
				(
					from x in classSymbol.GetMembers().OfType<IFieldSymbol>()
					where x is { CanBeReferencedByName: true, IsStatic: false }
						&& (
							x.IsReadOnly
							&& !hasInitializer(x)
							|| HasMarked(x, nameof(IncludedMemberWhileGeneratingPrimaryConstructorAttribute))
						)
						&& !HasMarked(x, nameof(IgnoredMemberWhileGeneratingPrimaryConstructorAttribute))
					select new SymbolInfo(
						x.Type.ToDisplayString(PropertyTypeFormat),
						toCamelCase(x.Name),
						x.Name,
						x.GetAttributes()
					)
				).Concat(
					from x in classSymbol.GetMembers().OfType<IPropertySymbol>()
					where x is { CanBeReferencedByName: true, IsStatic: false }
						&& (
							x.IsReadOnly
							&&
							!hasInitializer(x)
							|| HasMarked(x, nameof(IncludedMemberWhileGeneratingPrimaryConstructorAttribute))
						) && !HasMarked(x, nameof(IgnoredMemberWhileGeneratingPrimaryConstructorAttribute))
					select new SymbolInfo(
						x.Type.ToDisplayString(PropertyTypeFormat),
						toCamelCase(x.Name),
						x.Name,
						x.GetAttributes()
					)
				)
			);

			if (handleRecursively
				&& classSymbol.BaseType is { } baseType
				&& HasMarked(baseType, nameof(AutoGeneratePrimaryConstructorAttribute)))
			{
				result.AddRange(GetMembers(baseType, true));
			}

			return result;


			static string toCamelCase(string name)
			{
				name = name.TrimStart('_');
				return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
			}

			static bool hasInitializer(ISymbol symbol) =>
				/*length-pattern*/
				symbol is { DeclaringSyntaxReferences: { Length: not 0 } list }
				&& list[0] is (_, syntaxNode: VariableDeclaratorSyntax { Initializer: not null });
		}
	}
}
