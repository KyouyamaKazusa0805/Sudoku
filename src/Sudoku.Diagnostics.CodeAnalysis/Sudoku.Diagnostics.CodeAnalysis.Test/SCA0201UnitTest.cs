using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0201_UseArgumentAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0201UnitTest
{
	[TestMethod]
	public async Task TestCase_EmptyCode() => await VerifyCS.VerifyAnalyzerAsync(@"");

	[TestMethod]
	public async Task TestCase_NotTrigger()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			using System;
			using System.Diagnostics.CodeAnalysis;

			file sealed class Program
			{
				private static void Case()
				{
					string? s = null;
					if (s is null or [])
					{
						throw new ArgumentException("s");
					}
				}
			}
			"""
		);

	[TestMethod]
	public async Task TestCase_IsPattern()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			using System;
			using System.Diagnostics.CodeAnalysis;

			file sealed class Program
			{
				private static void Case1()
				{
					string? s = null;
					{|#0:if (s is null)
					{
						throw new ArgumentNullException(nameof(s));
					}|}
				}
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0201)).WithLocation(0)
		);

	[TestMethod]
	public async Task TestCase_EqualityOperator()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			using System;
			using System.Diagnostics.CodeAnalysis;

			file sealed class Program
			{
				private static void Case2()
				{
					string? s = null;
					{|#0:if (s == null)
					{
						throw new ArgumentNullException("s");
					}|}
				}
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0201)).WithLocation(0)
		);
}
