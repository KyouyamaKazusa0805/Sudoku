using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0115_SelfTypeParameterShouldNameTSelfAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0115UnitTest
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

			file interface ISelfType<{|#0:[Self] T|}> where T : ISelfType<T>
			{
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				[AttributeUsage(AttributeTargets.GenericParameter, Inherited = false)]
				file sealed class SelfAttribute : Attribute { }
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0115)).WithLocation(0)
		);
}
