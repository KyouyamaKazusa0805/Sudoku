using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0215_ExpressionSimplifyAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0215UnitTest
{
	[TestMethod]
	public async Task TestCase_EmptyCode() => await VerifyCS.VerifyAnalyzerAsync(@"");

	[TestMethod]
	public async Task TestCase_Normal()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			using System;
			using Sudoku.Concepts;

			file sealed class Program
			{
				public void TestCase()
				{
					var map1 = CellMap.Empty;
					var map2 = CellMap.Empty;
					if ({|#0:map1.Count != 0|})
					{
					}

					if (map1.Count != 0 && map2.Count != 0)
					{
					}

					if ({|#1:(map1 & map2).Count != 0|})
					{
					}

					if (map1 && map2)
					{
					}
				}
			}

			namespace Sudoku.Concepts
			{
				file struct CellMap
				{
					public static readonly CellMap Empty;
					
					public readonly int Count => 42;
					
					public static bool operator true(scoped in CellMap m) => throw new();
					public static bool operator false(scoped in CellMap m) => throw new();
					public static bool operator ==(scoped in CellMap m, scoped in CellMap n) => throw new();
					public static bool operator !=(scoped in CellMap m, scoped in CellMap n) => throw new();
					public static CellMap operator &(scoped in CellMap m, scoped in CellMap n) => throw new();
					public static CellMap operator |(scoped in CellMap m, scoped in CellMap n) => throw new();
				}
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0215)).WithLocation(0).WithArguments("map1"),
			VerifyCS.Diagnostic(nameof(SCA0215)).WithLocation(1).WithArguments("(map1 & map2)")
		);
}
