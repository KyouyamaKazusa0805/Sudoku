namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0301", "SCA0302")]
public sealed partial class NamingSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context is not
			{
				Node: var node,
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return;
		}

		var action = CheckLocalFunction;
		action += CheckExtensionDeconstructionMethodFirstParameter;

		action(node, compilation, semanticModel);
	}


	/// <summary>
	/// Checks for the name of the local function.
	/// </summary>
	/// <param name="node">The node to check.</param>
	/// <param name="compilation">The compilation.</param>
	/// <param name="semanticModel">The semantic model.</param>
	private void CheckLocalFunction(SyntaxNode node, [IsDiscard] Compilation compilation, [IsDiscard] SemanticModel semanticModel)
	{
		if (
			node is not LocalFunctionStatementSyntax
			{
				Identifier: { ValueText: [not (>= 'a' and <= 'z'), ..] } identifier
			}
		)
		{
			return;
		}

		Diagnostics.Add(Diagnostic.Create(SCA0301, identifier.GetLocation(), messageArgs: null));
	}

	/// <summary>
	/// Checks for the name of the first parameter in an extension deconstruction method.
	/// </summary>
	/// <param name="node">The node to check.</param>
	/// <param name="compilation">The compilation.</param>
	/// <param name="semanticModel">The semantic model.</param>
	private void CheckExtensionDeconstructionMethodFirstParameter(SyntaxNode node, Compilation compilation, SemanticModel semanticModel)
	{
		if (
			node is not MethodDeclarationSyntax
			{
				Identifier: { ValueText: "Deconstruct" } methodIdentifier,
				ParameterList.Parameters: [{ Identifier.ValueText: not "this" }, ..]
			}
		)
		{
			return;
		}

		var symbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
		if (
			symbol is not IMethodSymbol
			{
				IsExtensionMethod: true,
				ReturnType.SpecialType: SpecialType.System_Void
			}
		)
		{
			return;
		}

		Diagnostics.Add(Diagnostic.Create(SCA0302, methodIdentifier.GetLocation(), messageArgs: null));
	}
}
