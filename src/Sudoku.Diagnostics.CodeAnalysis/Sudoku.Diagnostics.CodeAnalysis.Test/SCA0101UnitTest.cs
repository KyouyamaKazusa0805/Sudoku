using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0101_LargeStructTypeAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0101UnitTest
{
	[TestMethod]
	public async Task TestCase_EmptyCode() => await VerifyCS.VerifyAnalyzerAsync(@"");

	[TestMethod]
	public async Task TestCase_Normal()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			using System.Diagnostics.CodeAnalysis;

			file sealed class Program
			{
				private static void Main()
				{
					var field = {|#0:new Grid()|};
				}
			}

			[IsLargeStruct]
			file struct Grid
			{
				public static readonly Grid Empty = default;
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				file sealed class IsLargeStructAttribute : Attribute
				{
				}
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0101)).WithLocation(0)
		);
}
