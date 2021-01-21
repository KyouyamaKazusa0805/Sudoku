using System;
using System.Extensions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Solving.Manual.Extensions;
using Sudoku.Windows.Extensions;

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
						if (_customDrawingMode != 3 && _currentColor == int.MinValue)
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
								case 1 when getCandidate() is var cand and >= 0 and < 729: // Candidate.
								{
									if (_view.ContainsCandidate(cand))
									{
										_view.RemoveCandidate(cand);
									}
									else
									{
										_view.AddCandidate(_currentColor, cand);
									}

									break;
								}
								case 2 when getCell() is var cell and >= 0 and < 81: // Region.
								{
									_selectedCellsWhileDrawingRegions.AddAnyway(cell);

									break;
								}
								case 3 when getCandidate() is var cand and >= 0 and < 729: // Chain.
								{
									// Normal mode: record the current candidate.
									_startCand = cand;

									break;
								}
							}

							_currentPainter.CustomView = _view;
							_currentPainter.Conclusions = null;
							_currentPainter.FocusedCells = null;

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
						int cell = _focusedCells.IsEmpty ? 0 : _focusedCells[0];
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

				_currentPainter.FocusedCells = _focusedCells;

				UpdateImageGrid();
			}
		}

		/// <inheritdoc cref="Events.MouseRightButtonDown(object?, EventArgs)"/>
		private void ImageGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			// Disable all menu items.
			const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
			for (int i = 0; i < 9; i++)
			{
				s(this, flags, i).Visibility = Visibility.Collapsed;
				d(this, flags, i).Visibility = Visibility.Collapsed;
			}

			// Check whether the cell is invalid.
			int cell = _pointConverter.GetCell((_currentRightClickPos = e.GetPosition(_imageGrid)).ToDPointF());
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
			int chosenCell = _pointConverter.GetCell(_currentRightClickPos.ToDPointF());
			foreach (int i in _puzzle.GetCandidates(chosenCell))
			{
				s(this, flags, i).Visibility = Visibility.Visible;
				d(this, flags, i).Visibility = Visibility.Visible;
			}

			static MenuItem s(MainWindow @this, BindingFlags flags, int i) =>
				(MenuItem)
				@this
				.GetType()
				.GetField($"_menuItemImageGridSet{(i + 1).ToString()}", flags)!
				.GetValue(@this)!;
			static MenuItem d(MainWindow @this, BindingFlags flags, int i) =>
				(MenuItem)
				@this
				.GetType()
				.GetField($"_menuItemImageGridDelete{(i + 1).ToString()}", flags)!
				.GetValue(@this)!;
		}

		/// <inheritdoc cref="Events.MouseLeftButtonUp(object?, EventArgs)"/>
		private void ImageGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (sender is not Image image || _customDrawingMode == -1)
			{
				e.Handled = true;
				return;
			}

			int getCell() => _pointConverter.GetCell(e.GetPosition(image).ToDPointF());
			int getCandidate() => _pointConverter.GetCandidate(e.GetPosition(image).ToDPointF());

			switch (Keyboard.Modifiers)
			{
				case ModifierKeys.None:
				{
					switch (_customDrawingMode)
					{
						case 2 when _customDrawingMode != -1: // Region.
						{
							int cell = getCell();
							_selectedCellsWhileDrawingRegions.AddAnyway(cell);

							if (_selectedCellsWhileDrawingRegions.Count != 2)
							{
								e.Handled = true;
								return;
							}

							switch (Keyboard.Modifiers)
							{
								case ModifierKeys.None when _currentColor == int.MinValue:
								{
									_focusedCells.Clear();
									_focusedCells.AddAnyway(cell);

									break;
								}
							}

							int first = _selectedCellsWhileDrawingRegions[0];
							int second = _selectedCellsWhileDrawingRegions[1];
							int r1 = first.ToRegion(RegionLabel.Row), r2 = second.ToRegion(RegionLabel.Row);
							int c1 = first.ToRegion(RegionLabel.Column), c2 = second.ToRegion(RegionLabel.Column);
							int b1 = first.ToRegion(RegionLabel.Block), b2 = second.ToRegion(RegionLabel.Block);
							int region = r1 == r2 ? r1 : c1 == c2 ? c1 : b1 == b2 ? b1 : -1;
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

							_selectedCellsWhileDrawingRegions.Clear();
							_currentPainter.FocusedCells = _focusedCells;

							break;
						}
						case 3 when getCandidate() is var cand and >= 0 and < 729 && _startCand != -1: // Chain.
						{
							_view.AddLink(new(_startCand, cand, LinkType.Strong));

							_startCand = -1;

							break;
						}
					}

					_currentPainter.FocusedCells = null;
					_currentPainter.CustomView = _view;
					_currentPainter.Conclusions = null;

					UpdateImageGrid();

					break;
				}
			}
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
			Action<object, RoutedEventArgs> a = _comboBoxMode.SelectedIndex switch
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
