using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0208_ThisParameterNameAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0208UnitTest
{
	[TestMethod]
	public async Task TestCase_EmptyCode() => await VerifyCS.VerifyAnalyzerAsync(@"");

	[TestMethod]
	public async Task TestCase_Normal()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			namespace System;

			file static class Int32Extensions
			{
				public static void E(this int {|#0:integer|}, int p = 42)
				{
					Console.WriteLine(integer - p);
				}
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0208)).WithLocation(0)
		);
}
