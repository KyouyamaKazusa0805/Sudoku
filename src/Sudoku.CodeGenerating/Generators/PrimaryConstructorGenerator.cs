namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Indicates a generator that generates primary constructors for <see langword="class"/>es
/// when they're marked <see cref="AutoPrimaryConstructorAttribute"/>.
/// </summary>
/// <remarks>
/// This generator can <b>only</b> support non-nested <see langword="class"/>es.
/// </remarks>
/// <seealso cref="AutoPrimaryConstructorAttribute"/>
[Generator]
public sealed partial class PrimaryConstructorGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
		var compilation = context.Compilation;
		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(AutoPrimaryConstructorAttribute).FullName);
		foreach (var (typeSymbol, accessibility, includes, excludes) in
			from type in receiver.CandidateClasses
			let model = compilation.GetSemanticModel(type.SyntaxTree)
			select model.GetDeclaredSymbol(type)! into typeSymbol
			let attributesData = typeSymbol.GetAttributes()
			let attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol))
			where attributeData is not null
			let accessibility = attributeData.TryGetNamedArgument(nameof(AutoPrimaryConstructorAttribute.Accessibility), out var result) ? (MemberAccessibility)result.Value! : MemberAccessibility.Public
			let includedMemberArg = attributeData.TryGetNamedArgument(nameof(AutoPrimaryConstructorAttribute.IncludedMemberNames), out var result) ? result.Values : ImmutableArray.Create<TypedConstant>()
			let included = from arg in includedMemberArg select (string)arg.Value!
			let excludedMemberArg = attributeData.TryGetNamedArgument(nameof(AutoPrimaryConstructorAttribute.ExcludedMemberNames), out var result) ? result.Values : ImmutableArray.Create<TypedConstant>()
			let excluded = from arg in excludedMemberArg select (string)arg.Value!
			select (typeSymbol, accessibility, included, excluded))
		{
			typeSymbol.DeconstructInfo(
				false, out string fullTypeName, out string namespaceName, out string genericParametersList,
				out _, out _, out _, out _
			);

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

partial class {typeSymbol.Name}{genericParametersList}
{{
	/// <summary>
	/// Initializes an instance via those arguments.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Diagnostics.CodeAnalysis.PrimaryConstructor]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public {typeSymbol.Name}({parameterList}){baseCtorInheritance}
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
						&& (x.IsReadOnly && !x.HasInitializer(context.CancellationToken) || included.Contains(x.Name))
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
						&& (x.IsReadOnly && !x.HasInitializer(context.CancellationToken) || included.Contains(x.Name))
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
		context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());
}
