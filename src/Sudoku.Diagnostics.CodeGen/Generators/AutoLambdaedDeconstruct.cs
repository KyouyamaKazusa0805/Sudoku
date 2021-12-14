namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the deconstruction methods with expressions.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoLambdaedDeconstruct : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var compilation = context.Compilation;
		foreach (
			var (
				typeSymbol, attributesData, (
					typeName, fullTypeName, namespaceName, genericParameterList, genericParameterListWithoutConstraint,
					typeKind, readOnlyKeyword, inKeyword, nullableAnnotation, _
				), possibleMembers
			) in ((Receiver)context.SyntaxContextReceiver!).Collection
		)
		{
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
	[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
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

partial {typeKind}{typeName}{genericParameterList}
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
		context.RegisterForSyntaxNotifications(() => new Receiver(context.CancellationToken));


	/// <summary>
	/// Defines a syntax context receiver.
	/// </summary>
	/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
	private sealed record Receiver(CancellationToken CancellationToken) : IResultCollectionReceiver<AutoLambdaedDeconstructInfo>
	{
		/// <inheritdoc/>
		public ICollection<AutoLambdaedDeconstructInfo> Collection { get; } = new List<AutoLambdaedDeconstructInfo>();


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

			Collection.Add(
				(
					typeSymbol,
					attributesData,
					SymbolOutputInfo.FromSymbol(typeSymbol),
					(
						from typeDetail in MemberDetail.GetDetailList(typeSymbol, attribute, false)
						select typeDetail
					).ToArray()
				)
			);
		}
	}
}
