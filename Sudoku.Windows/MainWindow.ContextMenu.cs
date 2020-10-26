using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Sudoku.DocComments;
using Sudoku.Drawing.Extensions;
using Sudoku.Solving;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Windows.Constants;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridSet1_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 0);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridSet2_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 1);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridSet3_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 2);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridSet4_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 3);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridSet5_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 4);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridSet6_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 5);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridSet7_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 6);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridSet8_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 7);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridSet9_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 8);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridDelete1_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 0);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridDelete2_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 1);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridDelete3_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 2);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridDelete4_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 3);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridDelete5_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 4);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridDelete6_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 5);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridDelete7_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 6);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridDelete8_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 7);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void MenuItemImageGridDelete9_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCell(_currentRightClickPos.ToDPointF()), 8);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ContextListBoxPathsCopyCurrentStep_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem)
			{
				try
				{
					if (_listBoxPaths.SelectedItem
						is ListBoxItem { Content: KeyedTuple<string, int, TechniqueInfo> triplet })
					{
						Clipboard.SetText(triplet.Item3.ToFullString());
					}
				}
				catch
				{
					Messagings.CannotCopyStep();
				}
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ContextListBoxPathsCopyAllSteps_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem)
			{
				var sb = new StringBuilder();
				foreach (string step in
					from ListBoxItem item in _listBoxPaths.Items
					let content = item.Content as KeyedTuple<string, int, TechniqueInfo>
					where content is not null
					select content.Item3.ToFullString())
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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ContextMenuTechniquesApply_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem
				&& _listBoxTechniques is { SelectedItem: ListBoxItem { Content: KeyedTuple<string, TechniqueInfo, bool> { Item3: true } triplet } })
			{
				var valueGrid = _puzzle._innerGrid;
				var info = triplet.Item2;
				if (!Settings.MainManualSolver.CheckConclusionValidityAfterSearched
					|| CheckConclusionsValidity(
						new UnsafeBitwiseSolver().Solve(valueGrid).Solution!.Value, info.Conclusions))
				{
					info.ApplyTo(ref valueGrid);
					_currentPainter = _currentPainter with { Conclusions = null, View = null, Grid = _puzzle };

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
