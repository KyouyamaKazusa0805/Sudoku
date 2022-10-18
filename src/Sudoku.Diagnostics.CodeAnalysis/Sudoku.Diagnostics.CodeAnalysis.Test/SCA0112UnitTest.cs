using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0112_NoNeedFileAccessOnlyAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0112UnitTest
{
	[TestMethod]
	public async Task TestCase_EmptyCode() => await VerifyCS.VerifyAnalyzerAsync(@"");

	[TestMethod]
	public async Task TestCase_Normal()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			using System;
			using System.Diagnostics.CodeAnalysis;

			file sealed class TestType
			{
				[FileAccessOnly]
				internal const string {|#0:Field|} = "";

				[FileAccessOnly]
				internal {|#1:TestType|}()
				{
				}
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				[AttributeUsage(AttributeTargets.Field | AttributeTargets.Constructor, Inherited = false)]
				file sealed class FileAccessOnlyAttribute : Attribute
				{
				}
			}
			""",
			VerifyCS.Diagnostic(SCA0112).WithLocation(0),
			VerifyCS.Diagnostic(SCA0112).WithLocation(1)
		);
}
