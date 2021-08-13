using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SD0413")]
public sealed partial class DiscardParameterAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(
			AnalyzeSyntaxNode,
			new[] { SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement }
		);
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		var (semanticModel, compilation, originalNode, _, cancellationToken) = context;

		Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
		var attribute = compilation.GetTypeByMetadataName("System.Diagnostics.CodeAnalysis.DiscardAttribute");
		switch (originalNode)
		{
			case MethodDeclarationSyntax method when whenClause(method, out string? parameterName):
			{
				traverseDescendants(
					method,
					parameterName
#if !NETSTANDARD2_1_OR_GREATER
					!
#endif
				);

				break;
			}
			case LocalFunctionStatementSyntax function when whenClause(function, out string? parameterName):
			{
				traverseDescendants(
					function,
					parameterName
#if !NETSTANDARD2_1_OR_GREATER
					!
#endif
				);

				break;
			}


			bool whenClause(
				SyntaxNode method,
#if NETSTANDARD2_1_OR_GREATER
				[NotNullWhen(true)]
#endif
				out string? parameterName
			)
			{
				if (
					semanticModel.GetDeclaredSymbol(method, cancellationToken) is IMethodSymbol
					{
						Parameters: { Length: not 0 } parameters
					} && parameters.FirstOrDefault(
						parameter => parameter.GetAttributes().Any(a => f(a.AttributeClass, attribute))
					) is { Name: var possibleParameterName }
				)
				{
					parameterName = possibleParameterName;
					return true;
				}
				else
				{
					parameterName = null;
					return false;
				}
			}

			void traverseDescendants(SyntaxNode method, string parameterName)
			{
				var descendants = method.DescendantNodes().ToArray();
				for (int i = 0, length = descendants.Length; i < length; i++)
				{
					switch (descendants[i])
					{
						case IdentifierNameSyntax
						{
							Parent: not ArgumentSyntax
							{
								Parent: ArgumentListSyntax
								{
									Parent: InvocationExpressionSyntax
									{
										Expression: IdentifierNameSyntax
										{
											Identifier: { ValueText: "nameof" }
										}
									}
								}
							},
							Identifier: { ValueText: var possibleReference }
						} usage
						when possibleReference == parameterName:
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SD0413,
									location: usage.GetLocation(),
									messageArgs: null
								)
							);

							break;
						}

						// The variable may not the same variable if the variable exists in:
						//
						//   1) a static lambda            'static v => { }'           or 'static (v) => { }'
						//   2) static anonymous function  'static delegate (T v) { }'
						//   3) a static local function    'static void f(T v) { }'
						//
						// If so, the whole expression should be skipped the checking.
						case SimpleLambdaExpressionSyntax { Modifiers: { Count: not 0 } modifiers } d
						when modifiers.Any(static m => m is { RawKind: (int)SyntaxKind.StaticKeyword }):
						{
							// Skip the whole expression.
							skipNodes(d, ref i);
							continue;
						}
						case ParenthesizedLambdaExpressionSyntax { Modifiers: { Count: not 0 } modifiers } d
						when modifiers.Any(static m => m is { RawKind: (int)SyntaxKind.StaticKeyword }):
						{
							// Skip the whole expression.
							skipNodes(d, ref i);
							continue;
						}
						case AnonymousFunctionExpressionSyntax { Modifiers: { Count: not 0 } modifiers } d
						when modifiers.Any(static m => m is { RawKind: (int)SyntaxKind.StaticKeyword }):
						{
							// Skip the whole expression.
							skipNodes(d, ref i);
							continue;
						}
						case LocalFunctionStatementSyntax { Modifiers: { Count: not 0 } modifiers } d
						when modifiers.Any(static m => m is { RawKind: (int)SyntaxKind.StaticKeyword }):
						{
							// Skip the whole expression.
							skipNodes(d, ref i);
							continue;
						}


						[MethodImpl(MethodImplOptions.AggressiveInlining)]
						void skipNodes(SyntaxNode d, ref int i) => i += d.DescendantNodes().Count();
					}
				}
			}
		}
	}
}
