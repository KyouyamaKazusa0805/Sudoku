using System.ComponentModel;

namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0411", "SCA0412", "SCA0413", "SCA0414", "SCA0415", "SCA0416", "SCA0417")]
public sealed partial class DeconstructionMethodSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context is not
			{
				Node: MethodDeclarationSyntax { Identifier: var identifier } node,
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return;
		}

		var symbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
		if (
			symbol is not IMethodSymbol
			{
				Name: "Deconstruct",
				IsExtensionMethod: var isExtensionMethod,
				Parameters: var parameters,
				ReturnType.SpecialType: var returnTypeSpecialType
			} methodSymbol
		)
		{
			return;
		}

		var location = identifier.GetLocation();
		if (returnTypeSpecialType != SpecialType.System_Void)
		{
			Diagnostics.Add(Diagnostic.Create(SCA0411, location, messageArgs: null));
			return;
		}

		if ((isExtensionMethod ? parameters.Skip(1) : parameters).Any(isNotOutParameter))
		{
			Diagnostics.Add(Diagnostic.Create(SCA0412, location, messageArgs: null));
			return;
		}

		var isDiscardAttribute = compilation.GetTypeSymbol<IsDiscardAttribute>();
		reportWrongUsagesOnEditorBrowsableNever(symbol);

		switch (symbol)
		{
			// Normal deconstruction method.
			case { IsGenericMethod: var isGeneric } when !isExtensionMethod:
			{
				if (isGeneric)
				{
					Diagnostics.Add(Diagnostic.Create(SCA0417, location, messageArgs: null));
					return;
				}

				if (parameters.Any(containsDiscardAttribute))
				{
					Diagnostics.Add(Diagnostic.Create(SCA0413, location, messageArgs: null));
					return;
				}

				switch (parameters)
				{
					case []:
					{
						Diagnostics.Add(Diagnostic.Create(SCA0414, location, messageArgs: null));
						break;
					}
					case [_]:
					{
						Diagnostics.Add(Diagnostic.Create(SCA0415, location, messageArgs: null));
						break;
					}
				}

				break;
			}

			// Extension deconstruction method.
			case var _ when isExtensionMethod:
			{
				var outParameters = parameters.Skip(1).ToImmutableArray();
				if (outParameters.Any(containsDiscardAttribute))
				{
					Diagnostics.Add(Diagnostic.Create(SCA0413, location, messageArgs: null));
					return;
				}

				switch (outParameters)
				{
					case []:
					{
						Diagnostics.Add(Diagnostic.Create(SCA0414, location, messageArgs: null));
						break;
					}
					case [_]:
					{
						Diagnostics.Add(Diagnostic.Create(SCA0415, location, messageArgs: null));
						break;
					}
				}

				break;
			}
		}


		static bool isNotOutParameter(IParameterSymbol parameter) => parameter.RefKind != RefKind.Out;

		bool containsDiscardAttribute(IParameterSymbol parameter) =>
			parameter.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, isDiscardAttribute));

		void reportWrongUsagesOnEditorBrowsableNever(ISymbol symbol)
		{
			var editorBrowsableAttribute = compilation.GetTypeSymbol<EditorBrowsableAttribute>();
			var methodAttributeData = symbol.GetAttributes().FirstOrDefault(
				attributeData =>
					attributeData is { AttributeClass: var attribute, ConstructorArguments.Length: 1 }
					&& SymbolEqualityComparer.Default.Equals(editorBrowsableAttribute, attribute)
			);

			if (
				methodAttributeData switch
				{
					null => true,
					{
						ConstructorArguments: [{ Value: EditorBrowsableState cArg }, ..]
					} when cArg == EditorBrowsableState.Always => true,
					_ => false
				}
			)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0416, location, messageArgs: null));
			}
		}
	}
}
