#if false

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0621", "SS0622")]
public sealed partial class AvailableAndOrPatternsToMergeAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(
			static context =>
			{
				CheckSS0621(context, context.Node);
				CheckSS0622(context, context.Node);
			},
			new[] { SyntaxKind.AndPattern, SyntaxKind.OrPattern }
		);
	}


	private static void CheckSS0621(SyntaxNodeAnalysisContext context, SyntaxNode originalNode)
	{
		if (
			originalNode is not BinaryPatternSyntax
			{
				Parent: not { RawKind: (int)SyntaxKind.AndPattern },
				RawKind: (int)SyntaxKind.AndPattern,
				Left: var leftPattern,
				Right: var rightPattern
			}
		)
		{
			return;
		}

		if (!checkRecursively(context, new[] { leftPattern, rightPattern }))
		{
			return;
		}

		context.ReportDiagnostic(
			Diagnostic.Create(
				descriptor: SS0621,
				location: originalNode.GetLocation(),
				messageArgs: null
			)
		);


		static bool checkRecursively(in SyntaxNodeAnalysisContext context, PatternSyntax[] patterns)
		{
			foreach (var pattern in patterns)
			{
				switch (pattern)
				{
					case BinaryPatternSyntax
					{
						RawKind: (int)SyntaxKind.AndPattern,
						Left: var nestedLeftPattern,
						Right: var nestedRightPattern
					}:
					{
						return checkRecursively(context, new[] { nestedLeftPattern, nestedRightPattern });
					}
					case { RawKind: (int)SyntaxKind.OrPattern or (int)SyntaxKind.ParenthesizedPattern }:
					{
						return false;
					}
				}
			}

			return true;
		}
	}

	private static void CheckSS0622(SyntaxNodeAnalysisContext context, SyntaxNode originalNode)
	{
		if (
			originalNode is not BinaryPatternSyntax
			{
				Parent: not { RawKind: (int)SyntaxKind.OrPattern or (int)SyntaxKind.ParenthesizedPattern },
				RawKind: (int)SyntaxKind.OrPattern,
				Left: var leftPattern,
				Right: var rightPattern
			}
		)
		{
			return;
		}

		if (!checkRecursively(context, new[] { leftPattern, rightPattern }))
		{
			return;
		}

		context.ReportDiagnostic(
			Diagnostic.Create(
				descriptor: SS0621,
				location: originalNode.GetLocation(),
				messageArgs: null
			)
		);


		static bool checkRecursively(in SyntaxNodeAnalysisContext context, PatternSyntax[] patterns)
		{
			foreach (var pattern in patterns)
			{
				switch (pattern)
				{
					case BinaryPatternSyntax
					{
						RawKind: (int)SyntaxKind.OrPattern,
						Left: var nestedLeftPattern,
						Right: var nestedRightPattern
					}:
					{
						return checkRecursively(context, new[] { nestedLeftPattern, nestedRightPattern });
					}
					case { RawKind: (int)SyntaxKind.AndPattern or (int)SyntaxKind.ParenthesizedPattern }:
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}


#endif