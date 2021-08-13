namespace Sudoku.Diagnostics.CodeAnalysis.Extensions;

/// <summary>
/// Provides extension methods on <see cref="MethodDeclarationSyntax"/>.
/// </summary>
/// <seealso cref="MethodDeclarationSyntax"/>
public static class MethodDeclarationSyntaxEx
{
	/// <summary>
	/// Determine whether the specified method syntax node is an valid deconstruction method
	/// declaration of a type.
	/// </summary>
	/// <param name="this">The node to check.</param>
	/// <param name="checkMemberExistence">
	/// A <see cref="bool"/> value indicating whether the all parameters can be corresponding
	/// to the members in that type.
	/// </param>
	/// <param name="semanticModel">
	/// The corresponding semantic model to check and get type symbol. This argument shouldn't be
	/// <see langword="null"/> when <paramref name="checkMemberExistence"/> is <see langword="true"/>;
	/// otherwise, the method can't infer the correct result, so always returns <see langword="false"/>
	/// for this case.
	/// </param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool IsDeconstructionMethod(
		this MethodDeclarationSyntax @this, bool checkMemberExistence = false,
		SemanticModel? semanticModel = null, CancellationToken cancellationToken = default)
	{
		// Check some general properties.
		if (
			@this is not
			{
				// Name must be "Deconstruct".
				Identifier: { ValueText: "Deconstruct" },

				// The number of parameters must be greater than 1 when works.
				ParameterList: { Parameters: { Count: >= 2 } parameters },

				// The deconstruct method shouldn't be a generic method.
				TypeParameterList: not { Parameters: { Count: not 0 } },

				// We don't support explicit interface method implementation checking now.
				ExplicitInterfaceSpecifier: null,

				// The return type must be "void".
				ReturnType: PredefinedTypeSyntax { Keyword: { RawKind: (int)SyntaxKind.VoidKeyword } },

				// Modifiers shouldn't contain the keyword 'static' (validate it later).
				Modifiers: var modifiers
			}
		)
		{
			return false;
		}

		// Check whether the deconstruction method is a public method.
		if (modifiers.All(static modifier => modifier is not { RawKind: (int)SyntaxKind.PublicKeyword }))
		{
			return false;
		}

		// Check whether the deconstruction method is an instance method.
		if (modifiers.Any(static modifier => modifier is { RawKind: (int)SyntaxKind.StaticKeyword }))
		{
			return false;
		}

		// Check whether the deconstruction method, whose all parameters is 'out' ones.
		if (
			parameters.Any(
				static parameter => parameter.Modifiers.All(
					static modifier => modifier.RawKind != (int)SyntaxKind.OutKeyword
				)
			)
		)
		{
			return false;
		}

		// If we don't need to check corresponding member existence, just return true.
		if (!checkMemberExistence)
		{
			return true;
		}

		// Now check the existence.
		if (
			semanticModel?.GetDeclaredSymbol(@this, cancellationToken) is not IMethodSymbol
			{
				Parameters: var symbolParameters
			} methodSymbol
		)
		{
			// We can't infer the result.
			return false;
		}

		// Iterate on each symbol parameter, to check it.
		foreach (var symbolParameter in symbolParameters)
		{
			var (type, name) = symbolParameter;
			string possibleFieldName = name.ToCamelCase()!, possiblePropertyName = name.ToPascalCase();

			if (
				(
					from fieldOrPropertySymbol in type.GetMembers()
					where fieldOrPropertySymbol.CanBeReferencedByName
					select fieldOrPropertySymbol
				).All(
					symbol => symbol switch
					{
						IFieldSymbol when symbol.Name != possibleFieldName => true,
						IPropertySymbol when symbol.Name != possiblePropertyName => true,
						_ => false
					}
				)
			)
			{
				return false;
			}
		}

		return true;
	}

	///// <summary>
	///// Determine whether the specified method syntax node is an valid extension deconstruction method
	///// declaration of a type.
	///// </summary>
	///// <param name="this">The node to check.</param>
	///// <param name="checkMemberExistence">
	///// A <see cref="bool"/> value indicating whether the all parameters can be corresponding
	///// to the members in that type.
	///// </param>
	///// <returns>A <see cref="bool"/> result indicating that.</returns>
	//public static bool IsExtensionDeconstructionMethod(
	//	this MethodDeclarationSyntax @this, bool checkMemberExistence = false)
	//{
	//
	//}
}
