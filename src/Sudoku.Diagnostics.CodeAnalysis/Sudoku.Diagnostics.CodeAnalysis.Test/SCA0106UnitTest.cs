using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0106_DisallowInvocationOnFunctionPointerMemberAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0106UnitTest
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

			file sealed class Program
			{
				private static unsafe void Method()
				{
					{|#0:TestType.FunctionPointer()|}; // Should be disallowed.
				}
			}

			file readonly struct TestType
			{
				[DisallowFunctionPointerInvocation]
				public static readonly unsafe delegate*<void> FunctionPointer = &Method;

				private static void Method()
				{
				}

				private static unsafe void InvocationInner()
				{
					FunctionPointer(); // Should be allowed.
				}
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				[AttributeUsage(AttributeTargets.Field, Inherited = false)]
				file sealed class DisallowFunctionPointerInvocationAttribute : Attribute
				{
				}
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0106)).WithLocation(0)
		);
}
