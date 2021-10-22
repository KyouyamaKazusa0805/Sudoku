namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the deconstruction methods with expressions.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoLambdaedDeconstruct : ISourceGenerator
{
	/// <summary>
	/// The result collection.
	/// </summary>
	private readonly ICollection<(INamedTypeSymbol Symbol, IEnumerable<AttributeData> AttributesData)> _resultCollection =
		new List<(INamedTypeSymbol, IEnumerable<AttributeData>)>();


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var compilation = context.Compilation;
		var attribute = compilation.GetTypeByMetadataName(typeof(AutoDeconstructAttribute).FullName);
		foreach (var (typeSymbol, attributesData) in _resultCollection)
		{
			var (
				typeName, fullTypeName, namespaceName, genericParameterList, genericParameterListWithoutConstraint,
				typeKind, readOnlyKeyword, inKeyword, nullableAnnotation, _
			) = SymbolOutputInfo.FromSymbol(typeSymbol);
			var possibleMembers = (
				from typeDetail in MemberDetail.GetDetailList(typeSymbol, attribute, false)
				select typeDetail
			).ToArray();

			string methods = string.Join(
				"\r\n\r\n\t",
				from attributeData in attributesData
				let memberArgs = attributeData.ConstructorArguments[0].Values
				select (from memberArg in memberArgs select ((string)memberArg.Value!).Trim()) into members
				where members.All(m => possibleMembers.Any(p => p.Name == m))
				let details = from m in members select possibleMembers.First(p => p.Name == m)
				let deprecatedTypeNames = (
					from detail in details
					let tempTypeName = detail.FullTypeName
					where !KeywordsToBclNames.ContainsKey(tempTypeName)
					let tempSymbol = detail.TypeSymbol
					where tempSymbol.CheckAnyTypeArgumentIsMarked<ObsoleteAttribute>(compilation)
					select $"'{tempTypeName}'"
				).ToArray()
				let obsoleteAttributeStr = deprecatedTypeNames.Length switch
				{
					0 => string.Empty,
					1 => $"\r\n\t[global::System.Obsolete(\"The method is deprecated because the inner type {deprecatedTypeNames[0]} is deprecated.\", false)]",
					> 1 => $"\r\n\t[global::System.Obsolete(\"The method is deprecated because the inner types {string.Join(", ", deprecatedTypeNames)} are deprecated.\", false)]",
				}
				let paramNames = from paramInfo in details select paramInfo.OutParameterDeclaration
				let paramNamesStr = string.Join(", ", paramNames)
				let assignments =
					from m in members
					let possibleMemberInfo = possibleMembers.First(p => p.Name == m)
					let parameterName = possibleMemberInfo.Name.ToCamelCase()
					let expression = getExpression(possibleMemberInfo.MemberSymbol, context.CancellationToken)
					select $"{parameterName} = {expression ?? m};"
				let assignmentsStr = string.Join("\r\n\t\t", assignments)
				select $@"/// <summary>
	/// Deconstruct the instance into multiple elements.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]{obsoleteAttributeStr}
	public {readOnlyKeyword}void Deconstruct({paramNamesStr})
	{{
		{assignmentsStr}
	}}"
			);

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.LambdaedDeconstructionMethod,
				$@"#nullable enable

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParameterList}
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
				IPropertySymbol { GetMethod: not null } p when !p.IsAutoImplemented() && p.Locations[0] is { SourceTree: { } st } l =>
					(PropertyDeclarationSyntax?)st.GetRoot(ct)?.FindNode(l.SourceSpan) switch
					{
						// public int Property => value;
						{ ExpressionBody.Expression: var expressionNode } =>
							expressionNode.ToString(),

						{ AccessorList.Accessors: { Count: not 0 } a } when a.FirstOrDefault(isGetKeyword) is { } getAccessor =>
							getAccessor switch
							{
								// public int Property { get { return value; } }
								{ Body.Statements: { Count: 1 } statements } when statements[0] is ReturnStatementSyntax
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool isGetKeyword(AccessorDeclarationSyntax a) => a is { Keyword.ValueText: "get" };
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(
			() => SyntaxContextReceiverCreator.Create(
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

					var attribute = compilation.GetTypeByMetadataName(typeof(AutoDeconstructLambdaAttribute).FullName)!;
					var attributesData =
						from attributeData in typeSymbol.GetAttributes()
						where !attributeData.ConstructorArguments.IsDefaultOrEmpty
						let tempSymbol = attributeData.AttributeClass
						where SymbolEqualityComparer.Default.Equals(tempSymbol, attribute)
						select attributeData;
					if (!attributesData.Any())
					{
						return;
					}

					_resultCollection.Add((typeSymbol, attributesData));
				}
			)
		);
}
