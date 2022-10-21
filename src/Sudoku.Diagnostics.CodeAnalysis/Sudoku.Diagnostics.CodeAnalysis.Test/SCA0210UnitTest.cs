using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0210_ExtensionMethodContainingTypeNameAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0210UnitTest
{
	[TestMethod]
	public async Task TestCase_EmptyCode() => await VerifyCS.VerifyAnalyzerAsync(@"");

	[TestMethod]
	public async Task TestCase_Normal()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			namespace System;

			file static class {|#0:Hello|}
			{
				public static void E(this int @this) { }
				public static void F(this int @this) { }
			}

			file static class AnotherExtensions
			{
				public static void E(this int @this) { }
				public static void F(this int @this) { }
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0210)).WithLocation(0)
		);
}
