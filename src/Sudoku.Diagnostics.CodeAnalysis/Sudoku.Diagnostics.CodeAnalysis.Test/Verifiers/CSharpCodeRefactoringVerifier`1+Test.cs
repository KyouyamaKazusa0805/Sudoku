namespace Sudoku.Diagnostics.CodeAnalysis.Test;

public static partial class CSharpCodeRefactoringVerifier<TCodeRefactoring> where TCodeRefactoring : CodeRefactoringProvider, new()
{
	public class Test : CSharpCodeRefactoringTest<TCodeRefactoring, MSTestVerifier>
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
