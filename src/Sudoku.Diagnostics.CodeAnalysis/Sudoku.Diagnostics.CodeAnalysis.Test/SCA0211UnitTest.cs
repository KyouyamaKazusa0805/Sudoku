using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0211_NamespaceShouldMatchExtensionMethodThisParameterTypeAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0211UnitTest
{
	[TestMethod]
	public async Task TestCase_EmptyCode() => await VerifyCS.VerifyAnalyzerAsync(@"");

	[TestMethod]
	public async Task TestCase_Normal()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			namespace Extensions;

			file static class {|#0:Int32Extensions|}
			{
				public static void E(this int @this) { }
				public static void F(this int @this) { }
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0211)).WithLocation(0)
		);
}
