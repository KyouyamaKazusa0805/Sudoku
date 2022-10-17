using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0205_DoNotUseUsingOnStringHandlerVariableAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0205UnitTest
{
	[TestMethod]
	public async Task TestCase_EmptyCode() => await VerifyCS.VerifyAnalyzerAsync(@"");

	[TestMethod]
	public async Task TestCase_Normal()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			using System;
			using System.Text;

			file sealed class Program
			{
				private static void Case()
				{
					using scoped var {|#0:sb|} = new StringHandler();
					sb.Append(1);
					sb.Append("2");
					sb.Append(3.0);
					sb.Append(true);

					var result = sb.ToStringAndClear();
					Console.WriteLine(result);
				}
			}

			namespace System.Text
			{
				file ref struct StringHandler
				{
					public readonly int Count => 42;

					public void Append<T>(T variable) { }
					public string ToStringAndClear() => string.Empty;
					public override readonly string ToString() => string.Empty;
					public void Dispose() { }
				}
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0205)).WithLocation(0)
		);
}
