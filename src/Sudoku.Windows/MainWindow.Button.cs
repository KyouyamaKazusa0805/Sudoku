#pragma warning disable IDE1006

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Sudoku.Data;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Manual;
using Sudoku.Windows.Extensions;
using Sudoku.Windows.Media;
using InfoTriplet = System.Collections.Generic.KeyedTuple<string, Sudoku.Solving.Manual.StepInfo, bool>;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		private async void ButtonFindAllSteps_Click(object sender, RoutedEventArgs e)
		{
			CancellationTokenSource? cts = null;
			try
			{
				cts = new();
				await internalOperation(cts);
			}
			catch (OperationCanceledException)
			{
				EnableSolvingControls();
				SwitchOnGeneratingComboBoxesDisplaying();
			}
			finally
			{
				cts?.Dispose();
			}

			async Task internalOperation(CancellationTokenSource cts)
			{
				var valueGrid = (SudokuGrid)_puzzle;
				if (!valueGrid.IsValid())
				{
					Messagings.FunctionIsUnavailable();

					e.Handled = true;
					return;
				}

				// Searching.
				ProgressWindow? dialog = null;
				_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
				_textBoxInfo.Text = (string)LangSource["WhileFindingAllSteps"];
				DisableSolvingControls();

				(dialog = new() { CancellationTokenSource = cts }).Show();
				var techniqueGroups =
					await new AllStepSearcher(Settings.MainManualSolver).SearchAsync(
						valueGrid, dialog.DefaultReporting, Settings.LanguageCode, cts.Token
					) ?? throw new OperationCanceledException();

				EnableSolvingControls();
				SwitchOnGeneratingComboBoxesDisplaying();
				_textBoxInfo.ClearValue(TextBox.TextProperty);

				// The boolean value stands for whether the technique is enabled.
				var collection = new ObservableCollection<ListBoxItem>();
				foreach (var techniqueGroup in techniqueGroups)
				{
					collection.AddRange(
						from info in techniqueGroup
						let pair = ColorPalette.DifficultyLevelColors[info.DifficultyLevel]
						select new ListBoxItem
						{
							Content = new InfoTriplet(info.ToSimpleString(), info, true),
							Foreground = new SolidColorBrush(pair.Foreground.ToWColor()),
							Background = new SolidColorBrush(pair.Background.ToWColor()),
							BorderThickness = default,
							HorizontalContentAlignment = HorizontalAlignment.Left,
							VerticalContentAlignment = VerticalAlignment.Center
						});
				}

				// Group them by its name.
				var srcView = new ListCollectionView(collection);
				var groupDescriptions = srcView.GroupDescriptions;
				if (groupDescriptions is null)
				{
					goto CloseDialog;
				}

				groupDescriptions.Add(new PropertyGroupDescription(z()));
				_listBoxTechniques.ItemsSource = srcView;

			CloseDialog:
				dialog.Close();
			}

			static string z()
			{
				var sb = new ValueStringBuilder(stackalloc char[17]);
				sb.Append(nameof(Content));
				sb.Append('.');
				sb.Append(nameof(InfoTriplet.Item2));
				sb.Append('.');
				sb.Append(nameof(StepInfo.Name));

				return sb.ToString();
			}
		}

		private void ButtonFirst_Click(object sender, RoutedEventArgs e)
		{
			int current = Settings.CurrentPuzzleNumber = 0;
			int max = _puzzlesText!.Length;
			LoadPuzzle(_puzzlesText[current].TrimEndNewLine());
			UpdateDatabaseControls(false, false, true, true);

			_labelPuzzleNumber.Content = $"1/{max.ToString()}";
		}

		private void ButtonPrev_Click(object sender, RoutedEventArgs e)
		{
			int current = --Settings.CurrentPuzzleNumber;
			int max = _puzzlesText!.Length;
			LoadPuzzle(_puzzlesText[current].TrimEndNewLine());

			bool condition = Settings.CurrentPuzzleNumber != 0;
			UpdateDatabaseControls(condition, condition, true, true);

			_labelPuzzleNumber.Content = $"{(current + 1).ToString()}/{max.ToString()}";
		}

		private void ButtonNext_Click(object sender, RoutedEventArgs e)
		{
			int current = ++Settings.CurrentPuzzleNumber;
			int max = _puzzlesText!.Length;
			LoadPuzzle(_puzzlesText[current].TrimEndNewLine());

			bool condition = Settings.CurrentPuzzleNumber != _puzzlesText.Length - 1;
			UpdateDatabaseControls(true, true, condition, condition);

			_labelPuzzleNumber.Content = $"{(current + 1).ToString()}/{max.ToString()}";
		}

		private void ButtonLast_Click(object sender, RoutedEventArgs e)
		{
			int current = Settings.CurrentPuzzleNumber = _puzzlesText!.Length - 1;
			int max = _puzzlesText.Length;
			LoadPuzzle(_puzzlesText[current].TrimEndNewLine());
			UpdateDatabaseControls(true, true, false, false);

			_labelPuzzleNumber.Content = $"{(current + 1).ToString()}/{max.ToString()}";
		}

		private void ButtonCellReset_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = int.MinValue;
			_customDrawingMode = -1;

			_view.Clear();
			_currentPainter.CustomView = null;

			UpdateImageGrid();
		}

		private void ButtonCellColor1_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 0;
			_customDrawingMode = 0;
		}

		private void ButtonCellColor2_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 1;
			_customDrawingMode = 0;
		}

		private void ButtonCellColor3_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 2;
			_customDrawingMode = 0;
		}

		private void ButtonCellColor4_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 3;
			_customDrawingMode = 0;
		}

		private void ButtonCellColor5_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -1;
			_customDrawingMode = 0;
		}

		private void ButtonCellColor6_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -2;
			_customDrawingMode = 0;
		}

		private void ButtonCellColor7_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -3;
			_customDrawingMode = 0;
		}

		private void ButtonCellColor8_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -4;
			_customDrawingMode = 0;
		}

		private void ButtonCellColor9_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 4;
			_customDrawingMode = 0;
		}

		private void ButtonCellColor10_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 5;
			_customDrawingMode = 0;
		}

		private void ButtonCellColor11_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 6;
			_customDrawingMode = 0;
		}

		private void ButtonCellColor12_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 7;
			_customDrawingMode = 0;
		}

		private void ButtonCellColor13_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 8;
			_customDrawingMode = 0;
		}

		private void ButtonCellColor14_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 9;
			_customDrawingMode = 0;
		}

		private void ButtonCandidateColor1_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 0;
			_customDrawingMode = 1;
		}

		private void ButtonCandidateColor2_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 1;
			_customDrawingMode = 1;
		}

		private void ButtonCandidateColor3_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 2;
			_customDrawingMode = 1;
		}

		private void ButtonCandidateColor4_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 3;
			_customDrawingMode = 1;
		}

		private void ButtonCandidateColor5_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -1;
			_customDrawingMode = 1;
		}

		private void ButtonCandidateColor6_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -2;
			_customDrawingMode = 1;
		}

		private void ButtonCandidateColor7_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -3;
			_customDrawingMode = 1;
		}

		private void ButtonCandidateColor8_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -4;
			_customDrawingMode = 1;
		}

		private void ButtonCandidateColor9_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 4;
			_customDrawingMode = 1;
		}

		private void ButtonCandidateColor10_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 5;
			_customDrawingMode = 1;
		}

		private void ButtonCandidateColor11_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 6;
			_customDrawingMode = 1;
		}

		private void ButtonCandidateColor12_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 7;
			_customDrawingMode = 1;
		}

		private void ButtonCandidateColor13_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 8;
			_customDrawingMode = 1;
		}

		private void ButtonCandidateColor14_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 9;
			_customDrawingMode = 1;
		}

		private void ButtonRegionColor1_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 0;
			_customDrawingMode = 2;
		}

		private void ButtonRegionColor2_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 1;
			_customDrawingMode = 2;
		}

		private void ButtonRegionColor3_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 2;
			_customDrawingMode = 2;
		}

		private void ButtonRegionColor4_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 3;
			_customDrawingMode = 2;
		}

		private void ButtonRegionColor5_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -1;
			_customDrawingMode = 2;
		}

		private void ButtonRegionColor6_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -2;
			_customDrawingMode = 2;
		}

		private void ButtonRegionColor7_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -3;
			_customDrawingMode = 2;
		}

		private void ButtonRegionColor8_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -4;
			_customDrawingMode = 2;
		}

		private void ButtonRegionColor9_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 4;
			_customDrawingMode = 2;
		}

		private void ButtonRegionColor10_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 5;
			_customDrawingMode = 2;
		}

		private void ButtonRegionColor11_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 6;
			_customDrawingMode = 2;
		}

		private void ButtonRegionColor12_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 7;
			_customDrawingMode = 2;
		}

		private void ButtonRegionColor13_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 8;
			_customDrawingMode = 2;
		}

		private void ButtonRegionColor14_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 9;
			_customDrawingMode = 2;
		}

		private void ButtonStartDrawingChain_Click(object sender, RoutedEventArgs e)
		{
			_customDrawingMode = 3;
			_startCand = -1;
		}

		private void ButtonEndDrawingChain_Click(object sender, RoutedEventArgs e)
		{
			_customDrawingMode = -1;
			_startCand = -1;
		}
	}
}
