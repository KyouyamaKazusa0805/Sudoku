namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

/// <summary>
/// Defines an analyzer that analyzes the code for the usage of the type <see cref="DiscardAttribute"/>
/// on parameters.
/// </summary>
/// <seealso cref="DiscardAttribute"/>
[Generator]
public sealed class DiscardParameterAnalyzer : ISourceGenerator
{
	/// <summary>
	/// Indicates the descriptor.
	/// </summary>
	private static readonly DiagnosticDescriptor Descriptor = new(
		id: "SS_DiscardParameter",
		title: "Can't reference the discarded parameter name",
		messageFormat: "Can't reference the discarded parameter name unless a 'nameof' expression",
		category: "Sunnie.Style",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		helpLinkUri: null,
		customTags: new[] { WellKnownDiagnosticTags.Unnecessary }
	);


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context) =>
		((Receiver)context.SyntaxContextReceiver!).Diagnostics.ForEach(context.ReportDiagnostic);

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new Receiver(context));


	private sealed class Receiver : ISyntaxContextReceiverWithResult
	{
		private readonly GeneratorInitializationContext _context;


		public Receiver(GeneratorInitializationContext context) => _context = context;


		public List<Diagnostic> Diagnostics { get; } = new();


		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			if (
				(Context: context, InitializationContext: _context) is not (
					Context: { Node: var node, SemanticModel: { Compilation: { } compilation } semanticModel },
					InitializationContext: { CancellationToken: var cancellationToken }
				)
			)
			{
				return;
			}

			var attribute = compilation.GetTypeByMetadataName(typeof(DiscardAttribute).FullName);
			switch (node)
			{
				case MethodDeclarationSyntax method when whenClause(method, out string? parameterName):
				{
					traverseDescendants(method, parameterName!);

					break;
				}
				case LocalFunctionStatementSyntax function when whenClause(function, out string? parameterName):
				{
					traverseDescendants(function, parameterName!);

					break;
				}


				bool whenClause(SyntaxNode method, out string? parameterName)
				{
					var methodSymbol = semanticModel.GetDeclaredSymbol(method, cancellationToken);
					(parameterName, bool @return) =
						methodSymbol is IMethodSymbol { Parameters: { Length: not 0 } parameters }
						&& parameters.FirstOrDefault(
							p =>
								p.GetAttributes().Any(
									a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)
								)
						) is { Name: var possibleParameterName } ? (possibleParameterName, true) : (null, false);

					return @return;
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
											Expression: IdentifierNameSyntax { Identifier.ValueText: "nameof" }
										}
									}
								},
								Identifier.ValueText: var possibleReference
							} usage
							when possibleReference == parameterName:
							{
								Diagnostics.Add(
									Diagnostic.Create(
										descriptor: Descriptor,
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
}
