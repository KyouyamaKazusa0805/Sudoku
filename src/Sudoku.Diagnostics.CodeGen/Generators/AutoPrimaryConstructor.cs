namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for primary constructors.
/// </summary>
[Generator]
public sealed class AutoPrimaryConstructor : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		foreach (
			var (
				typeSymbol, accessibility, includes, excludes, (
					typeName, fullTypeName, namespaceName, genericParameterList, _, _, _, _, _, _
				), baseCtorInheritance, parameterList, memberAssignments
			) in ((Receiver)context.SyntaxContextReceiver!).Collection
		)
		{
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
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new Receiver(context.CancellationToken));


	private static IReadOnlyList<(string Type, string ParameterName, string Name)> GetMembers(
		INamedTypeSymbol type,
		bool recursively,
		IEnumerable<string> included,
		IEnumerable<string> excluded,
		CancellationToken cancellationToken
	)
	{
		var result = new List<(string, string, string)>(
			(
				from x in type.GetMembers().OfType<IFieldSymbol>()
				where
					x is { CanBeReferencedByName: true, IsStatic: false }
					&& (x.IsReadOnly && !x.HasInitializer(cancellationToken) || included.Contains(x.Name))
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
					&& (x.IsReadOnly && !x.HasInitializer(cancellationToken) || included.Contains(x.Name))
					&& !excluded.Contains(x.Name)
				select (
					x.Type.ToDisplayString(TypeFormats.FullName),
					x.Name.ToCamelCase(),
					x.Name
				)
			)
		);

		if (recursively && type.BaseType is { } baseType)
		{
			result.AddRange(GetMembers(baseType, true, included, excluded, cancellationToken));
		}

		return result;
	}


	/// <summary>
	/// Defines a syntax context receiver.
	/// </summary>
	/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
	private sealed record Receiver(CancellationToken CancellationToken) : IResultCollectionReceiver<AutoPrimaryConstructorInfo>
	{
		/// <inheritdoc/>
		public ICollection<AutoPrimaryConstructorInfo> Collection { get; } = new List<AutoPrimaryConstructorInfo>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			if (
				context is not
				{
					Node: TypeDeclarationSyntax { AttributeLists.Count: not 0 } n,
					SemanticModel: { Compilation: { } compilation } semanticModel
				}
			)
			{
				return;
			}

			if (semanticModel.GetDeclaredSymbol(n, CancellationToken) is not { } typeSymbol)
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

			var includes =
				from arg in
					attributeData.TryGetNamedArgument(
						nameof(AutoPrimaryConstructorAttribute.IncludedMemberNames),
						out var rawIncludes
					) ? rawIncludes.Values : ImmutableArray.Create<TypedConstant>()
				select (string)arg.Value!;
			var excludes =
				from arg in
					attributeData.TryGetNamedArgument(
						nameof(AutoPrimaryConstructorAttribute.ExcludedMemberNames),
						out var rawExcludes
					) ? rawExcludes.Values : ImmutableArray.Create<TypedConstant>()
				select (string)arg.Value!;
			var baseClassCtorArgs =
				typeSymbol.BaseType is { } baseType
					&& baseType.GetAttributes() is var baseTypeAttributesData
					&& baseTypeAttributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute))
						? GetMembers(baseType, true, includes, excludes, CancellationToken)
						: null;
			string? baseCtorInheritance = baseClassCtorArgs is not { Count: not 0 }
				? null
				: $" : base({string.Join(", ", from x in baseClassCtorArgs select x.ParameterName)})";

			var members = GetMembers(typeSymbol, false, includes, excludes, CancellationToken);
			string parameterList = string.Join(
				", ",
				from x in baseClassCtorArgs is null ? members : members.Concat(baseClassCtorArgs)
				select $"{x.Type} {x.ParameterName}"
			);
			string memberAssignments = string.Join(
				"\r\n\t\t\t",
				from member in members select $"{member.Name} = {member.ParameterName};"
			);

			// Please note that the value-tuple more than 8 elements must be as nested value-tuples
			// on the last element when displayed the full type name.
			// For example, a nonuple '(1, 2, 3, 4, 5, 6, 7, 8, 9)' will be displayed as
			//
			//     ValueTuple<int, int, int, int, int, int, int, ValueTuple<int, int>>
			//
			// in the generated code.
			// The type of the last element in the API will be constrainted as 'where T : struct',
			// because here the last element is a 'ValueTuple<...>', which is a struct.
			Collection.Add(
				(
					typeSymbol,
					attributeData.TryGetNamedArgument(
						nameof(AutoPrimaryConstructorAttribute.Accessibility),
						out var resultAccessibility
					) ? (MemberAccessibility)resultAccessibility.Value! : MemberAccessibility.Public,
					includes,
					excludes,
					SymbolOutputInfo.FromSymbol(typeSymbol),
					baseCtorInheritance,
					parameterList,
					memberAssignments
				)
			);
		}
	}
}
