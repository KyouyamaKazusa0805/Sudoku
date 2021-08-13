namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0402", "SS0403")]
public sealed partial class FlaggedEnumFieldAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.EnumDeclaration });
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		var (semanticModel, _, n) = context;

		if (n is not EnumDeclarationSyntax { AttributeLists: { Count: not 0 } attributeLists } node)
		{
			return;
		}

		if (
			!attributeLists.Any(
				static attributeList =>
					attributeList is { Attributes: { Count: not 0 } attributes }
					&& attributes.Any(
						static attribute => attribute.Name is IdentifierNameSyntax
						{
							Identifier.ValueText: "Flags" or nameof(FlagsAttribute)
						}
					)
			)
		)
		{
			return;
		}

		long flagList = 0;
		foreach (var fieldDeclaration in node.DescendantNodes().OfType<EnumMemberDeclarationSyntax>())
		{
			switch (fieldDeclaration.EqualsValue)
			{
				case null:
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0403,
							location: fieldDeclaration.GetLocation(),
							messageArgs: null,
							properties: ImmutableDictionary.CreateRange(
								new KeyValuePair<string, string?>[]
								{
									new(
										"NextPossibleFlag",
										(1 << TrailingZeroCount((ulong)~flagList)).ToString()
									)
								}
							)
						)
					);

					break;
				}
				case { Value: var expression }
				when semanticModel.GetOperation(expression) is ILiteralOperation
				{
					ConstantValue: { HasValue: true, Value: var value and (int or long) }
				}:
				{
					switch (value)
					{
						case int i when (i & i - 1) == 0:
						{
							flagList |= (long)i;

							continue;
						}
						case long l when (l & l - 1) == 0:
						{
							flagList |= l;

							continue;
						}
					}

					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0402,
							location: expression.GetLocation(),
							messageArgs: null,
							properties: ImmutableDictionary.CreateRange(
								new KeyValuePair<string, string?>[]
								{
									new(
										"NextPossibleFlag",
										(1 << TrailingZeroCount((ulong)~flagList)).ToString()
									)
								}
							)
						)
					);

					break;
				}
			}
		}
	}


	private static ReadOnlySpan<byte> TrailingZeroCountDeBruijn => new byte[32]
	{
		00, 01, 28, 02, 29, 14, 24, 03,
		30, 22, 20, 15, 25, 17, 04, 08,
		31, 27, 13, 23, 21, 19, 16, 07,
		26, 12, 18, 06, 11, 05, 10, 09
	};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int TrailingZeroCount(ulong value)
	{
		uint lo = (uint)value;
		return lo == 0 ? 32 + TrailingZeroCount((uint)(value >> 32)) : TrailingZeroCount(lo);
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int TrailingZeroCount(uint value) => value == 0
		? 32
		: Unsafe.AddByteOffset(
			ref MemoryMarshal.GetReference(TrailingZeroCountDeBruijn),
			(IntPtr)(int)(((value & (uint)-(int)value) * 0x077CB531U) >> 27)
		);
}
