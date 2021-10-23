namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for primary constructors.
/// </summary>
[Generator]
public sealed class AutoPrimaryConstructor : ISourceGenerator
{
	/// <summary>
	/// The result collection.
	/// </summary>
	private readonly ICollection<(INamedTypeSymbol Symbol, MemberAccessibility Accessibility, IEnumerable<string> IncludedMembers, IEnumerable<string> ExcludedMembers)> _resultCollection =
		new List<(INamedTypeSymbol, MemberAccessibility, IEnumerable<string>, IEnumerable<string>)>();


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var compilation = context.Compilation;
		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(AutoPrimaryConstructorAttribute).FullName);
		foreach (var (typeSymbol, accessibility, includes, excludes) in _resultCollection)
		{
			var (
				typeName, fullTypeName, namespaceName, genericParameterList, _, _, _, _, _, _
			) = SymbolOutputInfo.FromSymbol(typeSymbol);

			var baseClassCtorArgs =
				typeSymbol.BaseType is { } baseType
				&& baseType.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol))
					? getMembers(baseType, true, includes, excludes)
					: null;

			string? baseCtorInheritance = baseClassCtorArgs is not { Count: not 0 }
				? null
				: $" : base({string.Join(", ", from x in baseClassCtorArgs select x.ParameterName)})";

			var members = getMembers(typeSymbol, false, includes, excludes);
			string parameterList = string.Join(
				", ",
				from x in baseClassCtorArgs is null ? members : members.Concat(baseClassCtorArgs)
				select $"{x.Type} {x.ParameterName}"
			);
			string memberAssignments = string.Join(
				"\r\n\t\t\t",
				from member in members select $"{member.Name} = {member.ParameterName};"
			);

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.PrimaryConstructorMethod,
				$@"#nullable enable

namespace {namespaceName};

partial class {typeName}{genericParameterList}
{{
	/// <summary>
	/// Initializes an instance via those arguments.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Diagnostics.CodeAnalysis.PrimaryConstructor]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public {typeName}({parameterList}){baseCtorInheritance}
	{{
		{memberAssignments}
	}}
}}
"
			);
		}


		IReadOnlyList<(string Type, string ParameterName, string Name)> getMembers(
			INamedTypeSymbol type,
			bool executeRecursively,
			IEnumerable<string> included,
			IEnumerable<string> excluded
		)
		{
			var result = new List<(string, string, string)>(
				(
					from x in type.GetMembers().OfType<IFieldSymbol>()
					where
						x is { CanBeReferencedByName: true, IsStatic: false }
						&& (
							x.IsReadOnly && !x.HasInitializer(context.CancellationToken)
							|| included.Contains(x.Name)
						)
						&& !excluded.Contains(x.Name)
					select (
						x.Type.ToDisplayString(TypeFormats.FullName),
						x.Name.ToCamelCase(),
						x.Name
					)
				).Concat(
					from x in type.GetMembers().OfType<IPropertySymbol>()
					where
						x is { CanBeReferencedByName: true, IsStatic: false }
						&& (
							x.IsReadOnly && !x.HasInitializer(context.CancellationToken)
							|| included.Contains(x.Name)
						)
						&& !excluded.Contains(x.Name)
					select (
						x.Type.ToDisplayString(TypeFormats.FullName),
						x.Name.ToCamelCase(),
						x.Name
					)
				)
			);

			if (executeRecursively && type.BaseType is { } baseType)
			{
				result.AddRange(getMembers(baseType, true, included, excluded));
			}

			return result;
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(
			() => new DefaultSyntaxContextReceiver(
				(syntaxNode, semanticModel) =>
				{
					if (
						(
							SyntaxNode: syntaxNode,
							SemanticModel: semanticModel,
							Context: context
						) is not (
							SyntaxNode: TypeDeclarationSyntax { AttributeLists.Count: not 0 } n,
							SemanticModel: { Compilation: { } compilation },
							Context: { CancellationToken: var cancellationToken }
						)
					)
					{
						return;
					}

					if (semanticModel.GetDeclaredSymbol(n, cancellationToken) is not { } typeSymbol)
					{
						return;
					}

					var attribute = compilation.GetTypeByMetadataName(typeof(AutoPrimaryConstructorAttribute).FullName)!;
					var attributesData = typeSymbol.GetAttributes();
					var attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute));
					if (attributeData is not { ConstructorArguments.IsDefaultOrEmpty: true })
					{
						return;
					}

					_resultCollection.Add(
						(
							Symbol: typeSymbol,
							Accessibility: attributeData.TryGetNamedArgument(
								nameof(AutoPrimaryConstructorAttribute.Accessibility),
								out var resultAccessibility
							) ? (MemberAccessibility)resultAccessibility.Value! : MemberAccessibility.Public,
							IncludedMembers:
								from arg in attributeData.TryGetNamedArgument(
									nameof(AutoPrimaryConstructorAttribute.IncludedMemberNames),
									out var rawIncludes
								) ? rawIncludes.Values : ImmutableArray.Create<TypedConstant>()
								select (string)arg.Value!,
							ExcludedMembers:
								from arg in attributeData.TryGetNamedArgument(
									nameof(AutoPrimaryConstructorAttribute.ExcludedMemberNames),
									out var rawExcludes
								) ? rawExcludes.Values : ImmutableArray.Create<TypedConstant>()
								select (string)arg.Value!
						)
					);
				}
			)
		);
}
