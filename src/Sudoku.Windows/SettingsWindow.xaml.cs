using System;
using System.Collections.ObjectModel;
using System.Extensions;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Sudoku.Solving.Manual;
using Sudoku.Windows.CustomControls;
using Sudoku.Windows.Extensions;
using StepTriplet = System.Collections.Generic.KeyedTuple<string, int, System.Type>;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>SettingsWindow.xaml</c>.
	/// </summary>
	public partial class SettingsWindow : Window
	{
		/// <summary>
		/// The manual solver used.
		/// </summary>
		private readonly ManualSolver _manualSolver;

		/// <summary>
		/// Indicates the searchers list.
		/// </summary>
		private readonly ObservableCollection<ListBoxItem> _priorityControls = new();

		///// <summary>
		///// Indicates the undo stack.
		///// </summary>
		//private readonly Stack<ObservableCollection<ListBoxItem>> _undoStack = new();
		//
		///// <summary>
		///// Indicates the redo stack.
		///// </summary>
		//private readonly Stack<ObservableCollection<ListBoxItem>> _redoStack = new();


		/// <summary>
		/// Indicates the handler that finish all assignments.
		/// </summary>
		private Action? _assigments;


		/// <summary>
		/// Initializes an instance with a <see cref="WindowsSettings"/> instance
		/// and a <see cref="ManualSolver"/> instance.
		/// </summary>
		/// <param name="settings">The settings instance.</param>
		/// <param name="manualSolver">The manual solver.</param>
		public SettingsWindow(WindowsSettings settings, ManualSolver manualSolver)
		{
			InitializeComponent();

			_manualSolver = manualSolver;
			Settings = settings;

			InitializeSettingControls();
			InitializePriorityControls();
		}


		/// <summary>
		/// Indicates the result settings.
		/// </summary>
		public WindowsSettings Settings { get; }


		///// <inheritdoc/>
		//protected override void OnKeyDown(KeyEventArgs e)
		//{
		//	switch (e.Key)
		//	{
		//		case Key.Z when Keyboard.Modifiers == ModifierKeys.Control && _undoStack.Count != 0:
		//		{
		//			// Undo a step.
		//			var triplets = _undoStack.Pop();
		//			_redoStack.Push(triplets);
		//
		//			// Then cover the old list.
		//			for (int i = 0; i < triplets.Count; i++)
		//			{
		//				_priorityControls[i] = triplets[i];
		//			}
		//
		//			break;
		//		}
		//		case Key.Y when Keyboard.Modifiers == ModifierKeys.Control && _redoStack.Count != 0:
		//		{
		//			// Redo a step.
		//			var triplets = _redoStack.Pop();
		//			_undoStack.Push(triplets);
		//
		//			// Then cover the old list.
		//			for (int i = 0; i < triplets.Count; i++)
		//			{
		//				_priorityControls[i] = triplets[i];
		//			}
		//
		//			break;
		//		}
		//	}
		//}

		/// <summary>
		/// Initialize setting controls.
		/// </summary>
		private void InitializeSettingControls()
		{
			// Page 1.
			_checkBoxAskWhileQuitting.IsChecked = Settings.AskWhileQuitting;
			_checkBoxEnableGcForcedly.IsChecked = Settings.MainManualSolver.EnableGarbageCollectionForcedly;
			_checkBoxSolveFromCurrent.IsChecked = Settings.SolveFromCurrent;
			_checkBoxTextFormatPlaceholdersAreZero.IsChecked = Settings.TextFormatPlaceholdersAreZero;
			_checkBoxPmGridCompatible.IsChecked = Settings.PmGridCompatible;
			_checkBoxOnlyShowSameLevelStepsInFindAllSteps.IsChecked = Settings.MainManualSolver.OnlyShowSameLevelTechniquesInFindAllSteps;
			_checkBoxShowStepLabel.IsChecked = Settings.ShowStepLabel;
			_checkBoxShowStepDifficulty.IsChecked = Settings.ShowStepDifficulty;
			_checkBoxDisplayAbbrRatherThanFullNameOfSteps.IsChecked = Settings.DisplayAcronymRatherThanFullNameOfSteps;
			_checkBoxShowLightRegion.IsChecked = Settings.ShowLightRegion;

			// Page 2.
			_checkBoxAllowOverlappingAlses.IsChecked = Settings.MainManualSolver.AllowOverlappingAlses;
			_checkBoxHighlightRegions.IsChecked = Settings.MainManualSolver.AlsHighlightRegionInsteadOfCell;
			_checkBoxAllowAlsCycles.IsChecked = Settings.MainManualSolver.AllowAlsCycles;
			_numericUpDownBowmanBingoMaxLength.CurrentValue = Settings.MainManualSolver.BowmanBingoMaximumLength;
			_checkBoxAllowAlq.IsChecked = Settings.MainManualSolver.CheckAlmostLockedQuadruple;
			_checkBoxCheckIncompleteUr.IsChecked = Settings.MainManualSolver.CheckIncompleteUniquenessPatterns;
			_numericUpDownMaxRegularWingSize.CurrentValue = Settings.MainManualSolver.CheckRegularWingSize;
			_checkBoxUseExtendedBugSearcher.IsChecked = Settings.MainManualSolver.UseExtendedBugSearcher;
			_checkBoxSearchExtendedUniqueRectangle.IsChecked = Settings.MainManualSolver.SearchExtendedUniqueRectangles;
			_numericUpDownMaxPetalsOfDeathBlossom.CurrentValue = Settings.MainManualSolver.MaxPetalsOfDeathBlossom;
			_checkBoxCheckAdvancedInExocet.IsChecked = Settings.MainManualSolver.CheckAdvancedInExocet;
			_checkBoxShowDirectLines.IsChecked = Settings.ShowDirectLines;
			_numericUpDownComplexFishMaxSize.CurrentValue = Settings.MainManualSolver.ComplexFishMaxSize;

			// Page 3.
			_numericUpDownGridLineWidth.CurrentValue = (decimal)Settings.GridLineWidth;
			_numericUpDownBlockLineWidth.CurrentValue = (decimal)Settings.BlockLineWidth;
			_numericUpDownValueScale.CurrentValue = Settings.ValueScale;
			_numericUpDownCandidateScale.CurrentValue = Settings.CandidateScale;
			_labelGivenFontName.Content = Settings.GivenFontName;
			_labelGivenFontName.FontFamily = new(Settings.GivenFontName);
			_labelModifiableFontName.Content = Settings.ModifiableFontName;
			_labelModifiableFontName.FontFamily = new(Settings.ModifiableFontName);
			_labelCandidateFontName.Content = Settings.CandidateFontName;
			_labelCandidateFontName.FontFamily = new(Settings.CandidateFontName);
			ChangeColor(_buttonBackgroundColor, Settings.BackgroundColor.ToWColor());
			ChangeColor(_buttonGivenColor, Settings.GivenColor.ToWColor());
			ChangeColor(_buttonModifiableColor, Settings.ModifiableColor.ToWColor());
			ChangeColor(_buttonCandidateColor, Settings.CandidateColor.ToWColor());
			ChangeColor(_buttonFocusColor, Settings.FocusedCellColor.ToWColor());
			ChangeColor(_buttonGridLineColor, Settings.GridLineColor.ToWColor());
			ChangeColor(_buttonBlockLineColor, Settings.BlockLineColor.ToWColor());
			ChangeColor(_buttonChainColor, Settings.ChainColor.ToWColor());
			ChangeColor(_buttonCrosshatchingOutlineColor, Settings.CrosshatchingInnerColor.ToWColor());
			ChangeColor(_buttonCrosshatchingValuesColor, Settings.CrosshatchingOutlineColor.ToWColor());
			ChangeColor(_buttonCrossSignColor, Settings.CrossSignColor.ToWColor());
			ChangeColor(_buttonColor1, Settings.Color1.ToWColor());
			ChangeColor(_buttonColor2, Settings.Color2.ToWColor());
			ChangeColor(_buttonColor3, Settings.Color3.ToWColor());
			ChangeColor(_buttonColor4, Settings.Color4.ToWColor());
			ChangeColor(_buttonColor5, Settings.Color5.ToWColor());
			ChangeColor(_buttonColor6, Settings.Color6.ToWColor());
			ChangeColor(_buttonColor7, Settings.Color7.ToWColor());
			ChangeColor(_buttonColor8, Settings.Color8.ToWColor());
			ChangeColor(_buttonColor9, Settings.Color9.ToWColor());
			ChangeColor(_buttonColor10, Settings.Color10.ToWColor());
			ChangeColor(_buttonColor11, Settings.Color11.ToWColor());
			ChangeColor(_buttonColor12, Settings.Color12.ToWColor());
			ChangeColor(_buttonColor13, Settings.Color13.ToWColor());
			ChangeColor(_buttonColor14, Settings.Color14.ToWColor());
			ChangeColor(_buttonColor15, Settings.Color15.ToWColor());
		}

		/// <summary>
		/// Change the button's background and foreground color.
		/// </summary>
		/// <param name="button">The button.</param>
		/// <param name="color">The color.</param>
		private void ChangeColor(Button button, in Color color)
		{
			button.Background = new SolidColorBrush(color);
			button.Foreground =
				new SolidColorBrush(
					(.299 * color.R + .587 * color.G + .114 * color.B) / 255 > .5 ? Colors.Black : Colors.White
				);
		}

		/// <summary>
		/// Close the window using the specified <see cref="bool"/> value indicating
		/// whether the window runs successfully.
		/// </summary>
		/// <param name="value">The value.</param>
		private void CloseWindow(bool value)
		{
			DialogResult = value;
			Close();
		}

		/// <summary>
		/// Initialize priority controls.
		/// </summary>
		private void InitializePriorityControls()
		{
			// Set the styles.
			var itemContainerStyle = new Style(typeof(ListBoxItem));
			var setters = itemContainerStyle.Setters;
			setters.Add(new Setter(AllowDropProperty, true));
			setters.Add(
				new EventSetter(
					PreviewMouseLeftButtonDownEvent,
					new MouseButtonEventHandler(PriorityControl_PreviewMouseLeftButtonDown)
				)
			);
			setters.Add(
				new EventSetter(
					DropEvent,
					new DragEventHandler(PriorityControl_Drop)
				)
			);
			_listBoxPriority.ItemContainerStyle = itemContainerStyle;

			// Set the controls.
			_priorityControls.AddRange(
				from triplet in StepSearcher.AllStepSearchers
				let props = triplet.Properties
				select new ListBoxItem()
				{
					Content = new StepTriplet(
						$"({props.Priority.ToString()}) {triplet.SearcherName}",
						props.Priority,
						triplet.CurrentType
					),
					HorizontalContentAlignment = HorizontalAlignment.Left,
					VerticalContentAlignment = VerticalAlignment.Center
				});

			// Assign.
			_listBoxPriority.ItemsSource = _priorityControls;

			// Update the view.
			_listBoxPriority.SelectedIndex = 0;
		}

		/// <summary>
		/// To handle the color settings.
		/// </summary>
		/// <param name="sender">The object to trigger the event.</param>
		/// <param name="settings">The setting target instance.</param>
		/// <param name="colorIndex">The index.</param>
		private void HandleColor(object sender, WindowsSettings settings, int colorIndex)
		{
			if (sender is Button button && ColorPickerWindow.ShowDialog(out var color) && color.HasValue)
			{
				var target = color.Value.ToDColor();
				typeof(WindowsSettings).GetProperty($"Color{colorIndex.ToString()}")!.SetValue(settings, target);
				ChangeColor(button, target.ToWColor());
			}
		}


		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{
			// Execute all assignments.
			_assigments?.Invoke();

			CloseWindow(true);
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e) => CloseWindow(false);

		private void CheckBoxAskWhileQuitting_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxAskWhileQuitting.IsChecked = Settings.AskWhileQuitting ^= true;

		private void CheckBoxSolveFromCurrent_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxSolveFromCurrent.IsChecked = Settings.SolveFromCurrent ^= true;

		private void CheckBoxTextFormatPlaceholdersAreZero_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxTextFormatPlaceholdersAreZero.IsChecked = Settings.TextFormatPlaceholdersAreZero ^= true;

		private void CheckBoxPmGridCompatible_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxPmGridCompatible.IsChecked = Settings.PmGridCompatible ^= true;

		private void CheckBoxOnlyShowSameLevelStepsInFindAllSteps_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxOnlyShowSameLevelStepsInFindAllSteps.IsChecked = _manualSolver.OnlyShowSameLevelTechniquesInFindAllSteps ^= true;

		private void CheckBoxDisplayAbbrRatherThanFullNameOfSteps_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxDisplayAbbrRatherThanFullNameOfSteps.IsChecked = Settings.DisplayAcronymRatherThanFullNameOfSteps ^= true;

		private void CheckBoxShowStepLabel_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxShowStepLabel.IsChecked = Settings.ShowStepLabel ^= true;

		private void CheckBoxShowStepDifficulty_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxShowStepDifficulty.IsChecked = Settings.ShowStepDifficulty ^= true;

		private void CheckBoxShowLightRegion_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxShowStepDifficulty.IsChecked = Settings.ShowLightRegion ^= true;

		private void CheckBoxSearchExtendedUniqueRectangle_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxSearchExtendedUniqueRectangle.IsChecked = _manualSolver.SearchExtendedUniqueRectangles ^= true;

		private void NumericUpDownMaxPetalsOfDeathBlossom_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_manualSolver.MaxPetalsOfDeathBlossom = (int)_numericUpDownMaxPetalsOfDeathBlossom.CurrentValue;

		private void CheckBoxCheckAdvancedInExocet_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxCheckAdvancedInExocet.IsChecked = _manualSolver.CheckAdvancedInExocet ^= true;

		private void NumericUpDownComplexFishMaxSize_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () => Settings.MainManualSolver.ComplexFishMaxSize = (int)_numericUpDownComplexFishMaxSize.CurrentValue;

		private void NumericUpDownGridLineWidth_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () => Settings.GridLineWidth = (float)_numericUpDownGridLineWidth.CurrentValue;

		private void NumericUpDownBlockLineWidth_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () => Settings.BlockLineWidth = (float)_numericUpDownBlockLineWidth.CurrentValue;

		private void NumericUpDownValueScale_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () => Settings.ValueScale = _numericUpDownValueScale.CurrentValue;

		private void NumericUpDownCandidateScale_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () => Settings.CandidateScale = _numericUpDownCandidateScale.CurrentValue;

		private void ButtonGivenFontName_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FontDialog();
			if (dialog.ShowDialog() is true)
			{
				_assigments += () =>
				{
					_labelGivenFontName.FontFamily = new(Settings.GivenFontName = dialog.SelectedFont.Name);
					_labelGivenFontName.Content = dialog.SelectedFont.Name;
				};
			}
		}

		private void ButtonModifiableFontName_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FontDialog();
			if (dialog.ShowDialog() is true)
			{
				_assigments += () =>
				{
					_labelModifiableFontName.FontFamily =
						new FontFamily(
							Settings.ModifiableFontName = dialog.SelectedFont.Name
						);
					_labelModifiableFontName.Content = dialog.SelectedFont.Name;
				};
			}
		}

		private void ButtonCandidateFontName_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FontDialog();
			if (dialog.ShowDialog() is true)
			{
				_assigments += () =>
				{
					_labelCandidateFontName.FontFamily = new(Settings.CandidateFontName = dialog.SelectedFont.Name);
					_labelCandidateFontName.Content = dialog.SelectedFont.Name;
				};
			}
		}

		private void ButtonBackgroundColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPickerWindow.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.BackgroundColor = z.ToDColor();
					ChangeColor(_buttonBackgroundColor, z);
				};
			}
		}

		private void ButtonGivenColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPickerWindow.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.GivenColor = z.ToDColor();
					ChangeColor(_buttonGivenColor, z);
				};
			}
		}

		private void ButtonModifiableColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPickerWindow.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.ModifiableColor = z.ToDColor();
					ChangeColor(_buttonModifiableColor, z);
				};
			}
		}

		private void ButtonCandidateColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPickerWindow.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.CandidateColor = z.ToDColor();
					ChangeColor(_buttonCandidateColor, z);
				};
			}
		}

		private void ButtonFocusColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPickerWindow.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.FocusedCellColor = z.ToDColor();
					ChangeColor(_buttonFocusColor, z);
				};
			}
		}

		private void ButtonGridLineColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPickerWindow.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.GridLineColor = z.ToDColor();
					ChangeColor(_buttonGridLineColor, z);
				};
			}
		}

		private void ButtonBlockLineColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPickerWindow.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.BlockLineColor = z.ToDColor();
					ChangeColor(_buttonBlockLineColor, z);
				};
			}
		}

		private void ButtonChainColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPickerWindow.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.ChainColor = z.ToDColor();
					ChangeColor(_buttonChainColor, z);
				};
			}
		}

		private void ButtonCrosshatchingOutlineColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPickerWindow.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.CrosshatchingInnerColor = z.ToDColor();
					ChangeColor(_buttonCrosshatchingOutlineColor, z);
				};
			}
		}

		private void ButtonCrosshatchingValuesColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPickerWindow.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.CrosshatchingOutlineColor = z.ToDColor();
					ChangeColor(_buttonCrosshatchingValuesColor, z);
				};
			}
		}

		private void ButtonCrossSignColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPickerWindow.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.CrossSignColor = z.ToDColor();
					ChangeColor(_buttonCrossSignColor, z);
				};
			}
		}

		private void ButtonColor1_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 1);

		private void ButtonColor2_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 2);

		private void ButtonColor3_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 3);

		private void ButtonColor4_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 4);

		private void ButtonColor5_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 5);

		private void ButtonColor6_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 6);

		private void ButtonColor7_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 7);

		private void ButtonColor8_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 8);

		private void ButtonColor9_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 9);

		private void ButtonColor10_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 10);

		private void ButtonColor11_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 11);

		private void ButtonColor12_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 12);

		private void ButtonColor13_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 13);

		private void ButtonColor14_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 14);

		private void ButtonColor15_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 15);

		private void CheckBoxAllowOverlappingAlses_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxAllowOverlappingAlses.IsChecked = _manualSolver.AllowOverlappingAlses ^= true;

		private void CheckBoxHighlightRegions_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxHighlightRegions.IsChecked = _manualSolver.AlsHighlightRegionInsteadOfCell ^= true;

		private void CheckBoxAllowAlsCycles_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxAllowAlsCycles.IsChecked = _manualSolver.AllowAlsCycles ^= true;

		private void NumericUpDownBowmanBingoMaxLength_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_manualSolver.BowmanBingoMaximumLength = (int)_numericUpDownBowmanBingoMaxLength.CurrentValue;

		private void CheckBoxAllowAlq_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxAllowAlq.IsChecked = _manualSolver.CheckAlmostLockedQuadruple ^= true;

		private void CheckBoxCheckIncompleteUr_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxCheckIncompleteUr.IsChecked = _manualSolver.CheckIncompleteUniquenessPatterns ^= true;

		private void NumericUpDownMaxRegularWingSize_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_manualSolver.CheckRegularWingSize = (int)_numericUpDownMaxRegularWingSize.CurrentValue;

		private void CheckBoxUseExtendedBugSearcher_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxUseExtendedBugSearcher.IsChecked = _manualSolver.UseExtendedBugSearcher ^= true;

		private void CheckBoxEnableGcForcedly_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxEnableGcForcedly.IsEnabled = _manualSolver.EnableGarbageCollectionForcedly ^= true;

		private void CheckBoxShowDirectLines_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxShowDirectLines.IsEnabled = Settings.ShowDirectLines = _manualSolver.ShowDirectLines ^= true;

		private void CheckBoxIsEnabled_Click(object sender, RoutedEventArgs e)
		{
			if (sender is CheckBox checkBox)
			{
				TechniqueProperties.FromType(
					((StepTriplet)((ListBoxItem)_listBoxPriority.SelectedItem).Content).Item3
				)!.IsEnabled = checkBox.IsChecked ?? false;
			}
		}

		private void TextBoxPriority_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox && int.TryParse(textBox.Text, out int value))
			{
				TechniqueProperties.FromType(
					((StepTriplet)((ListBoxItem)_listBoxPriority.SelectedItem).Content).Item3
				)!.Priority = value;
			}
		}

		private void PriorityControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is not ListBoxItem draggedItem)
			{
				return;
			}

			var type = ((StepTriplet)draggedItem.Content).Item3;
			if (TechniqueProperties.FromType(type)!.IsReadOnly)
			{
				// We can't modify the status of any fixed technique searchers.
				return;
			}

			DragDrop.DoDragDrop(draggedItem, draggedItem, DragDropEffects.Move);
			draggedItem.IsSelected = true;
		}

		private void PriorityControl_Drop(object sender, DragEventArgs e)
		{
			var droppedItem = (ListBoxItem)e.Data.GetData(typeof(ListBoxItem));
			var targetItem = (ListBoxItem)sender;

			int removedIndex = _listBoxPriority.Items.IndexOf(droppedItem);
			int targetIndex = _listBoxPriority.Items.IndexOf(targetItem);

			bool shouldRefresh = false;
			if (removedIndex < targetIndex)
			{
				_priorityControls.Insert(targetIndex + 1, droppedItem);
				_priorityControls.RemoveAt(removedIndex);

				shouldRefresh = true;
			}
			else
			{
				int newRemovedIndex = removedIndex + 1;
				if (_priorityControls.Count + 1 > newRemovedIndex)
				{
					_priorityControls.Insert(targetIndex, droppedItem);
					_priorityControls.RemoveAt(newRemovedIndex);

					shouldRefresh = true;
				}
			}

			// Refresh the list.
			if (shouldRefresh)
			{
				for (int index = 0; index < _priorityControls.Count; index++)
				{
					var (name, _, type, _) = (StepTriplet)_priorityControls[index].Content;

					_priorityControls[index].Content = new StepTriplet(
						$"({index.ToString()}) {name[(name.IndexOf(')') + 2)..]}",
						index,
						type
					);
					TechniqueProperties.FromType(type)!.Priority = index;
				}
			}
		}
	}
}
