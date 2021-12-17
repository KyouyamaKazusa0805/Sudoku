namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0201", "SCA0202", "SCA0203")]
public sealed partial class DiscardParameterSyntaxChecker : ISyntaxContextReceiver
{
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
				case { IsDiscard: true }:
				{
					report(parameter, SCA0203);

					break;
				}
				case { IsParams: true } or { RefKind: not (RefKind.None or RefKind.In) }:
				{
					report(parameter, SCA0202);

					break;
				}
				case { Name: var parameterName }:
				{
					traverseDescendants(node, parameterName);

					break;
				}
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
								descriptor: SCA0201,
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
