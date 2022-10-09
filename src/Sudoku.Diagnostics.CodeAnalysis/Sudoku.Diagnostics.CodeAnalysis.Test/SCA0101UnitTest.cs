using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpCodeFixVerifier<
	Sudoku.Diagnostics.CodeAnalysis.SCA0101_LargeStructTypeAnalyzer,
	Sudoku.Diagnostics.CodeAnalysis.SCA0101CodeFixProvider
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0101UnitTest
{
	[TestMethod]
	public async Task TestCase_EmptyCode() => await VerifyCS.VerifyAnalyzerAsync(@"");

	[TestMethod]
	public async Task TestCase_Normal()
		=> await VerifyCS.VerifyCodeFixAsync(
			"""
			#nullable enable

			using System.Diagnostics.CodeAnalysis;

			file sealed class Program
			{
				private static void Main()
				{
					var field = {|#0:new Grid()|};
				}
			}

			[LargeStruct(SuggestedMemberName = nameof(Empty))]
			file struct Grid
			{
				public static readonly Grid Empty = default;
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				file sealed class LargeStructAttribute : Attribute
				{
					public string? SuggestedMemberName { get; set; }
				}
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0101)).WithLocation(0),
			"""
			#nullable enable

			using System.Diagnostics.CodeAnalysis;

			file sealed class Program
			{
				private static void Main()
				{
					var field = Grid.Empty;
				}
			}
			
			[LargeStruct(SuggestedMemberName = nameof(Empty))]
			file struct Grid
			{
				public static readonly Grid Empty = default;
			}
			
			namespace System.Diagnostics.CodeAnalysis
			{
				file sealed class LargeStructAttribute : Attribute
				{
					public string? SuggestedMemberName { get; set; }
				}
			}
			"""
		);
}
