namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

/// <summary>
/// Defines the syntax checker that checks for the diagnostics below:
/// <list type="table">
/// <item>
/// <term><c>SDC0201</c></term>
/// <description>The discarded parameter can't be used or referenced unless a <see langword="nameof"/> expression.</description>
/// </item>
/// <item>
/// <term><c>SDC0202</c></term>
/// <description>Discard parameter can't be modified.</description>
/// </item>
/// <item>
/// <term><c>SDC0203</c></term>
/// <description>Can't apply <see cref="IsDiscardAttribute"/> onto a parameter that has already discarded.</description>
/// </item>
/// </list>
/// </summary>
public sealed class DiscardParameterSyntaxChecker : ISyntaxContextReceiver
{
	/// <summary>
	/// Indicates the context used.
	/// </summary>
	private readonly CancellationToken _cancellationToken;


	/// <summary>
	/// Initializes a <see cref="ThisConstraintSyntaxChecker"/> instance using the cancellation token.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	public DiscardParameterSyntaxChecker(CancellationToken cancellationToken) =>
		_cancellationToken = cancellationToken;


	/// <summary>
	/// Indicates all possible diagnostics types used.
	/// </summary>
	public List<Diagnostic> Diagnostics { get; } = new();


	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		// Deconstruct instance.
		if (
			context is not
			{
				Node: var node and (MethodDeclarationSyntax or LocalFunctionStatementSyntax),
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return;
		}

		var attribute = compilation.GetTypeByMetadataName(typeof(IsDiscardAttribute).FullName);

		var symbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
		if (symbol is not IMethodSymbol { Parameters: { Length: not 0 } parameters })
		{
			return;
		}

		foreach (var parameter in parameters)
		{
			var attributesData = parameter.GetAttributes();
			if (!attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
			{
				continue;
			}

			switch (parameter)
			{
				case { IsDiscard: true }: report(parameter, Sdc0203); break;
				case { IsParams: true } or { RefKind: not RefKind.None }: report(parameter, Sdc0202); break;
				case { Name: var parameterName }: traverseDescendants(node, parameterName); break;
			}
		}


		void report(IParameterSymbol parameter, DiagnosticDescriptor descriptor) =>
			Diagnostics.Add(
				Diagnostic.Create(
					descriptor,
					location: parameter.Locations[0],
					messageArgs: null
				)
			);

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
								descriptor: Sdc0201,
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
