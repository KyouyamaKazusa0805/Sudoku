#pragma warning disable IDE1006

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Extensions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing.Extensions;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Manual;
using Sudoku.Windows.Constants;
using InfoTriplet = System.KeyedTuple<string, Sudoku.Solving.Manual.StepInfo, bool>;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private async void ButtonFindAllSteps_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
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
				var list = new List<ListBoxItem>();
				_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
				_textBoxInfo.Text = (string)LangSource["WhileFindingAllSteps"];
				DisableSolvingControls();

				(dialog = new()).Show();
				var techniqueGroups = await Task.Run(() =>
				{
					return new StepFinder(Settings.MainManualSolver)
					.Search(valueGrid, dialog.DefaultReporting, Settings.LanguageCode);
				});

				EnableSolvingControls();
				SwitchOnGeneratingComboBoxesDisplaying();
				_textBoxInfo.ClearValue(TextBox.TextProperty);

				// The boolean value stands for whether the technique is enabled.
				var collection = new ObservableCollection<ListBoxItem>();
				foreach (var techniqueGroup in techniqueGroups)
				{
					string name = techniqueGroup.Key;
					collection.AddRange(
						from info in techniqueGroup
						let pair = Settings.DiffColors[info.DifficultyLevel]
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
				srcView.GroupDescriptions.Add(
					new PropertyGroupDescription(
						new StringBuilder()
						.Append(nameof(Content))
						.Append('.')
						.Append(nameof(InfoTriplet.Item2))
						.Append('.')
						.Append(nameof(StepInfo.Name))
						.ToString()));
				_listBoxTechniques.ItemsSource = srcView;

				dialog?.CloseAnyway();
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonFirst_Click(object sender, RoutedEventArgs e)
		{
			int current = Settings.CurrentPuzzleNumber = 0;
			int max = _puzzlesText!.Length;
			LoadPuzzle(_puzzlesText[current].TrimEndNewLine());
			UpdateDatabaseControls(false, false, true, true);

			_labelPuzzleNumber.Content = $"{current + 1}/{max}";
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonPrev_Click(object sender, RoutedEventArgs e)
		{
			int current = --Settings.CurrentPuzzleNumber;
			int max = _puzzlesText!.Length;
			LoadPuzzle(_puzzlesText[current].TrimEndNewLine());

			bool condition = Settings.CurrentPuzzleNumber != 0;
			UpdateDatabaseControls(condition, condition, true, true);

			_labelPuzzleNumber.Content = $"{current + 1}/{max}";
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonNext_Click(object sender, RoutedEventArgs e)
		{
			int current = ++Settings.CurrentPuzzleNumber;
			int max = _puzzlesText!.Length;
			LoadPuzzle(_puzzlesText[current].TrimEndNewLine());

			bool condition = Settings.CurrentPuzzleNumber != _puzzlesText.Length - 1;
			UpdateDatabaseControls(true, true, condition, condition);

			_labelPuzzleNumber.Content = $"{current + 1}/{max}";
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonLast_Click(object sender, RoutedEventArgs e)
		{
			int current = Settings.CurrentPuzzleNumber = _puzzlesText!.Length - 1;
			int max = _puzzlesText.Length;
			LoadPuzzle(_puzzlesText[current].TrimEndNewLine());
			UpdateDatabaseControls(true, true, false, false);

			_labelPuzzleNumber.Content = $"{current + 1}/{max}";
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellReset_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = int.MinValue;
			_customDrawingMode = -1;

			_view.Clear();
			_currentPainter = _currentPainter with { CustomView = null };

			UpdateImageGrid();
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellColor1_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 0;
			_customDrawingMode = 0;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellColor2_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 1;
			_customDrawingMode = 0;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellColor3_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 2;
			_customDrawingMode = 0;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellColor4_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 3;
			_customDrawingMode = 0;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellColor5_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -1;
			_customDrawingMode = 0;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellColor6_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -2;
			_customDrawingMode = 0;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellColor7_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -3;
			_customDrawingMode = 0;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellColor8_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -4;
			_customDrawingMode = 0;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellColor9_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 4;
			_customDrawingMode = 0;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellColor10_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 5;
			_customDrawingMode = 0;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellColor11_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 6;
			_customDrawingMode = 0;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellColor12_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 7;
			_customDrawingMode = 0;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellColor13_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 8;
			_customDrawingMode = 0;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCellColor14_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 9;
			_customDrawingMode = 0;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor1_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 0;
			_customDrawingMode = 1;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor2_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 1;
			_customDrawingMode = 1;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor3_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 2;
			_customDrawingMode = 1;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor4_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 3;
			_customDrawingMode = 1;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor5_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -1;
			_customDrawingMode = 1;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor6_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -2;
			_customDrawingMode = 1;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor7_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -3;
			_customDrawingMode = 1;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor8_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -4;
			_customDrawingMode = 1;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor9_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 4;
			_customDrawingMode = 1;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor10_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 5;
			_customDrawingMode = 1;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor11_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 6;
			_customDrawingMode = 1;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor12_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 7;
			_customDrawingMode = 1;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor13_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 8;
			_customDrawingMode = 1;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor14_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 9;
			_customDrawingMode = 1;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonRegionColor1_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 0;
			_customDrawingMode = 2;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonRegionColor2_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 1;
			_customDrawingMode = 2;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonRegionColor3_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 2;
			_customDrawingMode = 2;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonRegionColor4_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 3;
			_customDrawingMode = 2;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonRegionColor5_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -1;
			_customDrawingMode = 2;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonRegionColor6_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -2;
			_customDrawingMode = 2;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonRegionColor7_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -3;
			_customDrawingMode = 2;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonRegionColor8_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = -4;
			_customDrawingMode = 2;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonRegionColor9_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 4;
			_customDrawingMode = 2;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonRegionColor10_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 5;
			_customDrawingMode = 2;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonRegionColor11_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 6;
			_customDrawingMode = 2;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonRegionColor12_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 7;
			_customDrawingMode = 2;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonRegionColor13_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 8;
			_customDrawingMode = 2;
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonRegionColor14_Click(object sender, RoutedEventArgs e)
		{
			_currentColor = 9;
			_customDrawingMode = 2;
		}
	}
}
