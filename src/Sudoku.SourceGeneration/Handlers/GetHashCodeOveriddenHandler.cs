namespace Sudoku.SourceGeneration.Handlers;

/// <summary>
/// The generator handler for default overridden of <c>GetHashCode</c>.
/// </summary>
internal static class GetHashCodeOveriddenHandler
{
	/// <inheritdoc cref="IIncrementalGeneratorAttributeHandler{T}.Transform(GeneratorAttributeSyntaxContext, CancellationToken)"/>
	public static GetHashCodeCollectedResult? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken cancellationToken)
	{
		if (gasc is not
			{
				Attributes: [{ ConstructorArguments: [{ Value: int rawMode }, { Values: var extraArguments }] }],
				TargetNode: MethodDeclarationSyntax { Modifiers: var modifiers },
				TargetSymbol: IMethodSymbol
				{
					OverriddenMethod: var overriddenMethod,
					ContainingType: { } type,
					Name: nameof(GetHashCode),
					IsOverride: true,
					IsStatic: false,
					ReturnType.SpecialType: System_Int32,
					IsGenericMethod: false,
					Parameters: []
				} method
			})
		{
			return null;
		}

		// Check whether the method is overridden from object.GetHashCode.
		var rootMethod = overriddenMethod;
		var currentMethod = method;
		for (; rootMethod is not null; rootMethod = rootMethod.OverriddenMethod, currentMethod = currentMethod!.OverriddenMethod) ;
		if (currentMethod!.ContainingType.SpecialType is not (System_Object or System_ValueType))
		{
			return null;
		}

		if ((rawMode, type) switch { (0, { TypeKind: TypeKind.Struct, IsRefLikeType: true }) => false, (1 or 2, _) => false, _ => true })
		{
			return null;
		}

		return new(rawMode, modifiers, type, from extraArgument in extraArguments select (string)extraArgument.Value!);
	}
}
