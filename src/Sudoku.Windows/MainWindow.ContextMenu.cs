using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Sudoku.DocComments;
using Sudoku.Solving.BruteForces;
using Sudoku.Windows.Extensions;
using InfoTriplet = System.Collections.Generic.KeyedTuple<string, Sudoku.Solving.Manual.StepInfo, bool>;
using StepTriplet = System.Collections.Generic.KeyedTuple<string, int, Sudoku.Solving.Manual.StepInfo>;

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
					if (_listBoxPaths.SelectedItem is ListBoxItem { Content: StepTriplet triplet })
					{
						SystemClipboard.Text = triplet.Item3.ToFullString();
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
				var sb = new ValueStringBuilder(stackalloc char[50]);
				foreach (string step in
					from ListBoxItem item in _listBoxPaths.Items
					let content = item.Content as StepTriplet
					where content is not null
					select content.Item3.ToFullString())
				{
					sb.AppendLine(step);
				}

				try
				{
					SystemClipboard.Text = sb.ToString();
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
			if (
				sender is MenuItem
				&& _listBoxTechniques is
				{
					SelectedItem: ListBoxItem { Content: InfoTriplet { Item3: true } triplet }
				})
			{
				ref var valueGrid = ref _puzzle.InnerGrid;
				var info = triplet.Item2;
				if (!Settings.MainManualSolver.CheckConclusionValidityAfterSearched
					|| CheckConclusionsValidity(
						new UnsafeBitwiseSolver().Solve(valueGrid).Solution!.Value, info.Conclusions))
				{
					info.ApplyTo(ref valueGrid);
					_currentPainter.Conclusions = null;
					_currentPainter.View = null;

					_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
					_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
					_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);

					UpdateImageGrid();
				}
				else
				{
					Messagings.WrongHandling(info, valueGrid);
				}
			}
		}
	}
}
