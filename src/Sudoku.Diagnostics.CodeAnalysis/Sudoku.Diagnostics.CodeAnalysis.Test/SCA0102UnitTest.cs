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

			[IsLargeStruct]
			file struct Grid
			{
				public static readonly Grid Empty = default;
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				file sealed class IsLargeStructAttribute : Attribute
				{
				}
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0102)).WithLocation(0)
		);

	[TestMethod]
	public async Task TestCase_IgnoreExplicitInterface()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			using System.Diagnostics.CodeAnalysis;

			file sealed class C : I<Grid>
			{
				void I<Grid>.M(Grid element) { }
			}

			[IsLargeStruct]
			file struct Grid
			{
			}

			file interface I<T>
			{
				void M(T element);
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				file sealed class IsLargeStructAttribute : Attribute
				{
				}
			}
			"""
		);
}
