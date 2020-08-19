using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Sudoku.Drawing.Extensions;
using Sudoku.Extensions;
using Sudoku.Solving;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual;
using Sudoku.Windows.Tooling;
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
			_numericUpDownMaximumSizeHobiwanFish.CurrentValue = Settings.MainManualSolver.HobiwanFishMaximumSize;
			_numericUpDownMaximumExofinsHobiwanFish.CurrentValue = Settings.MainManualSolver.HobiwanFishMaximumExofinsCount;
			_numericUpDownMaximumEndofinsHobiwanFish.CurrentValue = Settings.MainManualSolver.HobiwanFishMaximumEndofinsCount;
			_checkBoxHobiwanFishCheckTemplates.IsChecked = Settings.MainManualSolver.HobiwanFishCheckTemplates;
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
				from Type in
					from Type in Assembly.Load("Sudoku.Solving").GetTypes()
					where !Type.IsAbstract && Type.IsSubclassOf<TechniqueSearcher>()
						&& Type.HasMarked<TechniqueDisplayAttribute>()
					select Type
				let AttributeInstance = Type.GetCustomAttribute<SearcherPropertyAttribute>()
				where AttributeInstance is not null
				let Priority = AttributeInstance.Priority
				orderby Priority
				select new ListBoxItem
				{
					Content =
						new PriorKeyedTuple<string, int, Type>(
							CoreResources.GetValue(
								$"Progress{Type.GetCustomAttribute<TechniqueDisplayAttribute>()!.DisplayName}"),
							Priority,
							Type)
				});
			_listBoxPriority.SelectedIndex = 0;
			var (_, priority, selectionType, _) = (PriorKeyedTuple<string, int, Type>)((ListBoxItem)_listBoxPriority.SelectedItem).Content;
			_checkBoxIsEnabled.IsEnabled = !selectionType.GetCustomAttribute<SearcherPropertyAttribute>()!.IsReadOnly;
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
			if (sender is Button button && ColorPicker.ShowDialog(out var color) && color.HasValue)
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
			_assigments += () =>
			_manualSolver.MaxPetalsOfDeathBlossom = (int)_numericUpDownMaxPetalsOfDeathBlossom.CurrentValue;

		private void CheckBoxCheckAdvancedInExocet_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxCheckAdvancedInExocet.IsChecked = _manualSolver.CheckAdvancedInExocet ^= true;

		private void NumericUpDownMaximumSizeHobiwanFish_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_manualSolver.HobiwanFishMaximumSize = (int)_numericUpDownMaximumSizeHobiwanFish.CurrentValue;

		private void NumericUpDownExofinsHobiwanFish_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_manualSolver.HobiwanFishMaximumExofinsCount = (int)_numericUpDownMaximumExofinsHobiwanFish.CurrentValue;

		private void NumericUpDownMaximumEndofinsHobiwanFish_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_manualSolver.HobiwanFishMaximumEndofinsCount = (int)_numericUpDownMaximumEndofinsHobiwanFish.CurrentValue;

		private void CheckBoxHobiwanFishCheckTemplates_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxHobiwanFishCheckTemplates.IsChecked = _manualSolver.HobiwanFishCheckTemplates ^= true;

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
					_labelCandidateFontName.FontFamily = new(Settings.CandidateFontName = dialog.SelectedFont.Name);
					_labelCandidateFontName.Content = dialog.SelectedFont.Name;
				};
			}
		}

		private void ButtonBackgroundColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color) && color is not null)
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
			if (ColorPicker.ShowDialog(out var color) && color is not null)
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
			if (ColorPicker.ShowDialog(out var color) && color is not null)
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
			if (ColorPicker.ShowDialog(out var color) && color is not null)
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
			if (ColorPicker.ShowDialog(out var color) && color is not null)
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
			if (ColorPicker.ShowDialog(out var color) && color is not null)
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
			if (ColorPicker.ShowDialog(out var color) && color is not null)
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
			if (ColorPicker.ShowDialog(out var color) && color is not null)
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

		private void CheckBoxAllowOverlappingAlses_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxAllowOverlappingAlses.IsChecked = _manualSolver.AllowOverlappingAlses ^= true;

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
			_assigments += () => _manualSolver.CheckRegularWingSize = (int)_numericUpDownMaxRegularWingSize.CurrentValue;

		private void CheckBoxUseExtendedBugSearcher_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxUseExtendedBugSearcher.IsChecked = _manualSolver.UseExtendedBugSearcher ^= true;

		private void CheckBoxEnableGcForcedly_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxEnableGcForcedly.IsEnabled = _manualSolver.EnableGarbageCollectionForcedly ^= true;

		private void ListBoxPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListBox
			{
				SelectedIndex: not -1,
				SelectedItem: ListBoxItem { Content: PriorKeyedTuple<string, int, Type> triplet } listBoxItem
			} listBox)
			{
				var (_, priority, type, _) = triplet;
				var (isEnabled, isReadOnly, _, _) = type.GetCustomAttribute<SearcherPropertyAttribute>()!;
				_checkBoxIsEnabled.IsChecked = isEnabled;
				_checkBoxIsEnabled.IsEnabled = !isReadOnly;
				_textBoxPriority.Text = priority.ToString();
				_textBoxPriority.IsReadOnly = isReadOnly;
			}
		}

		private void CheckBoxIsEnabled_Click(object sender, RoutedEventArgs e)
		{
			if (sender is CheckBox checkBox)
			{
				var type = ((PriorKeyedTuple<string, int, Type>)((ListBoxItem)_listBoxPriority.SelectedItem).Content).Item3;
				var attr = type.GetCustomAttribute<SearcherPropertyAttribute>()!;
				attr.IsEnabled = checkBox.IsChecked ?? default;
			}
		}

		private void TextBoxPriority_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox && int.TryParse(textBox.Text, out int value))
			{
				var type = ((PriorKeyedTuple<string, int, Type>)((ListBoxItem)_listBoxPriority.SelectedItem).Content).Item3;
				var attr = type.GetCustomAttribute<SearcherPropertyAttribute>()!;
				attr.Priority = value;
			}
		}
	}
}
