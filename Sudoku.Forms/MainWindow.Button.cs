using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Sudoku.Forms.Drawing.Extensions;
using Sudoku.Solving;
using Sudoku.Solving.Manual;
using w = System.Windows;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void ButtonFindAllSteps_Click(object sender, RoutedEventArgs e)
		{
			_listBoxTechniques.ClearValue(ItemsSourceProperty);
			_textBoxInfo.Text = "The solver is running slowly, please wait...";
			_buttonFindAllSteps.IsEnabled = false;
			DisableSolvingControls();

			var techniqueGroups = await new StepFinder(Settings).SearchAsync(_puzzle);

			EnableSolvingControls();
			_buttonFindAllSteps.IsEnabled = true;
			_textBoxInfo.ClearValue(w::Controls.TextBox.TextProperty);

			// The boolean value stands for whether the technique is enabled.
			var list = new List<w::Controls.ListBoxItem>();
			foreach (var techniqueGroup in techniqueGroups)
			{
				string name = techniqueGroup.Key;
				foreach (var info in techniqueGroup)
				{
					var (fore, back) = Settings.DiffColors[info.DifficultyLevel];
					list.Add(
						new w::Controls.ListBoxItem
						{
							Content = new PrimaryElementTuple<string, TechniqueInfo, bool>(
								info.ToSimpleString(),
								info,
								true),
							Foreground = new w::Media.SolidColorBrush(fore.ToWColor()),
							Background = new w::Media.SolidColorBrush(back.ToWColor())
						});
				}
			}

			_listBoxTechniques.ItemsSource = list;

			#region Obsolete code
			//_treeView.ClearValue(w::Controls.ItemsControl.ItemsSourceProperty);
			//_textBoxInfo.Text = "The solver is running to find all possible steps, please wait...";
			//_buttonFindAllSteps.IsEnabled = false;
			//DisableSolvingControls();
			//
			//var techniqueGroups = await new StepFinder(Settings).SearchAsync(_puzzle);
			//
			//EnableSolvingControls();
			//_buttonFindAllSteps.IsEnabled = true;
			//_textBoxInfo.ClearValue(w::Controls.TextBox.TextProperty);
			//
			//var itemList = new List<w::Controls.TreeViewItem>();
			//foreach (var techniqueGroup in techniqueGroups)
			//{
			//	var techniqueRoot = new TreeNode
			//	{
			//		DisplayName = new PrimaryElementTuple<string, TechniqueInfo?, bool>(
			//			techniqueGroup.Key, null, false)
			//	};
			//	foreach (var info in techniqueGroup)
			//	{
			//		techniqueRoot.Children.Add(new w::Controls.TreeViewItem
			//		{
			//			Header = new TreeNode
			//			{
			//				DisplayName = new PrimaryElementTuple<string, TechniqueInfo?, bool>(
			//					info.Name, info, true)
			//			}
			//		});
			//	}
			//
			//	itemList.Add(new w::Controls.TreeViewItem
			//	{
			//		Header = techniqueRoot,
			//		IsExpanded = true
			//	});
			//}
			//
			//_treeView.ItemsSource = itemList;
			#endregion
		}
	}
}
