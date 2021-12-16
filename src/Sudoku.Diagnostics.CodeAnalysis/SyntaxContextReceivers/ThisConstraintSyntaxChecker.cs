namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

/// <summary>
/// Defines the syntax checker that checks for the diagnostics below:
/// <list type="table">
/// <item>
/// <term><c>SDC0101</c></term>
/// <description>The type parameter lacks a CRTP-constraint clause.</description>
/// </item>
/// <item>
/// <term><c>SDC0102</c></term>
/// <description>The type parameter constraint lacks a CRTP-styled type as the constraint.</description>
/// </item>
/// </list>
/// </summary>
public sealed class ThisConstraintSyntaxChecker : ISyntaxContextReceiver
{
	/// <summary>
	/// Indicates the context used.
	/// </summary>
	private readonly CancellationToken _cancellationToken;


	/// <summary>
	/// Initializes a <see cref="ThisConstraintSyntaxChecker"/> instance using the cancellation token.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	public ThisConstraintSyntaxChecker(CancellationToken cancellationToken) =>
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
				Node: TypeDeclarationSyntax { ConstraintClauses: var whereClauses } node,
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return;
		}

		// Determine whether the type contains any type parameters.
		var symbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
		if (
			symbol is not INamedTypeSymbol
			{
				IsGenericType: true,
				TypeParameters: { IsDefaultOrEmpty: false } typeParameters
			}
		)
		{
			return;
		}

		// Checks for each type parameter, to determine whether the type parameter
		// has marked the attribute [SelfTypeParameter].
		var selfType = compilation.GetTypeByMetadataName(typeof(SelfAttribute).FullName)!;
		foreach (var typeParameter in typeParameters)
		{
			if (typeParameter is not { Name: var typeParameterName, Locations: { Length: >= 1 } locations })
			{
				continue;
			}

			if (typeParameter.GetAttributes() is not { IsDefaultOrEmpty: false } attributesData)
			{
				continue;
			}

			if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, selfType)))
			{
				continue;
			}

			string shortenTypeName = symbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat);

			if (whereClauses.Count == 0)
			{
				Diagnostics.Add(
					Diagnostic.Create(
						descriptor: Sdc0101,
						location: locations[0],
						messageArgs: new[] { shortenTypeName, typeParameterName }
					)
				);

				continue;
			}

			// Checks for the existence of the constraint of this type parameter.
			switch (whereClauses.FirstOrDefault(correspondingClauseMatcher))
			{
				case null:
				{
					Diagnostics.Add(
						Diagnostic.Create(
							descriptor: Sdc0101,
							location: locations[0],
							messageArgs: new[] { shortenTypeName, typeParameterName }
						)
					);

					continue;
				}
				case { Constraints: { Count: var constraintsCount } constraints } correspondingClause:
				{
					// If the constraint exists, check for the content.
					var typeConstraints = constraints.OfType<TypeConstraintSyntax>();
					if (!typeConstraints.Any() || typeConstraints.All(notSelfConstraintChecker))
					{
						Diagnostics.Add(
							Diagnostic.Create(
								descriptor: Sdc0102,
								location: locations[0],
								messageArgs: new[] { typeParameterName, shortenTypeName }
							)
						);
					}

					break;
				}


				bool notSelfConstraintChecker(TypeConstraintSyntax constraint) =>
					!SymbolEqualityComparer.Default.Equals(
						semanticModel.GetTypeInfo(constraint.Type, _cancellationToken).Type,
						symbol
					);
			}


			bool correspondingClauseMatcher(TypeParameterConstraintClauseSyntax clause) =>
				clause.Name is IdentifierNameSyntax { Identifier.ValueText: var boundTypeParameterName }
				&& boundTypeParameterName == typeParameterName;
		}
	}
}
