using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0107_FieldMustBeFunctionPointerTypeAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0107UnitTest
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

			file readonly unsafe struct TestType
			{
				[DisallowFunctionPointerInvocation]
				public static readonly void* F1 = (delegate*<void>)&Method;

				[DisallowFunctionPointerInvocation]
				public static readonly delegate*<void> F2 = &Method;

				[DisallowFunctionPointerInvocation]
				public static readonly int {|#0:F3 = 42|};


				private static void Method() { }
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				[AttributeUsage(AttributeTargets.Field, Inherited = false)]
				file sealed class DisallowFunctionPointerInvocationAttribute : Attribute
				{
				}
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0107)).WithLocation(0)
		);
}
