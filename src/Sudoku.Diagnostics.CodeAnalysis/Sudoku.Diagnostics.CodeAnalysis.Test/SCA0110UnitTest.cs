using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0110_FileAccessOnlyAttributePrivateFieldsAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0110UnitTest
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
				[FileAccessOnly]
				private static readonly int {|#0:ReadOnlyVariable|} = 42;

				public void Method()
				{
					var integer = ReadOnlyVariable;
					Console.WriteLine(integer);
				}
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				[AttributeUsage(AttributeTargets.Field, Inherited = false)]
				file sealed class FileAccessOnlyAttribute : Attribute
				{
				}
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0110)).WithLocation(0).WithArguments("ReadOnlyVariable")
		);
}
