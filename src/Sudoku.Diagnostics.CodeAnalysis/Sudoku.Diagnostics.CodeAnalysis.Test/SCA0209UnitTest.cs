using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0209_ExtensionMethodTypesShouldBeSameAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0209UnitTest
{
	[TestMethod]
	public async Task TestCase_EmptyCode() => await VerifyCS.VerifyAnalyzerAsync(@"");

	[TestMethod]
	public async Task TestCase_Normal()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			using System;

			namespace System;

			file static class {|#0:Int32Extensions|}
			{
				public static void E(this int @this) { }
				public static void F(this double @this) { }
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0209)).WithLocation(0)
		);
}
