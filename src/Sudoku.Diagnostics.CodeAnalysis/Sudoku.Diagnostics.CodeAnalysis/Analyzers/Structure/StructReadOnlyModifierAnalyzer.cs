namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS9005F")]
public sealed partial class StructReadOnlyModifierAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.PropertyDeclaration });
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		var (semanticModel, _, originalNode) = context;

		if (
			originalNode is not PropertyDeclarationSyntax
			{
				Parent: StructDeclarationSyntax,
				Modifiers: { Count: not 0 } modifiers,
				ExpressionBody: null,
				AccessorList: { Accessors: { Count: var accessorsCount and not 0 } accessors }
			} node
		)
		{
			return;
		}

		Action? a = accessorsCount switch
		{
			// readonly int Prop { get; }
			1 when accessors[0] is
			{
				Keyword: { RawKind: (int)SyntaxKind.GetKeyword },
				Modifiers: { Count: var count } getterModifiers,
				Body: null,
				ExpressionBody: null
			}
			&& (count == 0 || count != 0 && getterModifiers.All(isNotReadOnlyKeyword))
			&& modifiers.FirstOrDefault(isReadOnlyKeyword) is var possibleReadOnlyModifier
			&& possibleReadOnlyModifier != default => () => f(possibleReadOnlyModifier, node),

			// int Prop { readonly get; set; }
			2 when accessors[0] is
			{
				Keyword: { RawKind: (int)SyntaxKind.GetKeyword },
				Modifiers: { Count: var count } getterModifiers,
				Body: null,
				ExpressionBody: null
			} getAccessor
			&& (count == 0 || count != 0 && modifiers.All(isNotReadOnlyKeyword))
			&& getterModifiers.FirstOrDefault(isReadOnlyKeyword) is var possibleReadOnlyModifier
			&& possibleReadOnlyModifier != default => () => f(possibleReadOnlyModifier, getAccessor),

			_ => null
		};

		a?.Invoke();


		void f(SyntaxToken token, SyntaxNode node)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS9005,
					location: token.GetLocation(),
					properties: ImmutableDictionary.CreateRange(
						new KeyValuePair<string, string?>[]
						{
							new("AccessorsCount", accessorsCount.ToString())
						}
					),
					messageArgs: null,
					additionalLocations: new[] { node.GetLocation() }
				)
			);
		}

		static bool isReadOnlyKeyword(SyntaxToken t) => t.RawKind == (int)SyntaxKind.ReadOnlyKeyword;
		static bool isNotReadOnlyKeyword(SyntaxToken t) => t.RawKind != (int)SyntaxKind.ReadOnlyKeyword;
	}
}
