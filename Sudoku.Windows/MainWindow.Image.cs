using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sudoku.Constants;
using Sudoku.Drawing;
using Sudoku.Drawing.Extensions;
using Sudoku.Extensions;
using Sudoku.Windows.Drawing.Layers;
using static System.Math;
using static System.Reflection.BindingFlags;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		private void ImageGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			if (_imageGridContextMenu.IsOpen = _customDrawingMode == -1)
			{
				_selectedCellsWhileDrawingRegions.Clear();
			}
		}

		private void ImageGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is Image image)
			{
				int getCell() => _pointConverter.GetCellOffset(e.GetPosition(image).ToDPointF());
				int getCandidate() => _pointConverter.GetCandidateOffset(e.GetPosition(image).ToDPointF());

				switch (Keyboard.Modifiers)
				{
					case ModifierKeys.None:
					{
						if (_currentColor == int.MinValue)
						{
							_focusedCells.Clear();
							_focusedCells.Add(getCell());
						}
						else
						{
							switch (_customDrawingMode)
							{
								case 0: // Cell.
								{
									int cell = getCell();
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
								case 1: // Candidate.
								{
									int candidate = getCandidate();
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

							_layerCollection.Remove<FocusLayer>();
							_layerCollection.Add(
								new CustomViewLayer(
									_pointConverter, _view, null, Settings.PaletteColors,
									Settings.EliminationColor, Settings.CannibalismColor, Settings.ChainColor));

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
						_focusedCells.Add(getCell());

						break;
					}
					case ModifierKeys.Shift:
					{
						// Select a region of cells.
						int cell = _focusedCells.IsEmpty ? 0 : _focusedCells.SetAt(0);
						int currentClickedCell = getCell();
						int r1 = cell / 9, c1 = cell % 9;
						int r2 = currentClickedCell / 9, c2 = currentClickedCell % 9;
						int minRow = Min(r1, r2), minColumn = Min(c1, c2);
						int maxRow = Max(r1, r2), maxColumn = Max(c1, c2);
						for (int r = minRow; r <= maxRow; r++)
						{
							for (int c = minColumn; c <= maxColumn; c++)
							{
								_focusedCells.Add(r * 9 + c);
							}
						}

						break;
					}
					//case ModifierKeys.Windows:
					//{
					//	break;
					//}
				}

				_layerCollection.Add(new FocusLayer(_pointConverter, _focusedCells, Settings.FocusedCellColor));

				UpdateImageGrid();
			}
		}

		private void ImageGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			_selectedCellsWhileDrawingRegions.Add(
				_pointConverter.GetCellOffset((_currentRightClickPos = e.GetPosition(_imageGrid)).ToDPointF()));

			if (_imageGridContextMenu.IsOpen)
			{
				// Disable all menu items.
				for (int i = 0; i < 9; i++)
				{
					((MenuItem)
						GetType().GetField($"_menuItemImageGridSet{i + 1}", NonPublic | Instance)!.GetValue(this)!
					).IsEnabled = false;
					((MenuItem)
						GetType().GetField($"_menuItemImageGridDelete{i + 1}", NonPublic | Instance)!.GetValue(this)!
					).IsEnabled = false;
				}

				// Then enable some of them.
				foreach (int i in
					_puzzle.GetCandidateMask(
						_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF())).GetAllSets())
				{
					((MenuItem)
						GetType().GetField($"_menuItemImageGridSet{i + 1}", NonPublic | Instance)!.GetValue(this)!
					).IsEnabled = true;
					((MenuItem)
						GetType().GetField($"_menuItemImageGridDelete{i + 1}", NonPublic | Instance)!.GetValue(this)!
					).IsEnabled = true;
				}
			}
		}

		private void ImageGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (sender is Image image && _customDrawingMode != -1)
			{
				int getCell() => _pointConverter.GetCellOffset(e.GetPosition(image).ToDPointF());
				_selectedCellsWhileDrawingRegions.Add(getCell());

				switch (Keyboard.Modifiers)
				{
					case ModifierKeys.None:
					{
						if (_currentColor == int.MinValue)
						{
							_focusedCells.Clear();
							_focusedCells.Add(getCell());
						}
						else
						{
							switch (_customDrawingMode)
							{
								case 2 when _selectedCellsWhileDrawingRegions.Count == 2: // Region.
								{
									int first = _selectedCellsWhileDrawingRegions.SetAt(0);
									int second = _selectedCellsWhileDrawingRegions.SetAt(1);
									int r1 = GetRegion(first, Row), r2 = GetRegion(second, Row);
									int c1 = GetRegion(first, Column), c2 = GetRegion(second, Column);
									int b1 = GetRegion(first, Block), b2 = GetRegion(second, Block);
									int region = true switch
									{
										_ when r1 == r2 => r1,
										_ when c1 == c2 => c1,
										_ when b1 == b2 => b1,
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

									_selectedCellsWhileDrawingRegions.Clear();
									break;
								}
							}

							_layerCollection.Remove<FocusLayer>();
							_layerCollection.Add(
								new CustomViewLayer(
									_pointConverter, _view, null, Settings.PaletteColors,
									Settings.EliminationColor, Settings.CannibalismColor, Settings.ChainColor));

							UpdateImageGrid();
						}

						break;
					}
				}

				_layerCollection.Add(new FocusLayer(_pointConverter, _focusedCells, Settings.FocusedCellColor));

				UpdateImageGrid();
			}
		}

		private void ImageUndoIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
			MenuItemEditUndo_Click(sender, e);

		private void ImageRedoIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
			MenuItemEditRedo_Click(sender, e);

		private void ImageGeneratingIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
			((Action<object, RoutedEventArgs>)(
				_comboBoxMode.SelectedIndex switch
				{
					0 => (sender, e) => MenuItemGenerateWithSymmetry_Click(sender, e),
					1 => (sender, e) => MenuItemGenerateHardPattern_Click(sender, e),
					_ => throw Throwings.ImpossibleCase
				})
			)(sender, e);

		private void ImageSolve_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
			MenuItemAnalyzeAnalyze_Click(sender, e);
	}
}
