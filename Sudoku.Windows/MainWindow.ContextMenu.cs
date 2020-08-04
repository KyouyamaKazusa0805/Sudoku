using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Sudoku.Drawing.Extensions;
using Sudoku.Solving;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Windows.Constants;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		private void MenuItemImageGridSet1_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 0);

		private void MenuItemImageGridSet2_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 1);

		private void MenuItemImageGridSet3_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 2);

		private void MenuItemImageGridSet4_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 3);

		private void MenuItemImageGridSet5_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 4);

		private void MenuItemImageGridSet6_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 5);

		private void MenuItemImageGridSet7_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 6);

		private void MenuItemImageGridSet8_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 7);

		private void MenuItemImageGridSet9_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 8);

		private void MenuItemImageGridDelete1_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 0);

		private void MenuItemImageGridDelete2_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 1);

		private void MenuItemImageGridDelete3_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 2);

		private void MenuItemImageGridDelete4_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 3);

		private void MenuItemImageGridDelete5_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 4);

		private void MenuItemImageGridDelete6_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 5);

		private void MenuItemImageGridDelete7_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 6);

		private void MenuItemImageGridDelete8_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 7);

		private void MenuItemImageGridDelete9_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 8);

		private void ContextListBoxPathsCopyCurrentStep_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem)
			{
				try
				{
					if (_listBoxPaths.SelectedItem is ListBoxItem
					{
						Content: PrimaryElementTuple<string, int, TechniqueInfo> triplet
					})
					{
						Clipboard.SetText(triplet.Value3.ToFullString());
					}
				}
				catch
				{
					Messagings.CannotCopyStep();
				}
			}
		}

		[SuppressMessage("Style", "IDE0038:Use pattern matching", Justification = "<Pending>")]
		private void ContextListBoxPathsCopyAllSteps_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem)
			{
				var sb = new StringBuilder();
				foreach (string step in
					from ListBoxItem item in _listBoxPaths.Items
					let Content = item.Content
					where Content is PrimaryElementTuple<string, int, TechniqueInfo>
					select ((PrimaryElementTuple<string, int, TechniqueInfo>)Content).Value3.ToFullString())
				{
					sb.AppendLine(step);
				}

				try
				{
					Clipboard.SetText(sb.ToString());
				}
				catch
				{
					Messagings.CannotCopyStep();
				}
			}
		}

		private void ContextMenuTechniquesApply_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem && _listBoxTechniques.SelectedItem is ListBoxItem
			{
				Content: PrimaryElementTuple<string, TechniqueInfo, bool> { Value3: true } triplet
			})
			{
				var info = triplet.Value2;
				if (!Settings.MainManualSolver.CheckConclusionValidityAfterSearched
					|| CheckConclusionsValidity(new BitwiseSolver().Solve(_puzzle).Solution!, info.Conclusions))
				{
					info.ApplyTo(_puzzle);

					_currentPainter.Conclusions = null;
					_currentPainter.View = null;
					_currentPainter.Grid = _puzzle;

					_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
					_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
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
