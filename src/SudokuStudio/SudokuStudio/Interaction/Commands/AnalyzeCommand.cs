namespace SudokuStudio.Interaction.Commands;

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
		self.ClearAnalyzeTabsData();
		self.IsAnalyzerLaunched = true;

		var textFormat = GetString("AnalyzePage_AnalyzerProgress");

		var solver = ((App)Application.Current).ProgramSolver;
		var analysisResult = await Task.Run(analyze);

		self.AnalyzeButton.IsEnabled = true;
		self.IsAnalyzerLaunched = false;

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


		void progressReportHandler(double percent)
		{
			self.DispatcherQueue.TryEnqueue(updatePercentValueCallback);


			void updatePercentValueCallback()
			{
				self.ProgressPercent = percent * 100;
				self.AnalyzeProgressLabel.Text = string.Format(textFormat!, percent);
			}
		}

		LogicalSolverResult analyze()
		{
			lock (self.AnalyzeSyncRoot)
			{
				return solver.Solve(puzzle, new Progress<double>(progressReportHandler));
			}
		}
	}
}
