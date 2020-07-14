using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Sudoku.Solving;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Windows.Constants;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		private void TreeViewTechniques_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (sender is TreeView treeView && treeView.SelectedItem is TreeNode<TreeViewItem> node
				&& node.Content?.Header is PrimaryElementTuple<string, TechniqueInfo, bool> triplet)
			{
				var (_, info, isEnabled) = triplet;
				if (isEnabled)
				{
					_currentTechniqueInfo = info;
					_currentPainter.View = info.Views[_currentViewIndex = 0];
					_currentPainter.Conclusions = info.Conclusions;
					_textBoxInfo.Text = info.ToFullString();

					UpdateImageGrid();
				}
			}
			else
			{
				_contextMenuTechniques = null;
			}
		}

		private void ContextMenuTechniquesApply_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem
				&& _treeViewTechniques.SelectedItem is TreeNode<TreeViewItem> node
				&& node.Content?.Header is PrimaryElementTuple<string, TechniqueInfo, bool> triplet)
			{
				var (_, info, isEnabled) = triplet;
				if (isEnabled)
				{
					if (!Settings.MainManualSolver.CheckConclusionValidityAfterSearched
						|| CheckConclusionsValidity(new BitwiseSolver().Solve(_puzzle).Solution!, info.Conclusions))
					{
						info.ApplyTo(_puzzle);

						_currentPainter.View = null;
						_currentPainter.Grid = _puzzle;

						_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
						_treeViewTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
						_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);

						UpdateImageGrid();
					}
					else
					{
						Messagings.WrongHandling();
					}
				}
			}
		}
	}
}
