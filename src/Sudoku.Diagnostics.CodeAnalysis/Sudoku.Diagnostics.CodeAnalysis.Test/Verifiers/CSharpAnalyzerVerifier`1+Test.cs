namespace Sudoku.Diagnostics.CodeAnalysis.Test;

public static partial class CSharpAnalyzerVerifier<TAnalyzer> where TAnalyzer : DiagnosticAnalyzer, new()
{
	public class Test : CSharpAnalyzerTest<TAnalyzer, MSTestVerifier>
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

					return solution.WithProjectCompilationOptions(projectId, compilationOptions);
				}
			);
	}
}
