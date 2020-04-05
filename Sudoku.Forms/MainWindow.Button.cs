using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Sudoku.Drawing.Extensions;
using Sudoku.Solving;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Manual;
using static Sudoku.Forms.Constants.Processing;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void ButtonFindAllSteps_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				if (!_puzzle.IsValid(out _))
				{
					MessageBox.Show("The puzzle is invalid, so you cannot use this function.", "Warning");

					e.Handled = true;
					return;
				}

				_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
				_textBoxInfo.Text = "The solver is running slowly, please wait...";
				_buttonFindAllSteps.IsEnabled = false;
				DisableSolvingControls();

				var techniqueGroups = await new StepFinder(Settings).SearchAsync(_puzzle);

				EnableSolvingControls();
				SwitchOnGeneratingComboBoxesDisplaying();
				_buttonFindAllSteps.IsEnabled = true;
				_textBoxInfo.ClearValue(TextBox.TextProperty);

				// The boolean value stands for whether the technique is enabled.
				var list = new List<ListBoxItem>();
				foreach (var techniqueGroup in techniqueGroups)
				{
					string name = techniqueGroup.Key;
					foreach (var info in techniqueGroup)
					{
						var (fore, back) = Settings.DiffColors[info.DifficultyLevel];
						list.Add(
							new ListBoxItem
							{
								Content =
									new PrimaryElementTuple<string, TechniqueInfo, bool>(
										info.ToSimpleString(), info, true),
								BorderThickness = new Thickness(),
								Foreground = new SolidColorBrush(fore.ToWColor()),
								Background = new SolidColorBrush(back.ToWColor()),
							});
					}
				}

				_listBoxTechniques.ItemsSource = list;
			}

			#region Obsolete code
			//_treeView.ClearValue(ItemsControl.ItemsSourceProperty);
			//_textBoxInfo.Text = "The solver is running to find all possible steps, please wait...";
			//_buttonFindAllSteps.IsEnabled = false;
			//DisableSolvingControls();
			//
			//var techniqueGroups = await new StepFinder(Settings).SearchAsync(_puzzle);
			//
			//EnableSolvingControls();
			//_buttonFindAllSteps.IsEnabled = true;
			//_textBoxInfo.ClearValue(TextBox.TextProperty);
			//
			//var itemList = new List<TreeViewItem>();
			//foreach (var techniqueGroup in techniqueGroups)
			//{
			//	var techniqueRoot = new TreeNode
			//	{
			//		DisplayName = new PrimaryElementTuple<string, TechniqueInfo?, bool>(
			//			techniqueGroup.Key, null, false)
			//	};
			//	foreach (var info in techniqueGroup)
			//	{
			//		techniqueRoot.Children.Add(new TreeViewItem
			//		{
			//			Header = new TreeNode
			//			{
			//				DisplayName = new PrimaryElementTuple<string, TechniqueInfo?, bool>(
			//					info.Name, info, true)
			//			}
			//		});
			//	}
			//
			//	itemList.Add(new TreeViewItem
			//	{
			//		Header = techniqueRoot,
			//		IsExpanded = true
			//	});
			//}
			//
			//_treeView.ItemsSource = itemList;
			#endregion
		}

		private void ButtonFirst_Click(object sender, RoutedEventArgs e)
		{
			int current = Settings.CurrentPuzzleNumber = 0;
			int max = _puzzlesText!.Length;
			LoadPuzzle(_puzzlesText[current].TrimEnd(Splitter));
			UpdateDatabaseControls(false, false, true, true);

			_labelPuzzleNumber.Content = $"{current + 1}/{max}";
		}

		private void ButtonPrev_Click(object sender, RoutedEventArgs e)
		{
			int current = --Settings.CurrentPuzzleNumber;
			int max = _puzzlesText!.Length;
			LoadPuzzle(_puzzlesText![current].TrimEnd(Splitter));

			bool condition = Settings.CurrentPuzzleNumber != 0;
			UpdateDatabaseControls(condition, condition, true, true);

			_labelPuzzleNumber.Content = $"{current + 1}/{max}";
		}

		private void ButtonNext_Click(object sender, RoutedEventArgs e)
		{
			int current = ++Settings.CurrentPuzzleNumber;
			int max = _puzzlesText!.Length;
			LoadPuzzle(_puzzlesText![current].TrimEnd(Splitter));

			bool condition = Settings.CurrentPuzzleNumber != _puzzlesText.Length - 1;
			UpdateDatabaseControls(true, true, condition, condition);

			_labelPuzzleNumber.Content = $"{current + 1}/{max}";
		}

		private void ButtonLast_Click(object sender, RoutedEventArgs e)
		{
			int current = Settings.CurrentPuzzleNumber = _puzzlesText!.Length - 1;
			int max = _puzzlesText.Length;
			LoadPuzzle(_puzzlesText![current].TrimEnd(Splitter));
			UpdateDatabaseControls(true, true, false, false);

			_labelPuzzleNumber.Content = $"{current + 1}/{max}";
		}
	}
}
