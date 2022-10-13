using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0302_SelfTypeParameterMustBeDerivedFromItselfAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0302UnitTest
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

			file interface ISelfType1<{|#0:[Self] TSelf|}>
			{
			}

			file interface ISelfType2<{|#1:[Self] TSelf|}> where TSelf : struct
			{
			}

			file interface ISelfType3<[Self] TSelf> where TSelf : struct, ISelfType3<TSelf>?
			{
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				[AttributeUsage(AttributeTargets.GenericParameter, Inherited = false)]
				file sealed class SelfAttribute : Attribute { }
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0302)).WithLocation(0),
			VerifyCS.Diagnostic(nameof(SCA0302)).WithLocation(1)
		);

	[TestMethod]
	public async Task TestCase_MultipleTypeParameters()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			using System;
			using System.Diagnostics.CodeAnalysis;

			file interface ISelfType4<[Self] TSelf, T> where TSelf : ISelfType4<TSelf, T>
			{
			}

			file interface ISelfType5<{|#0:[Self] TSelf|}, T>
			{
			}

			namespace System.Diagnostics.CodeAnalysis
			{
				[AttributeUsage(AttributeTargets.GenericParameter, Inherited = false)]
				file sealed class SelfAttribute : Attribute { }
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0302)).WithLocation(0)
		);
}
