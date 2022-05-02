namespace Sudoku.Diagnostics.CodeGen.Generators;

partial class StepSearcherOptionsGenerator
{
	private sealed record class Receiver(CancellationToken CancellationToken) : ISyntaxContextReceiver
	{
		/// <summary>
		/// Defines a dictionary that stores the lookup table for fields in the enumeration type <c>EnabledAreas</c>.
		/// </summary>
		public IDictionary<byte, string> EnabledAreasFields { get; } = new Dictionary<byte, string>();

		/// <summary>
		/// Defines a dictionary that stores the lookup table for fields in the enumeration type <c>DisabledReason</c>.
		/// </summary>
		public IDictionary<short, string> DisabledReasonFields { get; } = new Dictionary<short, string>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			if (
				context is not
				{
					SemanticModel: { Compilation: var compilation } semanticModel,
					Node: EnumMemberDeclarationSyntax
					{
						Identifier.ValueText: var fieldName,
						EqualsValue: EqualsValueClauseSyntax equalsValueSyntax,
						Parent: EnumDeclarationSyntax enumTypeNode
					}
				}
			)
			{
				return;
			}

			var enumTypeSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(enumTypeNode, CancellationToken)!;
			var enabledAreaTypeSymbol = compilation.GetTypeByMetadataName("Sudoku.Solving.Manual.EnabledAreas")!;
			var disabledReasonTypeSymbol = compilation.GetTypeByMetadataName("Sudoku.Solving.Manual.DisabledReason")!;

			var operation = semanticModel.GetOperation(equalsValueSyntax, CancellationToken);
			if (
				operation is not IFieldInitializerOperation
				{
					Value.ConstantValue: { HasValue: true, Value: var value and (byte or short) }
				}
			)
			{
				return;
			}

			switch (value)
			{
				case byte realValue when SymbolEqualityComparer.Default.Equals(enumTypeSymbol, enabledAreaTypeSymbol):
				{
					EnabledAreasFields.Add(realValue, fieldName);
					break;
				}
				case short realValue when SymbolEqualityComparer.Default.Equals(enumTypeSymbol, disabledReasonTypeSymbol):
				{
					DisabledReasonFields.Add(realValue, fieldName);
					break;
				}
			}
		}
	}
}
