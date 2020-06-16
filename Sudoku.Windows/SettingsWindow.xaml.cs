using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Sudoku.Drawing.Extensions;
using Sudoku.Extensions;
using Sudoku.Solving;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual;
using Sudoku.Windows.Constants;
using Sudoku.Windows.Extensions;
using Sudoku.Windows.Tooling;
using static System.Reflection.BindingFlags;
using CoreResources = Sudoku.Windows.Resources;

namespace Sudoku.Windows
{
	/// <summary>
	/// Indicates the assignment handler.
	/// </summary>
	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	internal delegate void AssignmentHandler();

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
		/// Indicates the handler that finish all assignments.
		/// </summary>
		private AssignmentHandler? _assigments;


		/// <summary>
		/// Initializes an instance with a <see cref="Windows.Settings"/> instance
		/// and a <see cref="ManualSolver"/> instance.
		/// </summary>
		/// <param name="settings">The settings instance.</param>
		/// <param name="manualSolver">The manual solver.</param>
		public SettingsWindow(Settings settings, ManualSolver manualSolver)
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
		public Settings Settings { get; }


		/// <summary>
		/// Initialize setting controls.
		/// </summary>
		private void InitializeSettingControls()
		{
			_checkBoxAskWhileQuitting.IsChecked = Settings.AskWhileQuitting;
			_checkBoxEnableGcForcedly.IsChecked = Settings.MainManualSolver.EnableGarbageCollectionForcedly;
			_checkBoxSolveFromCurrent.IsChecked = Settings.SolveFromCurrent;
			_checkBoxTextFormatPlaceholdersAreZero.IsChecked = Settings.TextFormatPlaceholdersAreZero;
			_checkBoxPmGridCompatible.IsChecked = Settings.PmGridCompatible;
			_textBoxMaxLength.Text = Settings.MainManualSolver.AicMaximumLength.ToString();
			_checkBoxCheckLoop.IsChecked = Settings.MainManualSolver.CheckContinuousNiceLoop;
			_checkBoxCheckHeadCollision.IsChecked = Settings.MainManualSolver.CheckHeadCollision;
			_checkBoxOnlyRecordShortestPathAic.IsChecked = Settings.MainManualSolver.OnlySaveShortestPathAic;
			_checkBoxReductDifferentPathAic.IsChecked = Settings.MainManualSolver.ReductDifferentPathAic;
			_checkBoxAllowOverlappingAlses.IsChecked = Settings.MainManualSolver.AllowOverlappingAlses;
			_checkBoxHighlightRegions.IsChecked = Settings.MainManualSolver.AlsHighlightRegionInsteadOfCell;
			_checkBoxAllowAlsCycles.IsChecked = Settings.MainManualSolver.AllowAlsCycles;
			_textBoxBowmanBingoMaxLength.Text = Settings.MainManualSolver.BowmanBingoMaximumLength.ToString();
			_checkBoxAllowAlq.IsChecked = Settings.MainManualSolver.CheckAlmostLockedQuadruple;
			_checkBoxCheckIncompleteUr.IsChecked = Settings.MainManualSolver.CheckIncompleteUniquenessPatterns;
			_numericUpDownMaxRegularWingSize.CurrentValue = Settings.MainManualSolver.CheckRegularWingSize;
			_checkBoxUseExtendedBugSearcher.IsChecked = Settings.MainManualSolver.UseExtendedBugSearcher;
			_checkBoxSearchExtendedUniqueRectangle.IsChecked = Settings.MainManualSolver.SearchExtendedUniqueRectangles;
			_numericUpDownMaxPetalsOfDeathBlossom.CurrentValue = Settings.MainManualSolver.MaxPetalsOfDeathBlossom;
			_checkBoxCheckAdvancedInExocet.IsChecked = Settings.MainManualSolver.CheckAdvancedInExocet;
			_textBoxMaximumSizeHobiwanFish.Text = Settings.MainManualSolver.HobiwanFishMaximumSize.ToString();
			_textBoxMaximumExofinsHobiwanFish.Text = Settings.MainManualSolver.HobiwanFishMaximumExofinsCount.ToString();
			_textBoxMaximumEndofinsHobiwanFish.Text = Settings.MainManualSolver.HobiwanFishMaximumEndofinsCount.ToString();
			_checkBoxHobiwanFishCheckTemplates.IsChecked = Settings.MainManualSolver.HobiwanFishCheckTemplates;
			_textBoxGridLineWidth.Text = Settings.GridLineWidth.ToString();
			_textBoxBlockLineWidth.Text = Settings.BlockLineWidth.ToString();
			_textBoxValueScale.Text = Settings.ValueScale.ToString();
			_textBoxCandidateScale.Text = Settings.CandidateScale.ToString();
			_labelGivenFontName.Content = Settings.GivenFontName;
			_labelGivenFontName.FontFamily = new FontFamily(Settings.GivenFontName);
			_labelModifiableFontName.Content = Settings.ModifiableFontName;
			_labelModifiableFontName.FontFamily = new FontFamily(Settings.ModifiableFontName);
			_labelCandidateFontName.Content = Settings.CandidateFontName;
			_labelCandidateFontName.FontFamily = new FontFamily(Settings.CandidateFontName);
			_buttonBackgroundColor.Background = new SolidColorBrush(Settings.BackgroundColor.ToWColor());
			_buttonGivenColor.Background = new SolidColorBrush(Settings.GivenColor.ToWColor());
			_buttonModifiableColor.Background = new SolidColorBrush(Settings.ModifiableColor.ToWColor());
			_buttonCandidateColor.Background = new SolidColorBrush(Settings.CandidateColor.ToWColor());
			_buttonFocusColor.Background = new SolidColorBrush(Settings.FocusedCellColor.ToWColor());
			_buttonGridLineColor.Background = new SolidColorBrush(Settings.GridLineColor.ToWColor());
			_buttonBlockLineColor.Background = new SolidColorBrush(Settings.BlockLineColor.ToWColor());
			_buttonChainColor.Background = new SolidColorBrush(Settings.ChainColor.ToWColor());
			_buttonColor1.Background = new SolidColorBrush(Settings.Color1.ToWColor());
			_buttonColor2.Background = new SolidColorBrush(Settings.Color2.ToWColor());
			_buttonColor3.Background = new SolidColorBrush(Settings.Color3.ToWColor());
			_buttonColor4.Background = new SolidColorBrush(Settings.Color4.ToWColor());
			_buttonColor5.Background = new SolidColorBrush(Settings.Color5.ToWColor());
			_buttonColor6.Background = new SolidColorBrush(Settings.Color6.ToWColor());
			_buttonColor7.Background = new SolidColorBrush(Settings.Color7.ToWColor());
			_buttonColor8.Background = new SolidColorBrush(Settings.Color8.ToWColor());
			_buttonColor9.Background = new SolidColorBrush(Settings.Color9.ToWColor());
			_buttonColor10.Background = new SolidColorBrush(Settings.Color10.ToWColor());
			_buttonColor11.Background = new SolidColorBrush(Settings.Color11.ToWColor());
			_buttonColor12.Background = new SolidColorBrush(Settings.Color12.ToWColor());
			_buttonColor13.Background = new SolidColorBrush(Settings.Color13.ToWColor());
			_buttonColor14.Background = new SolidColorBrush(Settings.Color14.ToWColor());
			_buttonColor15.Background = new SolidColorBrush(Settings.Color15.ToWColor());
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
			_listBoxPriority.SetValue(
				ItemsControl.ItemsSourceProperty,
				from type in
					from type in Assembly.Load("Sudoku.Solving").GetTypes()
					where !type.IsAbstract && type.IsSubclassOf<TechniqueSearcher>()
						&& type.HasMarked<TechniqueDisplayAttribute>() && !type.HasMarked<HasBugAttribute>()
					select type
				let Priority = (int)(type.GetProperty("Priority", Public | Static)!.GetValue(null)!)
				orderby Priority
				select new ListBoxItem
				{
					Content =
						new PrimaryElementTuple<string, int, Type>(
							CoreResources.GetValue(
								$"Progress{type.GetCustomAttribute<TechniqueDisplayAttribute>()!.DisplayName}"),
							Priority,
							type)
				});
			_listBoxPriority.SelectedIndex = 0;
			var (_, priority, selectionType) =
				(PrimaryElementTuple<string, int, Type>)((ListBoxItem)_listBoxPriority.SelectedItem).Content;
			_checkBoxIsEnabled.IsEnabled = !selectionType.HasMarked<AlwaysEnableAttribute>();
			_textBoxPriority.Text = priority.ToString();
		}

		/// <summary>
		/// To handle the color settings.
		/// </summary>
		/// <param name="sender">The object to trigger the event.</param>
		/// <param name="settings">The setting target instance.</param>
		/// <param name="colorIndex">The index.</param>
		private void HandleColor(object sender, Settings settings, int colorIndex)
		{
			if (sender is Button button && ColorPicker.ShowDialog(out var color) && !(color is null))
			{
				var target = color.Value.ToDColor();
				typeof(Settings).GetProperty($"Color{colorIndex}")!.SetValue(settings, target);
				button.Background = new SolidColorBrush(target.ToWColor());
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

		private void CheckBoxSearchExtendedUniqueRectangle_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxSearchExtendedUniqueRectangle.IsChecked = _manualSolver.SearchExtendedUniqueRectangles ^= true;

		private void NumericUpDownMaxPetalsOfDeathBlossom_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () => _manualSolver.MaxPetalsOfDeathBlossom = _numericUpDownMaxPetalsOfDeathBlossom.CurrentValue;

		private void CheckBoxCheckAdvancedInExocet_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxCheckAdvancedInExocet.IsChecked = _manualSolver.CheckAdvancedInExocet ^= true;

		private void TextBoxMaximumSizeHobiwanFish_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox box && int.TryParse(box.Text, out int value))
			{
				if (value >= 2 && value <= 7)
				{
					_assigments += () => _manualSolver.HobiwanFishMaximumSize = value;
				}
				else
				{
					Messagings.CheckInput();
				}
			}
		}

		private void TextBoxMaximumExofinsHobiwanFish_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox box && int.TryParse(box.Text, out int value))
			{
				if (value >= 0 && value <= 10)
				{
					_assigments += () => _manualSolver.HobiwanFishMaximumExofinsCount = value;
				}
				else
				{
					Messagings.CheckInput();
				}
			}
		}

		private void TextBoxMaximumEndofinsHobiwanFish_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox box && int.TryParse(box.Text, out int value))
			{
				if (value >= 0 && value <= 10)
				{
					_assigments += () => _manualSolver.HobiwanFishMaximumEndofinsCount = value;
				}
				else
				{
					Messagings.CheckInput();
				}
			}
		}

		private void CheckBoxHobiwanFishCheckTemplates_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxHobiwanFishCheckTemplates.IsChecked = _manualSolver.HobiwanFishCheckTemplates ^= true;

		private void TextBoxGridLineWidth_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox && float.TryParse(textBox.Text, out float value))
			{
				_assigments += () => Settings.GridLineWidth = value;
			}
		}

		private void TextBoxBlockLineWidth_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox && float.TryParse(textBox.Text, out float value))
			{
				_assigments += () => Settings.BlockLineWidth = value;
			}
		}

		private void TextBoxValueScale_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox && decimal.TryParse(textBox.Text, out decimal value))
			{
				_assigments += () => Settings.ValueScale = value;
			}
		}

		private void TextBoxCandidateScale_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox && decimal.TryParse(textBox.Text, out decimal value))
			{
				_assigments += () => Settings.CandidateScale = value;
			}
		}

		private void ButtonGivenFontName_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FontDialog();
			if (dialog.ShowDialog() is true)
			{
				_assigments += () =>
				{
					_labelGivenFontName.FontFamily = new FontFamily(Settings.GivenFontName = dialog.SelectedFont.Name);
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
					_labelModifiableFontName.FontFamily = new FontFamily(
						Settings.ModifiableFontName = dialog.SelectedFont.Name);
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
					_labelCandidateFontName.FontFamily = new FontFamily(
						Settings.CandidateFontName = dialog.SelectedFont.Name);
					_labelCandidateFontName.Content = dialog.SelectedFont.Name;
				};
			}
		}

		private void ButtonBackgroundColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color) && !(color is null))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.BackgroundColor = z.ToDColor();
					_buttonBackgroundColor.Background = new SolidColorBrush(z);
				};
			}
		}

		private void ButtonGivenColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color) && !(color is null))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.GivenColor = z.ToDColor();
					_buttonGivenColor.Background = new SolidColorBrush(z);
				};
			}
		}

		private void ButtonModifiableColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color) && !(color is null))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.ModifiableColor = z.ToDColor();
					_buttonModifiableColor.Background = new SolidColorBrush(z);
				};
			}
		}

		private void ButtonCandidateColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color) && !(color is null))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.CandidateColor = z.ToDColor();
					_buttonCandidateColor.Background = new SolidColorBrush(z);
				};
			}
		}

		private void ButtonFocusColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color) && !(color is null))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.FocusedCellColor = z.ToDColor();
					_buttonFocusColor.Background = new SolidColorBrush(z);
				};
			}
		}

		private void ButtonGridLineColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color) && !(color is null))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.GridLineColor = z.ToDColor();
					_buttonGridLineColor.Background = new SolidColorBrush(z);
				};
			}
		}

		private void ButtonBlockLineColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color) && !(color is null))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.BlockLineColor = z.ToDColor();
					_buttonBlockLineColor.Background = new SolidColorBrush(z);
				};
			}
		}

		private void ButtonChainColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color) && !(color is null))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.ChainColor = z.ToDColor();
					_buttonChainColor.Background = new SolidColorBrush(z);
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

		private void TextBoxMaxLength_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox && int.TryParse(textBox.Text, out int value))
			{
				if (value >= 3 && value <= 20)
				{
					_assigments += () => _manualSolver.AicMaximumLength = value;
				}
				else
				{
					Messagings.CheckInput();
				}
			}
		}

		private void CheckBoxAllowOverlappingAlses_Click(object sender, RoutedEventArgs e) =>
			_checkBoxAllowOverlappingAlses.IsChecked = _manualSolver.AllowOverlappingAlses ^= true;

		private void CheckBoxHighlightRegions_Click(object sender, RoutedEventArgs e) =>
			_checkBoxHighlightRegions.IsChecked = _manualSolver.AlsHighlightRegionInsteadOfCell ^= true;

		private void CheckBoxAllowAlsCycles_Click(object sender, RoutedEventArgs e) =>
			_checkBoxAllowAlsCycles.IsChecked = _manualSolver.AllowAlsCycles ^= true;

		private void TextBoxBowmanBingoMaxLength_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox && int.TryParse(textBox.Text, out int value))
			{
				if (value >= 1 && value <= 64)
				{
					_assigments += () => _manualSolver.BowmanBingoMaximumLength = value;
				}
				else
				{
					Messagings.CheckInput();
				}
			}
		}

		private void CheckBoxAllowAlq_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxAllowAlq.IsChecked = _manualSolver.CheckAlmostLockedQuadruple ^= true;

		private void CheckBoxCheckLoop_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxCheckLoop.IsChecked = _manualSolver.CheckContinuousNiceLoop ^= true;

		private void CheckBoxCheckHeadCollision_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxCheckHeadCollision.IsChecked = _manualSolver.CheckHeadCollision ^= true;

		private void CheckBoxCheckIncompleteUr_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxCheckIncompleteUr.IsChecked = _manualSolver.CheckIncompleteUniquenessPatterns ^= true;

		private void NumericUpDownMaxRegularWingSize_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () => _manualSolver.CheckRegularWingSize = _numericUpDownMaxRegularWingSize.CurrentValue;

		private void CheckBoxOnlyRecordShortestPathAic_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxOnlyRecordShortestPathAic.IsChecked = _manualSolver.OnlySaveShortestPathAic ^= true;

		private void CheckBoxReductDifferentPathAic_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxReductDifferentPathAic.IsChecked = _manualSolver.ReductDifferentPathAic ^= true;

		private void CheckBoxUseExtendedBugSearcher_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxUseExtendedBugSearcher.IsChecked = _manualSolver.UseExtendedBugSearcher ^= true;

		private void TextBoxBowmanBingoMaxLength_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (!(sender is TextBox textBox && e.Key.IsDigit() && string.IsNullOrEmpty(textBox.Text)))
			{
				e.Handled = true;
				return;
			}
		}

		private void TextBoxMaxRegularWingSize_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (!(sender is TextBox textBox && e.Key.IsDigit() && string.IsNullOrEmpty(textBox.Text)))
			{
				e.Handled = true;
				return;
			}
		}

		private void TextBoxMaxLength_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (!(sender is TextBox textBox && e.Key.IsDigit() && string.IsNullOrEmpty(textBox.Text)))
			{
				e.Handled = true;
				return;
			}
		}

		private void CheckBoxEnableGcForcedly_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxEnableGcForcedly.IsEnabled = _manualSolver.EnableGarbageCollectionForcedly ^= true;

		private void ListBoxPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListBox listBox
				&& listBox.SelectedIndex != -1
				&& listBox.SelectedItem is ListBoxItem listBoxItem
				&& listBoxItem.Content is PrimaryElementTuple<string, int, Type> triplet)
			{
				_assigments += () =>
				{
					var (_, priority, type) = triplet;
					_checkBoxIsEnabled.IsChecked = (bool)type.GetProperty("IsEnabled")!.GetValue(null)!;
					_textBoxPriority.Text = priority.ToString();
					_checkBoxIsEnabled.IsEnabled = _textBoxPriority.IsReadOnly = !type.HasMarked<AlwaysEnableAttribute>();
				};
			}
		}
	}
}
