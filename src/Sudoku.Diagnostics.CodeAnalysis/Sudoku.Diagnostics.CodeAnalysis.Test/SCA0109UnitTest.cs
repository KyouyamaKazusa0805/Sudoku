using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0109_FileAccessOnlyAttributeAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0109UnitTest
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
				internal static readonly int ReadOnlyVariable = 42;
			}

			file sealed class AnotherType
			{
				public void Method()
				{
					int variable = TestType.ReadOnlyVariable;
				}
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				[AttributeUsage(AttributeTargets.Field, Inherited = false)]
				file sealed class FileAccessOnlyAttribute : Attribute
				{
				}
			}
			"""
		);
}
