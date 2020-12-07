using System;
using System.Extensions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing.Extensions;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		/// <inheritdoc cref="Events.ContextMenuOpening(object?, EventArgs)"/>
		private void ImageGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			if (_imageGridContextMenu.IsOpen = _customDrawingMode == -1)
			{
				_selectedCellsWhileDrawingRegions.Clear();
			}
		}

		/// <inheritdoc cref="Events.MouseLeftButtonDown(object?, EventArgs)"/>
		private void ImageGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is Image image)
			{
				int getCell() => _pointConverter.GetCell(e.GetPosition(image).ToDPointF());
				int getCandidate() => _pointConverter.GetCandidate(e.GetPosition(image).ToDPointF());

				switch (Keyboard.Modifiers)
				{
					case ModifierKeys.None:
					{
						if (_currentColor == int.MinValue)
						{
							_focusedCells.Clear();
							_focusedCells.AddAnyway(getCell());
						}
						else
						{
							switch (_customDrawingMode)
							{
								case 0 when getCell() is var cell and not (< 0 or >= 81): // Cell.
								{
									if (_view.ContainsCell(cell))
									{
										_view.RemoveCell(cell);
									}
									else
									{
										_view.AddCell(_currentColor, cell);
									}

									break;
								}
								case 1 when getCandidate() is var candidate and not (< 0 or >= 729): // Candidate.
								{
									if (_view.ContainsCandidate(candidate))
									{
										_view.RemoveCandidate(candidate);
									}
									else
									{
										_view.AddCandidate(_currentColor, candidate);
									}

									break;
								}
							}

							_currentPainter = _currentPainter with
							{
								CustomView = _view,
								Conclusions = null,
								FocusedCells = null
							};

							UpdateImageGrid();
						}

						break;
					}
					//case ModifierKeys.Alt:
					//{
					//	break;
					//}
					case ModifierKeys.Control:
					{
						// Multi-select.
						_focusedCells.AddAnyway(getCell());

						break;
					}
					case ModifierKeys.Shift:
					{
						// Select a region of cells.
						int cell = _focusedCells.IsEmpty ? 0 : _focusedCells.First;
						int currentClickedCell = getCell();
						int r1 = cell / 9, c1 = cell % 9;
						int r2 = currentClickedCell / 9, c2 = currentClickedCell % 9;
						int minRow = Math.Min(r1, r2), minColumn = Math.Min(c1, c2);
						int maxRow = Math.Max(r1, r2), maxColumn = Math.Max(c1, c2);
						for (int r = minRow; r <= maxRow; r++)
						{
							for (int c = minColumn; c <= maxColumn; c++)
							{
								_focusedCells.AddAnyway(r * 9 + c);
							}
						}

						break;
					}
					//case ModifierKeys.Windows:
					//{
					//	break;
					//}
				}

				_currentPainter = _currentPainter with { FocusedCells = _focusedCells };

				UpdateImageGrid();
			}
		}

		/// <inheritdoc cref="Events.MouseRightButtonDown(object?, EventArgs)"/>
		private void ImageGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			int cell = _pointConverter.GetCell((_currentRightClickPos = e.GetPosition(_imageGrid)).ToDPointF());
			_selectedCellsWhileDrawingRegions.AddAnyway(cell);

			// Disable all menu items.
			var flags = BindingFlags.NonPublic | BindingFlags.Instance;
			for (int i = 0; i < 9; i++)
			{
				s(this, flags, i).Visibility = Visibility.Collapsed;
				d(this, flags, i).Visibility = Visibility.Collapsed;
			}

			// Check whether the cell is invalid.
			if (cell == -1)
			{
				e.Handled = true;
				return;
			}

			// Check whether the specified cell is not empty.
			if (_puzzle.GetStatus(cell) != CellStatus.Empty)
			{
				e.Handled = true;
				return;
			}

			// Then enable some of them.
			foreach (int i in _puzzle.GetCandidateMask(_pointConverter.GetCell(_currentRightClickPos.ToDPointF())))
			{
				s(this, flags, i).Visibility = Visibility.Visible;
				d(this, flags, i).Visibility = Visibility.Visible;
			}

			static MenuItem s(MainWindow @this, BindingFlags flags, int i) =>
				(MenuItem)@this.GetType().GetField($"_menuItemImageGridSet{i + 1}", flags)!.GetValue(@this)!;
			static MenuItem d(MainWindow @this, BindingFlags flags, int i) =>
				(MenuItem)@this.GetType().GetField($"_menuItemImageGridDelete{i + 1}", flags)!.GetValue(@this)!;
		}

		/// <inheritdoc cref="Events.MouseRightButtonUp(object?, EventArgs)"/>
		private void ImageGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			if ((sender, _customDrawingMode) is not (Image image, not -1))
			{
				e.Handled = true;
				return;
			}

			int cell = _pointConverter.GetCell(e.GetPosition(image).ToDPointF());
			_selectedCellsWhileDrawingRegions.AddAnyway(cell);

			switch (Keyboard.Modifiers)
			{
				case ModifierKeys.None:
				{
					if (_currentColor == int.MinValue)
					{
						_focusedCells.Clear();
						_focusedCells.AddAnyway(cell);
					}
					else
					{
						switch (_customDrawingMode)
						{
							case 2 when _selectedCellsWhileDrawingRegions.Count == 2: // Region.
							{
								int first = _selectedCellsWhileDrawingRegions.First;
								int second = _selectedCellsWhileDrawingRegions.SetAt(1);
								int r1 = RegionLabel.Row.GetRegion(first);
								int r2 = RegionLabel.Row.GetRegion(second);
								int c1 = RegionLabel.Column.GetRegion(first);
								int c2 = RegionLabel.Column.GetRegion(second);
								int b1 = RegionLabel.Block.GetRegion(first);
								int b2 = RegionLabel.Block.GetRegion(second);
								int region = (r1 == r2, c1 == c2, b1 == b2) switch
								{
									(true, _, _) => r1,
									(_, true, _) => c1,
									(_, _, true) => b1,
									_ => -1
								};
								if (region != -1)
								{
									if (_view.ContainsRegion(region))
									{
										_view.RemoveRegion(region);
									}
									else
									{
										_view.AddRegion(_currentColor, region);
									}
								}

								break;
							}
						}

						_currentPainter = _currentPainter with
						{
							FocusedCells = null,
							CustomView = _view,
							Conclusions = null
						};

						UpdateImageGrid();
					}

					break;
				}
			}

			_selectedCellsWhileDrawingRegions.Clear();
			_currentPainter = _currentPainter with { FocusedCells = _focusedCells };

			UpdateImageGrid();
		}

		/// <inheritdoc cref="Events.MouseLeftButtonDown(object?, EventArgs)"/>
		private void ImageUndoIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
			MenuItemEditUndo_Click(sender, e);

		/// <inheritdoc cref="Events.MouseLeftButtonDown(object?, EventArgs)"/>
		private void ImageRedoIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
			MenuItemEditRedo_Click(sender, e);

		/// <inheritdoc cref="Events.MouseLeftButtonDown(object?, EventArgs)"/>
		private void ImageGeneratingIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Action<object, RoutedEventArgs> a =
				_comboBoxMode.SelectedIndex switch
				{
					0 => MenuItemGenerateWithSymmetry_Click,
					1 => MenuItemGenerateHardPattern_Click
				};

			a(sender, e);
		}

		/// <inheritdoc cref="Events.MouseLeftButtonDown(object?, EventArgs)"/>
		private void ImageSolve_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
			MenuItemAnalyzeAnalyze_Click(sender, e);
	}
}
