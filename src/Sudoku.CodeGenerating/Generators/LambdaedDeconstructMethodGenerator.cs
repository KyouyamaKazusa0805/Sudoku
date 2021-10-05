namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Defines a source generator that generates the deconstruction methods with expressions.
/// </summary>
[Generator]
public sealed partial class LambdaedDeconstructMethodGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
		var compilation = context.Compilation;
		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(AutoDeconstructLambdaAttribute).FullName)!;

		foreach (var (typeSymbol, attributesData) in
			from type in receiver.Candidates
			let model = compilation.GetSemanticModel(type.SyntaxTree)
			select model.GetDeclaredSymbol(type)! into typeSymbol
			let attributesData =
				from attributeData in typeSymbol.GetAttributes()
				where !attributeData.ConstructorArguments.IsDefaultOrEmpty
				let tempSymbol = attributeData.AttributeClass
				where SymbolEqualityComparer.Default.Equals(tempSymbol, attributeSymbol)
				select attributeData
			where attributesData.Any()
			select (typeSymbol, attributesData))
		{
			typeSymbol.DeconstructInfo(
				false, out string fullTypeName, out string namespaceName, out string genericParametersList,
				out _, out string typeKind, out string readonlyKeyword, out _
			);
			var possibleMembers = (
				from x in GetMembers(typeSymbol, attributeSymbol, false)
				select (Info: x, Param: $"out {x.Type} {x.ParameterName}")
			).ToArray();

			string methods = string.Join(
				"\r\n\r\n\t",
				from attributeData in attributesData
				let memberArgs = attributeData.ConstructorArguments[0].Values
				select (from memberArg in memberArgs select ((string)memberArg.Value!).Trim()) into members
				where members.All(m => possibleMembers.Any(p => p.Info.Name == m))
				let paramInfos = from m in members select possibleMembers.First(p => p.Info.Name == m)
				let deprecatedTypeNames = (
					from paramInfo in paramInfos
					select paramInfo.Info into info
					let tempTypeName = info.Type
					where !KeywordsToBclNames.ContainsKey(tempTypeName)
					let tempSymbol = info.TypeSymbol
					where tempSymbol.CheckAnyTypeArgumentIsMarked<ObsoleteAttribute>(compilation)
					select $"'{tempTypeName}'"
				).ToArray()
				let obsoleteAttributeStr = deprecatedTypeNames.Length switch
				{
					0 => string.Empty,
					1 => $"\r\n\t[global::System.Obsolete(\"The method is deprecated because the inner type {deprecatedTypeNames[0]} is deprecated.\", false)]",
					> 1 => $"\r\n\t[global::System.Obsolete(\"The method is deprecated because the inner types {string.Join(", ", deprecatedTypeNames)} are deprecated.\", false)]",
				}
				let paramNames = from paramInfo in paramInfos select paramInfo.Param
				let paramNamesStr = string.Join(", ", paramNames)
				let assignments =
					from m in members
					let possibleMemberInfo = possibleMembers.First(p => p.Info.Name == m).Info
					let parameterName = possibleMemberInfo.ParameterName
					let expression = getExpression(possibleMemberInfo.MemberSymbol, context.CancellationToken)
					select $"{parameterName} = {expression ?? m};"
				let assignmentsStr = string.Join("\r\n\t\t", assignments)
				select $@"/// <summary>
	/// Deconstruct the instance into multiple elements.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]{obsoleteAttributeStr}
	public {readonlyKeyword}void Deconstruct({paramNamesStr})
	{{
		{assignmentsStr}
	}}"
			);

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.LambdaedDeconstructionMethod,
				$@"#nullable enable

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParametersList}
{{
	{methods}
}}
"
			);
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string? getExpression(ISymbol fieldOrPropertySymbol, CancellationToken ct) =>
			fieldOrPropertySymbol switch
			{
				IPropertySymbol { GetMethod: not null } p
				when !p.IsAutoImplemented() && p.Locations[0] is { SourceTree: { } st } l =>
					(PropertyDeclarationSyntax?)st.GetRoot(ct)?.FindNode(l.SourceSpan) switch
					{
						// public int Property => value;
						{ ExpressionBody.Expression: var expressionNode } => expressionNode.ToString(),

						{ AccessorList.Accessors: { Count: not 0 } a }
						when a.FirstOrDefault(static a => a is { Keyword.ValueText: "get" }) is { } getAccessor =>
							getAccessor switch
							{
								// public int Property { get { return value; } }
								{Body.Statements: { Count: 1 } statements}
								when statements[0] is ReturnStatementSyntax
								{
									Expression: { } expressionNode
								} => expressionNode.ToString(),

								// public int Property { get => value; }
								{ ExpressionBody.Expression: var expressionNode } => expressionNode.ToString(),

								_ => null
							},
						_ => null
					},
				_ => null
			};
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();


	/// <summary>
	/// Try to get all possible fields or properties in the specified type.
	/// </summary>
	/// <param name="typeSymbol">The specified symbol.</param>
	/// <param name="attributeSymbol">The attribute symbol to check.</param>
	/// <param name="handleRecursively">
	/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.
	/// </param>
	/// <returns>The result list that contains all member symbols.</returns>
	private static IReadOnlyList<(string Type, string ParameterName, string Name, ISymbol MemberSymbol, INamedTypeSymbol? TypeSymbol, ImmutableArray<AttributeData> Attributes)> GetMembers(
		INamedTypeSymbol typeSymbol,
		INamedTypeSymbol? attributeSymbol,
		bool handleRecursively
	)
	{
		var result = new List<(string, string, string, ISymbol, INamedTypeSymbol?, ImmutableArray<AttributeData>)>(
			(
				from x in typeSymbol.GetMembers().OfType<IFieldSymbol>()
				select (
					x.Type.ToDisplayString(TypeFormats.FullName),
					x.Name.ToCamelCase(),
					x.Name,
					(ISymbol)x,
					x.Type as INamedTypeSymbol,
					x.GetAttributes()
				)
			).Concat(
				from x in typeSymbol.GetMembers().OfType<IPropertySymbol>()
				select (
					x.Type.ToDisplayString(TypeFormats.FullName),
					x.Name.ToCamelCase(),
					x.Name,
					(ISymbol)x,
					x.Type as INamedTypeSymbol,
					x.GetAttributes()
				)
			)
		);

		if (handleRecursively && typeSymbol.BaseType is { } baseType
			&& baseType.GetAttributes() is var attributesData
			&& attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)))
		{
			result.AddRange(GetMembers(baseType, attributeSymbol, true));
		}

		return result;
	}
}
