namespace Sudoku.CodeGenerating.Generators;

partial class ExtensionDeconstructMethodGenerator
{
	/// <summary>
	/// Indicates the inner syntax receiver.
	/// </summary>
	private sealed class SyntaxReceiver : ISyntaxReceiver
	{
		/// <summary>
		/// Indicates the attributes result that targets to a module.
		/// </summary>
		public IList<AttributeSyntax> Attributes { get; } = new List<AttributeSyntax>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (
				syntaxNode is not AttributeListSyntax
				{
					Attributes: { Count: not 0 } attributes,
					Target.Identifier.ValueText: "assembly"
				}
			)
			{
				return;
			}

			bool listContainsGenericAttribute = false;
			foreach (var attribute in attributes)
			{
				if (
					attribute is
					{
						Name: GenericNameSyntax
						{
							Identifier.ValueText:
								"AutoDeconstructExtension"
								or "AutoDeconstructExtensionAttribute"
						}
					}
				)
				{
					listContainsGenericAttribute = true;
					break;
				}
			}
			if (!listContainsGenericAttribute)
			{
				return;
			}

			foreach (var attribute in attributes)
			{
				Attributes.Add(attribute);
			}
		}
	}
}
