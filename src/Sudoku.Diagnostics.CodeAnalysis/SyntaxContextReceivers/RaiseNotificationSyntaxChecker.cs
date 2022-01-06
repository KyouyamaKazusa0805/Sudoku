namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0201")]
public sealed partial class RaiseNotificationSyntaxChecker : ISyntaxContextReceiver
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

		var dataContextSymbol = compilation.GetTypeByMetadataName("Sudoku.UI.Windows.DataContexts.DataContext");
		if (dataContextSymbol is null)
		{
			return;
		}

		var operation = semanticModel.GetOperation(node, _cancellationToken);
		if (
			operation is not IInvocationOperation
			{
				TargetMethod:
				{
					Name: "RaiseNotification",
					ReturnType.SpecialType: SpecialType.System_Void
				},
				Instance.Type: var instanceType,
				Arguments: [
				{
					Type.SpecialType: SpecialType.System_String,
					ConstantValue: { HasValue: true, Value: string value }
				}],
				Syntax.Parent.Parent.Parent.Parent.Parent: PropertyDeclarationSyntax
				{
					Identifier: { ValueText: var propertyName } identifier
				}
			}
		)
		{
			return;
		}

		bool baseTypeMatches = false;
		for (var symbol = instanceType; symbol is not null; symbol = symbol.BaseType)
		{
			if (SymbolEqualityComparer.Default.Equals(symbol, dataContextSymbol))
			{
				baseTypeMatches = true;
				break;
			}
		}
		if (!baseTypeMatches)
		{
			return;
		}

		if (value == propertyName)
		{
			return;
		}

		Diagnostics.Add(Diagnostic.Create(SCA0201, identifier.GetLocation(), messageArgs: null));
	}
}
