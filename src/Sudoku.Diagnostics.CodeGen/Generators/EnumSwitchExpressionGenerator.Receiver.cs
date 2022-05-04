namespace Sudoku.Diagnostics.CodeGen.Generators;

partial class EnumSwitchExpressionGenerator
{
	/// <summary>
	/// The inner syntax context receiver instance.
	/// </summary>
	/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
	private sealed partial record class Receiver(CancellationToken CancellationToken) : ISyntaxContextReceiver
	{
		private const string
			SwitchExprRootFullName = "System.Diagnostics.CodeGen.EnumSwitchExpressionRootAttribute",
			SwitchExprArmFullName = "System.Diagnostics.CodeGen.EnumSwitchExpressionArmAttribute";


		/// <summary>
		/// Indicates the result collection.
		/// </summary>
		public ICollection<(INamedTypeSymbol Type, Tuple[] Tuples)> Collection { get; }
			= new List<(INamedTypeSymbol, Tuple[])>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			if (
				context is not
				{
					Node: EnumDeclarationSyntax enumDeclarationSyntaxNode,
					SemanticModel: { Compilation: var compilation } semanticModel
				}
			)
			{
				return;
			}

			var rawTypeSymbol = semanticModel.GetDeclaredSymbol(enumDeclarationSyntaxNode, CancellationToken);
			if (rawTypeSymbol is not INamedTypeSymbol typeSymbol)
			{
				return;
			}

			var switchExprRoot = compilation.GetTypeByMetadataName(SwitchExprRootFullName);
			var typeAttributesData = (
				from attributeData in typeSymbol.GetAttributes()
				where SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, switchExprRoot)
				select attributeData
			).Distinct(new AttributeDataComparerDistinctByKey()).ToArray();
			if (typeAttributesData.Length == 0)
			{
				return;
			}

			var result = new List<Tuple>();
			var switchExprArm = compilation.GetTypeByMetadataName(SwitchExprArmFullName);
			var fieldAndItsCorrespondingAttributeData = new List<(IFieldSymbol, AttributeData)>();
			foreach (var attributeData in typeAttributesData)
			{
				string key = (string)attributeData.ConstructorArguments[0].Value!;
				foreach (var field in typeSymbol.GetMembers().OfType<IFieldSymbol>())
				{
					var fieldAttributeData = (
						from fad in field.GetAttributes()
						where SymbolEqualityComparer.Default.Equals(fad.AttributeClass, switchExprArm)
						let construtorArgs = fad.ConstructorArguments
						where construtorArgs.Length >= 1
						let firstConstructorArg = (string)construtorArgs[0].Value!
						where firstConstructorArg == key
						select fad
					).FirstOrDefault();

					fieldAndItsCorrespondingAttributeData.Add((field, fieldAttributeData));
				}

				result.Add(new(typeSymbol, key, attributeData, fieldAndItsCorrespondingAttributeData.ToArray()));
				fieldAndItsCorrespondingAttributeData.Clear();
			}

			Collection.Add((typeSymbol, result.ToArray()));
		}
	}
}
