#pragma warning disable IDE1006

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verifier = Sudoku.CodeAnalysis.Test.CSharpCodeFixVerifier<
	Sudoku.CodeAnalysis.SudokuCodeAnalysisAnalyzer,
	Sudoku.CodeAnalysis.SudokuCodeAnalysisCodeFixProvider
>;

namespace Sudoku.CodeAnalysis.Test
{
	/// <summary>
	/// The unit test for that analyzer.
	/// </summary>
	[TestClass]
	public sealed class SudokuCodeAnalysisUnitTest
	{
		/// <summary>
		/// The method that is only used for verifying the analyzer.
		/// </summary>
		/// <returns>The task of the method.</returns>
		[TestMethod]
		public async Task VerifyOnly()
		{
			string test = @"";

			await Verifier.VerifyAnalyzerAsync(test);
		}

		/// <summary>
		/// The method that is used for both verifying and code fixing.
		/// </summary>
		/// <returns>The task of the method.</returns>
#if false
		[TestMethod]
#endif
		public async Task VerifyAndFix()
		{
			string test = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;

	namespace ConsoleApplication1
	{
		class {|#0:TypeName|}
		{   
		}
	}";

			string fixtest = @"
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Diagnostics;

	namespace ConsoleApplication1
	{
		class TYPENAME
		{   
		}
	}";

			var expected = Verifier.Diagnostic("SudokuCodeAnalysis").WithLocation(0).WithArguments("TypeName");
			await Verifier.VerifyCodeFixAsync(test, expected, fixtest);
		}
	}
}
