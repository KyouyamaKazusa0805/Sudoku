using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Sudoku.Forms.Tooling;
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
			_treeView.ClearValue(w::Controls.ItemsControl.ItemsSourceProperty);
			_textBoxInfo.Text = "The solver is running to find all possible steps, please wait...";
			_buttonFindAllSteps.IsEnabled = false;
			DisableSolvingControls();

			var techniqueGroups = await new StepFinder(Settings).SearchAsync(_puzzle);

			EnableSolvingControls();
			_buttonFindAllSteps.IsEnabled = true;
			_textBoxInfo.ClearValue(w::Controls.TextBox.TextProperty);

			var itemList = new List<w::Controls.TreeViewItem>();
			foreach (var techniqueGroup in techniqueGroups)
			{
				var techniqueRoot = new TreeNode
				{
					Header = new PrimaryElementTuple<string, TechniqueInfo?, bool>(
						techniqueGroup.Key, null, false)
				};
				foreach (var info in techniqueGroup)
				{
					techniqueRoot.Children.Add(new w::Controls.TreeViewItem
					{
						Header = new TreeNode
						{
							Header = new PrimaryElementTuple<string, TechniqueInfo?, bool>(
							info.Name, info, true)
						}
					});
				}

				itemList.Add(new w::Controls.TreeViewItem
				{
					Header = techniqueRoot
				});
			}

			_treeView.ItemsSource = itemList;
		}
	}
}
