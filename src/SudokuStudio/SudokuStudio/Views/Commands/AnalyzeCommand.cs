namespace SudokuStudio.Views.Commands;

/// <summary>
/// Defines a command to analyze a puzzle.
/// </summary>
public sealed class AnalyzeCommand : ButtonCommand
{
	/// <inheritdoc/>
	public override async void Execute(object? parameter)
	{
		if (parameter is not AnalyzePage self)
		{
			return;
		}

		var puzzle = self.SudokuPane.Puzzle;
		if (!puzzle.IsValid())
		{
			return;
		}

		self.AnalyzeButton.IsEnabled = false;

		var solver = ((App)Application.Current).RunningContext.Solver;
		var analysisResult = await Task.Run(() => { lock (self.AnalyzeSyncRoot) { return solver.Solve(puzzle); } });

		self.AnalyzeButton.IsEnabled = true;

		switch (analysisResult)
		{
			case { IsSolved: true }:
			{
				self.UpdateAnalysisResult(analysisResult);

				break;
			}
#if false
			case
			{
				WrongStep: _,
				FailedReason: _,
				UnhandledException: WrongStepException { CurrentInvalidGrid: _ }
			}:
			{
				break;
			}
			case { FailedReason: _, UnhandledException: _ }:
			{
				break;
			}
#endif
		}
	}
}
