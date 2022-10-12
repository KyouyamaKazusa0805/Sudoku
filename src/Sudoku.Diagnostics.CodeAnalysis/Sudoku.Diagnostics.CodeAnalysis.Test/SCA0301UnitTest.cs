using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0301_SelfAttributeMissingAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0301UnitTest
{
	[TestMethod]
	public async Task TestCase_EmptyCode() => await VerifyCS.VerifyAnalyzerAsync(@"");

	[TestMethod]
	public async Task TestCase_Normal()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			using System;

			file interface ISelfType<{|#0:TSelf|}> where TSelf : ISelfType<TSelf>
			{
			}

			file interface IAnotherSelfType<{|#1:TSelf|}> where TSelf : IAnotherSelfType<TSelf>?
			{
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				[AttributeUsage(AttributeTargets.GenericParameter, Inherited = false)]
				file sealed class SelfAttribute : Attribute { }
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0301)).WithLocation(0),
			VerifyCS.Diagnostic(nameof(SCA0301)).WithLocation(1)
		);
}
