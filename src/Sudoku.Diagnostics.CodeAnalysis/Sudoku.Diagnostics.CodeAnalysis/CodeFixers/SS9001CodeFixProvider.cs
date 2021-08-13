namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS9001")]
public sealed partial class SS9001CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SS9001);
		var semanticModel = (await document.GetSemanticModelAsync(context.CancellationToken))!;
		var compilation = semanticModel.Compilation;
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var ((_, badExprSpan), descriptor) = diagnostic;
		var badExpression = (ExpressionSyntax)root.FindNode(badExprSpan, getInnermostNodeForTie: true);
		var (_, forLoopSpan) = diagnostic.AdditionalLocations[0];
		var forLoop = (ForStatementSyntax)root.FindNode(forLoopSpan, getInnermostNodeForTie: true);

		if (forLoop.Declaration is not { } declaration)
		{
			return;
		}

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS9001,
				createChangedDocument: async c =>
				{
					// Now define a temporary name to be replaced.
					const string systemSuggestedDefaultVariableName = "variableName";
					string suggestedName = diagnostic.Properties["SuggestedName"]!;
					string resultDefaultVariableName = suggestedName ?? systemSuggestedDefaultVariableName;

					// Now we can change and replace the syntax nodes.
					var editor = await DocumentEditor.CreateAsync(document, c);

					// Check whether the 'variableDeclaration' part is empty.
					//
					//  The 'variableDeclaration' part
					//                  ↓
					//    for (variableDeclaration; condition; incrementors)
					//    {
					//        ...
					//    }
					//
					// If 'variableDeclaration' is empty, we just need to create a new list.
					// Check the for loop statement node is valid to replace.
					if (
						declaration is
						{
							Type: { } declarationType,
							Variables: { Count: not 0 } variableDeclarators
						}
					)
					{
						// Check whether the result variable name is duplicate
						// in the old variable declarator list.
						var flowAnalysis = semanticModel.AnalyzeDataFlow(forLoop);
						string[]? listOfVariablesOutside = flowAnalysis is null
							? null
							: (
								from variable in flowAnalysis.VariablesDeclared
								select variable.Name into name
								where !string.IsNullOrEmpty(name)
								select name
							).Concat(
								from variable in flowAnalysis.ReadOutside
								select variable.Name
							).Concat(
								from variable in flowAnalysis.WrittenOutside
								select variable.Name
							).Concat(
								from method in flowAnalysis.UsedLocalFunctions
								from variable in method.Parameters
								select variable.Name
							).Concat(
								from variable in flowAnalysis.CapturedOutside
								select variable.Name
							).Distinct().ToArray();

						if (Array.IndexOf(listOfVariablesOutside, resultDefaultVariableName) != -1)
						{
							for (uint trial = 1; ; trial = checked(trial + 1))
							{
								bool exists = false;
								string currentPossibleName = $"{resultDefaultVariableName}{trial}";
								foreach (var variableDeclarator in variableDeclarators)
								{
									if (variableDeclarator.Identifier.ValueText == currentPossibleName)
									{
										exists = true;
										break;
									}
								}

								if (!exists && Array.IndexOf(listOfVariablesOutside, currentPossibleName) == -1)
								{
									resultDefaultVariableName = currentPossibleName;
									break;
								}
							}
						}

						editor.ReplaceNode(
							declaration,
							SyntaxFactory.VariableDeclaration(
								declarationType,
								SyntaxFactory.SeparatedList(
									new List<VariableDeclaratorSyntax>(variableDeclarators)
									{
										SyntaxFactory.VariableDeclarator(
											SyntaxFactory.Identifier(
												resultDefaultVariableName
											)
										)
										.WithInitializer(
											SyntaxFactory.EqualsValueClause(
												badExpression
											)
										)
										.NormalizeWhitespace(indentation: "\t")
									}
								)
							)
						);
						editor.ReplaceNode(
							badExpression,
							SyntaxFactory.IdentifierName(
								resultDefaultVariableName
							)
						);
					}

					// Returns the changed document.
					return editor.GetChangedDocument();
				},
				equivalenceKey: nameof(CodeFixTitles.SS9001)
			),
			diagnostic
		);
	}


	/// <summary>
	/// Check whether the specified type symbol is built-in type.
	/// </summary>
	/// <param name="symbol">The symbol.</param>
	/// <param name="compilation">The compilation.</param>
	/// <returns>
	/// The method will return:
	/// <list type="bullet">
	/// <item>
	/// <term><see cref="PredefinedTypeSyntax"/></term>
	/// <description>The type is built-in type.</description>
	/// </item>
	/// <item>
	/// <term><see cref="IdentifierNameSyntax"/></term>
	/// <description>The type is <see langword="nint"/> or <see langword="nuint"/>.</description>
	/// </item>
	/// <item>
	/// <term><see langword="var"/> type</term>
	/// <description>Otherwise.</description>
	/// </item>
	/// </list>
	/// </returns>
	private static TypeSyntax GetTypeSyntax(ITypeSymbol symbol, Compilation compilation)
	{
		Func<ISymbol?, ISymbol?, bool> e = SymbolEqualityComparer.Default.Equals;
		Func<SyntaxToken, PredefinedTypeSyntax> r = SyntaxFactory.PredefinedType;
		Func<SyntaxKind, SyntaxToken> t = SyntaxFactory.Token;
		Func<SpecialType, INamedTypeSymbol> c = compilation.GetSpecialType;
		Func<string, IdentifierNameSyntax> i = SyntaxFactory.IdentifierName;

		if (e(symbol, c(SpecialType.System_Byte)))
			return r(t(SyntaxKind.ByteKeyword));
		else if (e(symbol, c(SpecialType.System_SByte)))
			return r(t(SyntaxKind.SByteKeyword));
		else if (e(symbol, c(SpecialType.System_Int16)))
			return r(t(SyntaxKind.ShortKeyword));
		else if (e(symbol, c(SpecialType.System_UInt16)))
			return r(t(SyntaxKind.UShortKeyword));
		else if (e(symbol, c(SpecialType.System_Int32)))
			return r(t(SyntaxKind.IntKeyword));
		else if (e(symbol, c(SpecialType.System_UInt32)))
			return r(t(SyntaxKind.UIntKeyword));
		else if (e(symbol, c(SpecialType.System_Int64)))
			return r(t(SyntaxKind.LongKeyword));
		else if (e(symbol, c(SpecialType.System_UInt64)))
			return r(t(SyntaxKind.ULongKeyword));
		else if (e(symbol, c(SpecialType.System_Char)))
			return r(t(SyntaxKind.CharKeyword));
		else if (e(symbol, c(SpecialType.System_String)))
			return r(t(SyntaxKind.StringKeyword));
		else if (e(symbol, c(SpecialType.System_Single)))
			return r(t(SyntaxKind.FloatKeyword));
		else if (e(symbol, c(SpecialType.System_Double)))
			return r(t(SyntaxKind.DoubleKeyword));
		else if (e(symbol, c(SpecialType.System_Decimal)))
			return r(t(SyntaxKind.DecimalKeyword));
		else if (e(symbol, c(SpecialType.System_Boolean)))
			return r(t(SyntaxKind.BoolKeyword));
		else if (e(symbol, c(SpecialType.System_Object)))
			return r(t(SyntaxKind.ObjectKeyword));
		else if (e(symbol, c(SpecialType.System_IntPtr)) && symbol.IsNativeIntegerType)
			return i("nint");
		else if (e(symbol, c(SpecialType.System_UIntPtr)) && symbol.IsNativeIntegerType)
			return i("nuint");
		else
			return i("var");
	}
}
