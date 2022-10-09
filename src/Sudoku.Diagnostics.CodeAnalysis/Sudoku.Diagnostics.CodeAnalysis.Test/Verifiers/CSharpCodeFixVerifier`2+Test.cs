namespace Sudoku.Diagnostics.CodeAnalysis.Test;

public static partial class CSharpCodeFixVerifier<TAnalyzer, TCodeFix>
	where TAnalyzer : DiagnosticAnalyzer, new()
	where TCodeFix : CodeFixProvider, new()
{
	public class Test : CSharpCodeFixTest<TAnalyzer, TCodeFix, MSTestVerifier>
	{
		public Test()
			=> SolutionTransforms.Add(
				static (solution, projectId) =>
				{
					var compilationOptions = solution.GetProject(projectId)!.CompilationOptions;
					compilationOptions = compilationOptions!.WithSpecificDiagnosticOptions(
						compilationOptions
							.SpecificDiagnosticOptions
							.SetItems(CSharpVerifierHelper.NullableWarnings)
					);
					solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

					return solution;
				}
			);
	}
}
