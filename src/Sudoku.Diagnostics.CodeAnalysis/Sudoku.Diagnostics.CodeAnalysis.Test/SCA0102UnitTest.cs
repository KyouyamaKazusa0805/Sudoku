using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0102_LargeStructureShouldPassByRefAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0102UnitTest
{
	[TestMethod]
	public async Task TestCase_EmptyCode() => await VerifyCS.VerifyAnalyzerAsync(@"");

	[TestMethod]
	public async Task TestCase_Normal()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			using System.Diagnostics.CodeAnalysis;

			file sealed class Program
			{
				private static void MethodNoParameter()
				{
				}

				private static void Method({|#0:Grid grid|})
				{
				}

				private static int Function(out Grid grid)
				{
					grid = Grid.Empty;
					return 42;
				}
			}

			[IsLargeStruct(SuggestedMemberName = nameof(Empty))]
			file struct Grid
			{
				public static readonly Grid Empty = default;
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				file sealed class IsLargeStructAttribute : Attribute
				{
					public string? SuggestedMemberName { get; set; }
				}
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0102)).WithLocation(0)
		);
}
