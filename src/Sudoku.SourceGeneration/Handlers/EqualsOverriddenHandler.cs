namespace Sudoku.SourceGeneration.Handlers;

/// <summary>
/// The generator handler for default overridden of <c>Equals</c>.
/// </summary>
internal static class EqualsOverriddenHandler
{
	/// <inheritdoc cref="IIncrementalGeneratorAttributeHandler{T}.Transform(GeneratorAttributeSyntaxContext, CancellationToken)"/>
	public static EqualsOverriddenCollectedResult? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken cancellationToken)
	{
		if (gasc is not
			{
				Attributes: [{ ConstructorArguments: [{ Value: int rawMode }] }],
				TargetNode: MethodDeclarationSyntax { Modifiers: var modifiers },
				TargetSymbol: IMethodSymbol
				{
					OverriddenMethod: var overriddenMethod,
					ContainingType: { } type,
					Name: nameof(Equals),
					IsOverride: true,
					IsStatic: false,
					ReturnType.SpecialType: System_Boolean,
					IsGenericMethod: false,
					Parameters: [{ Name: var parameterName, Type: { SpecialType: System_Object, NullableAnnotation: Annotated } }]
				} method
			})
		{
			return null;
		}

		// Check whether the method is overridden from object.Equals(object?).
		var rootMethod = overriddenMethod;
		var currentMethod = method;
		for (; rootMethod is not null; rootMethod = rootMethod.OverriddenMethod, currentMethod = currentMethod!.OverriddenMethod) ;
		if (currentMethod!.ContainingType.SpecialType is not (System_Object or System_ValueType))
		{
			return null;
		}

		if ((rawMode, type) switch
		{
			(0, { TypeKind: TypeKind.Struct, IsRefLikeType: true }) => false,
			(1, _) => false,
			(2, { TypeKind: TypeKind.Class }) => false,
			_ => true
		})
		{
			return null;
		}

		return new(rawMode, modifiers, type, parameterName);
	}
}
