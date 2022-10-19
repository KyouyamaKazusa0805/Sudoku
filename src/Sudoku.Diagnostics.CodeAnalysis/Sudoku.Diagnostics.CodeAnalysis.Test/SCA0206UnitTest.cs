using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0206_UseExplicitCastAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0206UnitTest
{
	[TestMethod]
	public async Task TestCase_EmptyCode() => await VerifyCS.VerifyAnalyzerAsync(@"");

	[TestMethod]
	public async Task TestCase_Normal()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			using System;
			using Sudoku.Concepts;

			file sealed class Program
			{
				private static void Case()
				{
					var grid = {|#0:Grid.Parse("6...8..4..+4.....862..4.67+5.+1+5397+486+2.7...+3+51+4+462+5183+9+7..61.2+4+7572+4+8...+3..1..4+7.+28:314 916 921 325 925 932 933 371 985 991")|};
				}
			}

			namespace Sudoku.Concepts
			{
				file struct Grid
				{
					public static Grid Parse(string s) => throw new();

					public static explicit operator Grid(string? s) => throw new();
				}
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0206)).WithLocation(0)
		);
}
