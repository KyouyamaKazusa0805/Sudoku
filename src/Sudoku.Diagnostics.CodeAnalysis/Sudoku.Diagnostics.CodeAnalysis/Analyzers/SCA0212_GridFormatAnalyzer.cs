namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

using static LocalConstant;

[SupportedDiagnostics("SCA0001", "SCA0212", "SCA0213")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.InvocationExpression))]
public sealed partial class SCA0212_GridFormatAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SyntaxNodeAnalysisContext context)
	{
		if (context is not
			{
				Node: InvocationExpressionSyntax node,
				Compilation: var compilation,
				SemanticModel: var semanticModel,
				CancellationToken: var ct
			})
		{
			return;
		}

		if (semanticModel.GetOperation(node, ct) is not IInvocationOperation
			{
				TargetMethod:
				{
					ContainingType: var type,
					Name: nameof(ToString),
					Parameters: [{ Type.SpecialType: SpecialType.System_String }, ..],
					IsStatic: false
				},
				Arguments: { Length: 1 or 2 } and [{ Value.ConstantValue: { HasValue: true, Value: var format and (null or string) } }, ..]
			})
		{
			return;
		}

		var location = node.GetLocation();
		if (compilation.GetTypeByMetadataName(SpecialFullTypeNames.Grid) is not { } gridType)
		{
			context.ReportDiagnostic(Diagnostic.Create(SCA0001, location, messageArgs: new[] { SpecialFullTypeNames.Grid }));
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(type, gridType))
		{
			return;
		}

		var realFormat = (string?)format;
		if (realFormat is not null && Array.IndexOf(SupportedFormats, realFormat) == -1)
		{
			context.ReportDiagnostic(Diagnostic.Create(SCA0212, location));
			return;
		}

		if (realFormat is "#." or "+:" or [.. ".+" or "+." or "0+" or "+0", ':'])
		{
			context.ReportDiagnostic(Diagnostic.Create(SCA0213, location));
		}
	}
}

/// <summary>
/// Stores the local constants.
/// </summary>
file static class LocalConstant
{
	public static readonly string[] SupportedFormats = new[]
	{
		".", "+", ".+", "+.", "0", ":", "!", ".!", "!.", "0!", "!0", ".:", "0:",
		"0+", "+0", "+:", "+.:", ".+:", "#", "#.", "0+:", "+0:", "#0",
		".!:", "!.:", "0!:", "!0:", ".*", "*.", "0*", "*0", "@", "@.", "@0", "@!", "@.!", "@!.", "@0!", "@!0",
		"@*", "@.*", "@*.", "@0*", "@*0", "@!*", "@*!", "@:", "@:!", "@!:", "@*:", "@:*",
		"@!*:", "@*!:", "@!:*", "@*:!", "@:!*", "@:*!", "~", "~0", "~.", "@~", "~@", "@~0", "@0~", "~@0", "~0@",
		"@~.", "@.~", "~@.", "~.@", "%", "^"
	};
}
