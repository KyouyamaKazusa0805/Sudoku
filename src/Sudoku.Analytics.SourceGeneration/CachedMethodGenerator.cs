namespace Sudoku.SourceGeneration;

/// <summary>
/// Represents a source generator that copies the whole method, and replaces some variables with cached fields.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class CachedMethodGenerator : IIncrementalGenerator
{
	private const string InterceptorMethodCallerAttributeTypeFullName = "Sudoku.Runtime.InterceptorServices.InterceptorMethodCallerAttribute";

	private const string CachedAttributeTypeFullName = "Sudoku.Runtime.InterceptorServices.CachedAttribute";

	private const string GeneratedNamespace = "Sudoku.Runtime.InterceptorServices.Generated";

	private const string InterceptsLocationAttributeFullName = "System.Runtime.CompilerServices.InterceptsLocationAttribute";

	private const string CommentLineBegin = "// --INTERCEPTOR_VARIABLE_DECLARATION_BEGIN--";

	private const string CommentLineEnd = "// --INTERCEPTOR_VARIABLE_DECLARATION_END--";

	private const string AttributeInsertionMatchString = "<<insert-here>>";


	private static readonly string[] ValidVariableNames = [
		"__EmptyCells",
		"__BivalueCells",
		"__CandidatesMap",
		"__DigitsMap",
		"__ValuesMap"
	];

	/// <summary>
	/// Represents message "Method marked '[InterceptorMethodCaller]' can only be block body".
	/// </summary>
	private static readonly DiagnosticDescriptor Descriptor_Interceptor0101 = new(
		"INTERCEPTOR0101",
		"Method marked '[InterceptorMethodCaller]' can only be block body",
		"Method '{0}' marked '[InterceptorMethodCaller]' can only be block body",
		"Interceptor.Design",
		DiagnosticSeverity.Error,
		true
	);

	/// <summary>
	/// Represents message "Method marked '[InterceptorMethodCaller]' requires at least one invocation expression
	/// that references to a method marked '[Cached]'".
	/// </summary>
	private static readonly DiagnosticDescriptor Descriptor_Interceptor0102 = new(
		"INTERCEPTOR0102",
		"Method marked '[InterceptorMethodCaller]' requires at least one invocation expression that references to a method marked '[Cached]'",
		"Method '{0}' marked '[InterceptorMethodCaller]' requires at least one invocation expression that references to a method marked '[Cached]'",
		"Interceptor.Design",
		DiagnosticSeverity.Error,
		true
	);

	/// <summary>
	/// Represents message "Method marked '[Cached]' cannot be <see langword="partial"/>".
	/// </summary>
	private static readonly DiagnosticDescriptor Descriptor_Interceptor0103 = new(
		"INTERCEPTOR0103",
		"Method marked '[Cached]' cannot be partial",
		"Method '{0}' marked '[Cached]' cannot be partial",
		"Interceptor.Design",
		DiagnosticSeverity.Error,
		true
	);

	/// <summary>
	/// Represents message "Lacks of usage of necessary comments:
	/// '<c>INTERCEPTOR_VARIABLE_DECLARATION_BEGIN</c>' and '<c>INTERCEPTOR_VARIABLE_DECLARATION_END</c>'".
	/// </summary>
	private static readonly DiagnosticDescriptor Diagnostic_Interceptor0104 = new(
		"INTERCEPTOR0104",
		"Lacks of usage of necessary comments: 'INTERCEPTOR_VARIABLE_DECLARATION_BEGIN' and 'INTERCEPTOR_VARIABLE_DECLARATION_END'",
		"Lacks of usage of necessary comments: 'INTERCEPTOR_VARIABLE_DECLARATION_BEGIN' and 'INTERCEPTOR_VARIABLE_DECLARATION_END'",
		"Interceptor.Design",
		DiagnosticSeverity.Error,
		true
	);

	/// <summary>
	/// Represents message "Duplicate comments:
	/// '<c>INTERCEPTOR_VARIABLE_DECLARATION_BEGIN</c>' and '<c>INTERCEPTOR_VARIABLE_DECLARATION_END</c>'".
	/// </summary>
	private static readonly DiagnosticDescriptor Diagnostic_Interceptor0105 = new(
		"INTERCEPTOR0105",
		"Duplicate comments: 'INTERCEPTOR_VARIABLE_DECLARATION_BEGIN' and 'INTERCEPTOR_VARIABLE_DECLARATION_END'",
		"Duplicate comments: 'INTERCEPTOR_VARIABLE_DECLARATION_BEGIN' and 'INTERCEPTOR_VARIABLE_DECLARATION_END'",
		"Interceptor.Design",
		DiagnosticSeverity.Error,
		true
	);


	/// <inheritdoc/>
	/// <remarks>
	/// This generator will aim to methods, to find whether a method block contains at least one intercepted usage.
	/// If so, such method will be replaced with cached method.
	/// The requirement is that, the callee method should be applied with '<c>[InterceptorMethodCaller]</c>'.
	/// </remarks>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName(
					InterceptorMethodCallerAttributeTypeFullName,
					static (node, _) => node is MethodDeclarationSyntax,
					Transform
				)
				.Where(static data => data is not null)
				.Select(static (data, _) => data!)
				.Collect(),
			Output
		);


	private static void Output(SourceProductionContext spc, ImmutableArray<TransformResult> transformResults)
	{
		var failed = new List<FailedTransformResult>();
		var success = new List<SuccessTransformResult>();
		foreach (var result in transformResults)
		{
			switch (result)
			{
				case FailedTransformResult f: { failed.Add(f); break; }
				case SuccessTransformResult s: { success.Add(s); break; }
			}
		}
		failed.ForEach(f => spc.ReportDiagnostic(f.Diagnostic));
		if (failed.Count != 0)
		{
			return;
		}

		var list = new List<string>();
		foreach (var resultGroup in from result in success group result by result.Text)
		{
			// Replace all with new values.
			var attribute = new List<string>();
			foreach (var (_, (filePath, lineNumber, characterNumber)) in resultGroup)
			{
				attribute.Add($""""[global::{InterceptsLocationAttributeFullName}("""{filePath}""", {lineNumber}, {characterNumber})]"""");
			}
			list.Add(resultGroup.Key.Replace(AttributeInsertionMatchString, string.Join("\r\n\t", attribute)));
		}

		spc.AddSource(
			"InterceptsLocationAttribute.cs",
			$$"""
			{{Banner.AutoGenerated}}

			#nullable enable

			namespace System.Runtime.CompilerServices;

			/// <summary>
			/// <para>
			/// Interceptors are an experimental compiler feature planned to ship in .NET 8 (with support for C# only).
			/// The feature may be subject to breaking changes or removal in a future release.
			/// </para>
			/// <para>
			/// An interceptor is a method which can declaratively substitute a call to an interceptable method with a call to itself at compile time.
			/// This substitution occurs by having the interceptor declare the source locations of the calls that it intercepts.
			/// This provides a limited facility to change the semantics of existing code by adding new code to a compilation (e.g. in a source generator).
			/// </para>
			/// </summary>
			/// <param name="filePath">The required file path.</param>
			/// <param name="line">The line.</param>
			/// <param name="character">The character position.</param>
			[global::System.AttributeUsageAttribute(global::System.AttributeTargets.Method, AllowMultiple = true)]
			[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(CachedMethodGenerator).FullName}}", "{{Value}}")]
			[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
			public sealed partial class InterceptsLocationAttribute(string filePath, int line, int character) : global::System.Attribute
			{
				/// <summary>
				/// The generated property declaration for parameter <c>filePath</c>.
				/// </summary>
				public string FilePath { get; } = filePath;

				/// <summary>
				/// The generated property declaration for parameter <c>line</c>.
				/// </summary>
				public int Line { get; } = line;

				/// <summary>
				/// The generated property declaration for parameter <c>character</c>.
				/// </summary>
				public int Character { get; } = character;
			}
			"""
		);

		spc.AddSource(
			"Intercepted.g.cs",
			$$"""
			{{Banner.AutoGenerated}}

			#nullable enable
			#pragma warning disable

			namespace {{GeneratedNamespace}};
			
			{{string.Join("\r\n\r\n", list)}}
			"""
		);
	}

	private static TransformResult? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken ct)
	{
#if DEBUG
		if (!System.Diagnostics.Debugger.IsAttached)
		{
			System.Diagnostics.Debugger.Launch();
		}
#endif

		if (gasc is not
			{
				TargetNode: MethodDeclarationSyntax
				{
					ExpressionBody: var expressionBody,
					Identifier: { ValueText: var methodName } identifierToken,
					//ConstraintClauses: var constraints
				} node,
				SemanticModel: { Compilation: var compilation } semanticModel
			})
		{
			return null;
		}

		if (expressionBody is not null)
		{
			return new FailedTransformResult(
				Diagnostic.Create(
					Descriptor_Interceptor0101,
					node.GetLocation(),
					messageArgs: [methodName]
				)
			);
		}

		var cachedAttributeSymbol = compilation.GetTypeByMetadataName(CachedAttributeTypeFullName);
		if (cachedAttributeSymbol is null)
		{
			return null;
		}

		foreach (var invocation in node.DescendantNodes().OfType<InvocationExpressionSyntax>())
		{
			var invocationLocation = default(Location);
			var referencedMethodDeclaration = default(MethodDeclarationSyntax);
			var referencedMethodSymbol = default(IMethodSymbol);

			var invocationExpression = invocation.Expression;
			var symbolInfo = semanticModel.GetSymbolInfo(invocation, ct);
			switch (symbolInfo)
			{
				case { CandidateSymbols: [IMethodSymbol { DeclaringSyntaxReferences: var syntaxRefs } methodSymbol] }
				when methodSymbol.GetAttributes().Any(cachedAttributeChecker):
				{
					if (syntaxRefs is not [var syntaxRef])
					{
						return new FailedTransformResult(
							Diagnostic.Create(
								Descriptor_Interceptor0103,
								Location.Create(syntaxRefs[0].SyntaxTree, syntaxRefs[0].Span),
								messageArgs: [invocationExpression.ToString()]
							)
						);
					}

					invocationLocation = (invocationExpression as MemberAccessExpressionSyntax)?.Name.GetLocation();
					referencedMethodDeclaration = syntaxRef.GetSyntax(ct) as MethodDeclarationSyntax;
					referencedMethodSymbol = methodSymbol;
					break;
				}
				case { Symbol: IMethodSymbol { DeclaringSyntaxReferences: var syntaxRefs } methodSymbol }
				when methodSymbol.GetAttributes().Any(cachedAttributeChecker):
				{
					if (syntaxRefs is not [var syntaxRef])
					{
						return new FailedTransformResult(
							Diagnostic.Create(
								Descriptor_Interceptor0103,
								Location.Create(syntaxRefs[0].SyntaxTree, syntaxRefs[0].Span),
								messageArgs: [invocationExpression.ToString()]
							)
						);
					}

					invocationLocation = (invocationExpression as MemberAccessExpressionSyntax)?.Name.GetLocation();
					referencedMethodDeclaration = syntaxRef.GetSyntax(ct) as MethodDeclarationSyntax;
					referencedMethodSymbol = methodSymbol;
					break;
				}
			}

			if (invocationLocation is null || referencedMethodDeclaration is null || referencedMethodSymbol is null)
			{
				continue;
			}

			if (referencedMethodSymbol.IsGenericMethod)
			{
				continue;
			}

			if (referencedMethodDeclaration.Body is not { Statements: var bodyStatements })
			{
				continue;
			}

			// Now we have the referenced method data.
			// We should determine whether the method is block-bodied, and remove variable declarations
			// bound with cached-related properties, and append attributes [InterceptsLocation], emitting referenced locations.
			// However, here we cannot collect for all possible usages of [InterceptsLocation], so it will be postponed to be checked.
			var statements = new List<StatementSyntax>();
			var flag = false;
			var (existsBeginComment, existsEndComment) = (false, false);
			var duplicateBeginCommentOrEndComment = false;
			foreach (var statement in bodyStatements)
			{
				if (statement.HasLeadingTrivia)
				{
					var leadingTrivia = statement.GetLeadingTrivia().ToFullString();
					if (leadingTrivia.Contains(CommentLineBegin))
					{
						flag = true;
						if (!existsBeginComment)
						{
							existsBeginComment = true;
						}
						else
						{
							duplicateBeginCommentOrEndComment = true;
							break;
						}
						continue;
					}
					if (leadingTrivia.Contains(CommentLineEnd))
					{
						// Get leading trivia, and only removes special line.
						var otherLines = new List<string>();
						foreach (var triviaLine in leadingTrivia.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
						{
							if (!triviaLine.Contains(CommentLineEnd))
							{
								otherLines.Add(triviaLine);
							}
						}

						statements.Add(
							statement
								.WithoutLeadingTrivia()
								.WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia(string.Join("\r\n", otherLines)))
						);
						flag = false;
						if (!existsEndComment)
						{
							existsEndComment = true;
						}
						else
						{
							duplicateBeginCommentOrEndComment = true;
							break;
						}
						continue;
					}
				}
				if (!flag)
				{
					statements.Add(statement);
				}
			}
			if (duplicateBeginCommentOrEndComment)
			{
				return new FailedTransformResult(
					Diagnostic.Create(
						Diagnostic_Interceptor0105,
						identifierToken.GetLocation(),
						messageArgs: [identifierToken.ValueText]
					)
				);
			}
			if (!existsBeginComment || !existsEndComment)
			{
				return new FailedTransformResult(
					Diagnostic.Create(
						Diagnostic_Interceptor0104,
						identifierToken.GetLocation(),
						messageArgs: [identifierToken.ValueText]
					)
				);
			}

			if (invocationLocation.SourceTree is not { FilePath: var filePath })
			{
				continue;
			}

			//var genericTypesString = referencedMethodSymbol.IsGenericMethod
			//	&& (from typeParameter in referencedMethodSymbol.TypeParameters select typeParameter.ToDisplayString()) is var p
			//	&& string.Join(", ", p) is var typeParametersString
			//	? $"<{typeParametersString}>"
			//	: string.Empty;
			var lineSpan = invocationLocation.GetMappedLineSpan();
			var parametersString = string.Join(
				", ",
				from parameter in referencedMethodSymbol.Parameters
				let refKindString = parameter.RefKind switch
				{
					RefKind.Ref => "ref ",
					RefKind.Out => "out ",
					RefKind.In => "in ",
					RefKind.RefReadOnlyParameter => "ref readonly ",
					_ => string.Empty
				}
				let scopedKindString = parameter.ScopedKind switch
				{
					ScopedKind.ScopedRef or ScopedKind.ScopedValue => "scoped ",
					_ => string.Empty
				}
				let typeNameString = parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
				let defaultValue = parameter.HasExplicitDefaultValue
					? parameter.ExplicitDefaultValue switch
					{
						null => "default",
						string s =>
							$""""
							"""{s}"""
							"""",
						char c => $"'{c}'",
						var value => value.ToString()
					}
					: null
				let defaultValueString = defaultValue is null ? string.Empty : $" = {defaultValue}"
				select $"{scopedKindString}{refKindString}{typeNameString} {parameter.Name}{defaultValueString}"
			);

			// Replace reserved identifiers with cached properties.
			var targetString = string.Join("\r\n", from statement in statements select statement.ToFullString());
			foreach (var variableName in ValidVariableNames)
			{
				targetString = targetString.Replace(variableName, variableName[2..]);
			}

			// Add one extra indenting.
			var targetStringAppendIndenting = new List<string>();
			foreach (var line in targetString.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
			{
				targetStringAppendIndenting.Add(line);
			}

			// Get XML doc comments.
			// The target XML doc commment structure won't include triple slashes. We should append them;
			// In addition, consider indenting, we should append indenting \t's.
			// And, we should remove the first and last line (<member> tag).
			var xmlDocComments = referencedMethodSymbol.GetDocumentationCommentXml(cancellationToken: ct)?
				.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
			var xmlDocCommentLines = new List<string>();
			if (xmlDocComments is not null)
			{
				for (var i = 1; i < xmlDocComments.Length - 1; i++)
				{
					xmlDocCommentLines.Add($"/// {xmlDocComments[i].TrimStart()}");
				}
			}

			// Return type.
			var returnTypeName = referencedMethodSymbol.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			var returnRefKindType = referencedMethodSymbol switch
			{
				{ ReturnsByRefReadonly: true } => "ref readonly ",
				{ ReturnsByRef: true } => "ref ",
				_ => string.Empty
			};
			var returnTypeString = $"{returnRefKindType}{returnTypeName}";

			//var constraintsString = constraints.Count != 0 ? constraints.ToString() : string.Empty;

			// Return the value.
			return new SuccessTransformResult(
				$$"""
				/// <summary>
				/// Interceptor method that will replace implementation on such referenced method invocation.
				/// </summary>
				public static class {{referencedMethodSymbol.ContainingType.Name}}_Intercepted
				{
					{{(xmlDocCommentLines.Count == 0 ? "/// <summary>Intercepted method.</summary>" : string.Join("\r\n\t", xmlDocCommentLines))}}
					{{AttributeInsertionMatchString}}
					public static {{returnTypeString}} {{referencedMethodSymbol.Name}}({{parametersString}})
					{
					{{string.Join("\r\n", targetStringAppendIndenting)}}
					}
				}
				""",

				// Here we should manually add 1... I don't know why the value is always less exactly 1.
				new(filePath, lineSpan.StartLinePosition.Line + 1, lineSpan.StartLinePosition.Character + 1)
			);
		}

		return new FailedTransformResult(
			Diagnostic.Create(
				Descriptor_Interceptor0102,
				identifierToken.GetLocation(),
				messageArgs: [identifierToken.ValueText]
			)
		);


		bool cachedAttributeChecker(AttributeData a)
			=> SymbolEqualityComparer.Default.Equals(a.AttributeClass, cachedAttributeSymbol);
	}
}
