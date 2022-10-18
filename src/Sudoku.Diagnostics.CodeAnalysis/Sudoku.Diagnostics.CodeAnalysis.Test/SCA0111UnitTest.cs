using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0111_FileAccessOnlyForConstructorAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0111UnitTest
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
				internal TestType()
				{
				}
			}

			file sealed class AnotherType
			{
				public void Method()
				{
					var variable = new TestType();
				}
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				[AttributeUsage(AttributeTargets.Constructor, Inherited = false)]
				file sealed class FileAccessOnlyAttribute : Attribute
				{
				}
			}
			"""
		);
}
