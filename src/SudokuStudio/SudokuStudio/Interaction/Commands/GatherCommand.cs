namespace SudokuStudio.Interaction.Commands;

/// <summary>
/// Defines a gather command.
/// </summary>
public sealed class GatherCommand : ButtonCommand
{
	/// <inheritdoc/>
	public override async void Execute(object? parameter)
	{
		if (parameter is not AnalyzePage self)
		{
			return;
		}

		var grid = self.SudokuPane.Puzzle;
		if (!grid.IsValid())
		{
			return;
		}

		self.GatherTabPage.GatherButton.IsEnabled = false;
		self.IsGathererLaunched = true;
		self.GatherTabPage.TechniqueGroupView.ClearViewSource();

		var textFormat = GetString("AnalyzePage_AnalyzerProgress");

		var gatherer = ((App)Application.Current).EnvironmentVariables.Gatherer;
		var result = await Task.Run(gather);

		self.GatherTabPage._currentFountSteps = result;
		self.GatherTabPage.TechniqueGroupView.TechniqueGroups.Source = GridGathering.GetTechniqueGroups(result);
		self.GatherTabPage.GatherButton.IsEnabled = true;
		self.IsGathererLaunched = false;


		IEnumerable<IStep> gather()
		{
			lock (self.AnalyzeSyncRoot)
			{
				return gatherer.Search(grid, new Progress<double>(progressReportHandler));
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
		}
	}
}
