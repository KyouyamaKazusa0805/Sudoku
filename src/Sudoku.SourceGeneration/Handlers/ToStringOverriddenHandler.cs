namespace Sudoku.SourceGeneration.Handlers;

/// <summary>
/// The generator handler for default overridden of <c>ToString</c>.
/// </summary>
internal static class ToStringOverriddenHandler
{
	/// <inheritdoc cref="IIncrementalGeneratorAttributeHandler{T}.Transform(GeneratorAttributeSyntaxContext, CancellationToken)"/>
	public static ToStringCollectedResult? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken cancellationToken)
	{
		if (gasc is not
			{
				Attributes: [{ ConstructorArguments: [{ Value: int rawMode }, { Values: var extraArguments }] }],
				TargetNode: MethodDeclarationSyntax { Modifiers: var modifiers },
				TargetSymbol: IMethodSymbol
				{
					OverriddenMethod: var overriddenMethod,
					ContainingType: { } type,
					Name: nameof(ToString),
					IsOverride: true,
					IsStatic: false,
					ReturnType.SpecialType: System_String,
					IsGenericMethod: false,
					Parameters: []
				} method,
				SemanticModel.Compilation: var compilation
			})
		{
			return null;
		}

		// Check whether the method is overridden from object.ToString.
		var rootMethod = overriddenMethod;
		var currentMethod = method;
		for (; rootMethod is not null; rootMethod = rootMethod.OverriddenMethod, currentMethod = currentMethod!.OverriddenMethod) ;
		if (currentMethod!.ContainingType.SpecialType is not (System_Object or System_ValueType))
		{
			return null;
		}

		var attributeType = compilation.GetTypeByMetadataName("System.SourceGeneration.ToStringIdentifierAttribute");
		if (attributeType is null)
		{
			return null;
		}

		return new(rawMode, modifiers, type, attributeType, from extraArgument in extraArguments select (string)extraArgument.Value!);
	}
}
