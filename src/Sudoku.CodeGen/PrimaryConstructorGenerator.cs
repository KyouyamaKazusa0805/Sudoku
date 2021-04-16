#pragma warning disable IDE0057

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.CodeGen.Annotations;
using Sudoku.CodeGen.Extensions;
using GenericsOptions = Microsoft.CodeAnalysis.SymbolDisplayGenericsOptions;
using MiscellaneousOptions = Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions;
using TypeQualificationStyle = Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

namespace Sudoku.CodeGen
{
	/// <summary>
	/// Indicates a generator that generates primary constructors for <see langword="class"/>es
	/// when they're marked <see cref="AutoGeneratePrimaryConstructorAttribute"/>.
	/// </summary>
	/// <seealso cref="AutoGeneratePrimaryConstructorAttribute"/>
	[Generator]
	public sealed partial class PrimaryConstructorGenerator : ISourceGenerator
	{
		/// <summary>
		/// Indicates the type format, and the property type format.
		/// </summary>
		private static readonly SymbolDisplayFormat
			TypeFormat = new(
				globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
				typeQualificationStyle: TypeQualificationStyle.NameAndContainingTypesAndNamespaces,
				genericsOptions: GenericsOptions.IncludeTypeParameters | GenericsOptions.IncludeTypeConstraints,
				miscellaneousOptions:
					MiscellaneousOptions.UseSpecialTypes
					| MiscellaneousOptions.EscapeKeywordIdentifiers
					| MiscellaneousOptions.IncludeNullableReferenceTypeModifier
			),
			PropertyTypeFormat = new(
				globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
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

			var classNameDic = new Dictionary<string, int>();
			foreach (var classSymbol in g(context, receiver))
			{
				_ = classNameDic.TryGetValue(classSymbol.Name, out int i);
				var name = i == 0 ? classSymbol.Name : $"{classSymbol.Name}{i + 1}";
				classNameDic[classSymbol.Name] = i + 1;
				context.AddSource($"{name}.PrimaryConstructor.PrimaryConstructor.cs", getPrimaryConstructorCode(classSymbol));
			}


			static IEnumerable<INamedTypeSymbol> g(GeneratorExecutionContext context, SyntaxReceiver receiver)
			{
				var compilation = context.Compilation;

				return from candidateClass in receiver.CandidateClasses
					   let model = compilation.GetSemanticModel(candidateClass.SyntaxTree)
					   select model.GetDeclaredSymbol(candidateClass)! into classSymbol
					   where HasMarked(classSymbol, nameof(AutoGeneratePrimaryConstructorAttribute))
					   select (INamedTypeSymbol)classSymbol;
			}

			static string getPrimaryConstructorCode(INamedTypeSymbol classSymbol)
			{
				string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

				var baseClassConstructorArgs =
					classSymbol.BaseType is { } baseType
					&& HasMarked(baseType, nameof(AutoGeneratePrimaryConstructorAttribute))
					? GetMembers(baseType, handleRecursively: true)
					: null;
				string baseConstructorInheritance = baseClassConstructorArgs is { Count: not 0 }
					? $" : base({string.Join(", ", from x in baseClassConstructorArgs select x.ParameterName)})"
					: string.Empty;

				var members = GetMembers(classSymbol, handleRecursively: false);
				var arguments =
					from x in baseClassConstructorArgs is null ? members : members.Concat(baseClassConstructorArgs)
					select $"{x.Type} {x.ParameterName}";
				string fullTypeName = classSymbol.ToDisplayString(TypeFormat);
				int i = fullTypeName.IndexOf('<');
				string generic = i < 0 ? string.Empty : fullTypeName.Substring(i);

#pragma warning disable IDE0055
				// Code generating.
				// Here the code may be ugly...
				var source = new StringBuilder(
					$@"#pragma warning disable 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
    partial class {classSymbol.Name}{generic}
    {{
        [CompilerGenerated]
        public {classSymbol.Name}({string.Join(", ", arguments)}){baseConstructorInheritance}
        {{");

				foreach (var item in members)
				{
					source.Append(
						$@"
            this.{item.Name} = {item.ParameterName};");
				}

				source.Append(@"
        }
    }
}
");
#pragma warning restore IDE0055

				return source.ToString();
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) =>
			context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());


		/// <summary>
		/// To determine whether the symbol has marked the specified attribute.
		/// </summary>
		/// <param name="symbol">The symbol.</param>
		/// <param name="name">The attribute name marked.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		private static bool HasMarked(ISymbol symbol, string name) =>
			symbol.GetAttributes().Any(x => x.AttributeClass?.Name == name);

		/// <summary>
		/// Try to get all possible fields or properties in the specified <see langword="class"/> type.
		/// </summary>
		/// <param name="classSymbol">The specified class symbol.</param>
		/// <param name="handleRecursively">
		/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.</param>
		/// <returns>The result list that contains all member symbols.</returns>
		private static IReadOnlyList<SymbolInfo> GetMembers(
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
