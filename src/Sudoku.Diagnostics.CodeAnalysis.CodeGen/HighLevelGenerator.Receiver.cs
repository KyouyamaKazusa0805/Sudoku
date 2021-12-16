namespace Sudoku.Diagnostics.CodeGen;

partial class HighLevelGenerator
{
	/// <summary>
	/// Indicates the inner receiver used.
	/// </summary>
	private sealed class Receiver : ISyntaxContextReceiver
	{
		/// <summary>
		/// Indicates the regular expression instance to match a valid diagnostic ID value.
		/// </summary>
		private static readonly Regex DiagnosticIdRegex = new(
			pattern: @"SCA\d{4}",
			options: RegexOptions.Compiled,
			matchTimeout: TimeSpan.FromSeconds(5)
		);


		/// <summary>
		/// Indicates the cancellation token to cancel the operation.
		/// </summary>
		private readonly CancellationToken _cancellationToken;


		/// <summary>
		/// Initializes a <see cref="Receiver"/> instance via a cancellation token.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		public Receiver(CancellationToken cancellationToken) => _cancellationToken = cancellationToken;


		/// <summary>
		/// Indicates the valid type names found.
		/// </summary>
		public ICollection<(string ShortName, string FullName, string[] DiagnosticIds)> Result { get; } = new List<(string, string, string[])>();

		/// <summary>
		/// Indicates the possible compiler diagnostics found.
		/// </summary>
		public ICollection<Diagnostic> Diagnostics { get; } = new List<Diagnostic>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			// Determines the validity of the selected node.
			if (
				context is not
				{
					Node: ClassDeclarationSyntax
					{
						Modifiers: var modifiers,
						Identifier: var identifierName
					} node,
					SemanticModel: { Compilation: var compilation } semanticModel
				}
			)
			{
				return;
			}

			// Gets the type symbol to check later.
			var symbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
			if (
				symbol is not INamedTypeSymbol
				{
					IsAbstract: var isAbstract,
					IsGenericType: var isGeneric,
					DeclaringSyntaxReferences.Length: var partialFilesCount,
					IsSealed: var isSealed,
					ContainingType: var containingType,
					Name: var name,
					AllInterfaces: var interfaces
				} typeSymbol
			)
			{
				return;
			}

			// Check the existence of type 'SyntaxCheckerAttribute'.
			var attributesData = symbol.GetAttributes();
			var attributeTypeSymbol = compilation.GetTypeByMetadataName(typeof(SyntaxCheckerAttribute).FullName);
			var attribute = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeTypeSymbol));
			if (
				attribute is not
				{
					ConstructorArguments: { Length: var attributeArgsCount } constructorArguments,
					ApplicationSyntaxReference: { } attributeSyntaxReference
				}
			)
			{
				return;
			}

			// Gets the property result.
			if (
				constructorArguments[0] is not
				{
					Kind: TypedConstantKind.Array,
					IsNull: false,
					Values: { Length: var argsCount } constants
				}
			)
			{
				return;
			}

			// The attribute argument list must contain at least one element.
			var attributeLocation = attributeSyntaxReference.ToLocation();
			if (attributeArgsCount != 1 || argsCount == 0)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0001, attributeLocation, messageArgs: null));
				return;
			}

			// Gets the real diagnostic IDs.
			var realValues = new List<string>(argsCount);
			for (int i = 0; i < constants.Length; i++)
			{
				var typedConstant = constants[i];

				// The value can't be null.
				if (typedConstant.IsNull)
				{
					Diagnostics.Add(Diagnostic.Create(SCA0002, attributeLocation, messageArgs: null));
					continue;
				}

				// The value must start with 'SCA' and end with four-digit ID number.
				string convertedValue = (string)typedConstant.Value!;
				if (!DiagnosticIdRegex.IsMatch(convertedValue))
				{
					Diagnostics.Add(Diagnostic.Create(SCA0003, attributeLocation, messageArgs: null));
					continue;
				}

				realValues.Add(convertedValue);
			}

			// The type must be top-level type (i.e. non-nested type).
			if (containingType is not null)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0004, identifierName.GetLocation(), messageArgs: null));
				return;
			}

			// The type can't be modified 'abstract'.
			if (isAbstract)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0005, identifierName.GetLocation(), messageArgs: null));
				return;
			}

			if (isGeneric)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0006, identifierName.GetLocation(), messageArgs: null));
				return;
			}

			// The type must be modified 'partial'.
			if (!modifiers.Any(SyntaxKind.PartialKeyword))
			{
				Diagnostics.Add(Diagnostic.Create(SCA0007, identifierName.GetLocation(), messageArgs: null));
				return;
			}

			// The type should be modified 'sealed'.
			if (!isSealed)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0008, identifierName.GetLocation(), messageArgs: null));
			}

			// The type must implement the interface 'ISyntaxContextReceiver'.
			var receiverSymbol = compilation.GetTypeByMetadataName(typeof(ISyntaxContextReceiver).FullName);
			if (interfaces.All(t => !SymbolEqualityComparer.Default.Equals(t, receiverSymbol)))
			{
				Diagnostics.Add(Diagnostic.Create(SCA0009, identifierName.GetLocation(), messageArgs: null));
				return;
			}

			// Gets the members.
			var members = typeSymbol.GetMembers();

			// Check the existence of the field '_cancellationToken'.
			var field = members.OfType<IFieldSymbol>().FirstOrDefault(static field => field.Name == "_cancellationToken");
			if (field is not null)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0010, field.Locations[0], messageArgs: null));
				return;
			}

			// Check the existence of the property 'Diagnostics'.
			var property = members.OfType<IPropertySymbol>().FirstOrDefault(property => property.Name == "Diagnostics");
			if (property is not null)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0011, property.Locations[0], messageArgs: null));
				return;
			}

			// Find the position of the suffix 'SyntaxChecker'.
			int indexOfSyntaxChecker = name.IndexOf("SyntaxChecker");
			if (indexOfSyntaxChecker == -1)
			{
				Diagnostics.Add(Diagnostic.Create(SCA0012, identifierName.GetLocation(), messageArgs: null));
			}

			// Valid name. Add it into the collection.
			Result.Add((name.Substring(0, indexOfSyntaxChecker), name, realValues.ToArray()));
		}
	}
}
