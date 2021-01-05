using System;
using System.Extensions;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Sudoku.DocComments;
using Sudoku.Solving.Manual;
using Sudoku.Windows.Extensions;
using Sudoku.Windows.Tooling;
using CoreResources = Sudoku.Windows.Resources;
using StepTriplet = System.KeyedTuple<string, int, System.Type>;

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
		/// Indicates the handler that finish all assignments.
		/// </summary>
		private Assignment? _assigments;


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
			_numericUpDownMaximumSizeHobiwanFish.CurrentValue = Settings.MainManualSolver.HobiwanFishMaximumSize;
			_numericUpDownMaximumExofinsHobiwanFish.CurrentValue = Settings.MainManualSolver.HobiwanFishMaximumExofinsCount;
			_numericUpDownMaximumEndofinsHobiwanFish.CurrentValue = Settings.MainManualSolver.HobiwanFishMaximumEndofinsCount;
			_checkBoxHobiwanFishCheckTemplates.IsChecked = Settings.MainManualSolver.HobiwanFishCheckTemplates;
			_checkBoxShowDirectLines.IsChecked = Settings.ShowDirectLines;

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
		/// <param name="color">(<see langword="in"/> parameter) The color.</param>
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
			_listBoxPriority.ItemsSource =
				from type in Assembly.Load("Sudoku.Solving").GetTypes()
				where !type.IsAbstract && type.IsSubclassOf<StepSearcher>() && !type.IsDefined<ObsoleteAttribute>()
				let prior = TechniqueProperties.GetPropertiesFrom(type)!.Priority
				orderby prior
				let v = type.GetProperty("Properties", BindingFlags.Public | BindingFlags.Static)?.GetValue(null)
				let casted = v as TechniqueProperties
				where casted is not null && !casted.DisabledReason.Flags(DisabledReason.HasBugs)
				let c = new StepTriplet(CoreResources.GetValue($"Progress{casted.DisplayLabel}"), prior, type)
				select new ListBoxItem
				{
					Content = c,
					HorizontalContentAlignment = HorizontalAlignment.Left,
					VerticalContentAlignment = VerticalAlignment.Center
				};

			_listBoxPriority.SelectedIndex = 0;
			var (_, priority, selectedType, _) = (StepTriplet)((ListBoxItem)_listBoxPriority.SelectedItem).Content;
			_checkBoxIsEnabled.IsEnabled = !TechniqueProperties.GetPropertiesFrom(selectedType)!.IsReadOnly;
			_textBoxPriority.Text = priority.ToString();
		}

		/// <summary>
		/// To handle the color settings.
		/// </summary>
		/// <param name="sender">The object to trigger the event.</param>
		/// <param name="settings">The setting target instance.</param>
		/// <param name="colorIndex">The index.</param>
		private void HandleColor(object sender, WindowsSettings settings, int colorIndex)
		{
			if (sender is Button button && ColorPicker.ShowDialog(out var color) && color.HasValue)
			{
				var target = color.Value.ToDColor();
				typeof(WindowsSettings).GetProperty($"Color{colorIndex}")!.SetValue(settings, target);
				ChangeColor(button, target.ToWColor());
			}
		}


		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{
			// Execute all assignments.
			_assigments?.Invoke();

			CloseWindow(true);
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCancel_Click(object sender, RoutedEventArgs e) => CloseWindow(false);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxAskWhileQuitting_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxAskWhileQuitting.IsChecked = Settings.AskWhileQuitting ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxSolveFromCurrent_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxSolveFromCurrent.IsChecked = Settings.SolveFromCurrent ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxTextFormatPlaceholdersAreZero_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxTextFormatPlaceholdersAreZero.IsChecked = Settings.TextFormatPlaceholdersAreZero ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxPmGridCompatible_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxPmGridCompatible.IsChecked = Settings.PmGridCompatible ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxOnlyShowSameLevelStepsInFindAllSteps_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxOnlyShowSameLevelStepsInFindAllSteps.IsChecked = _manualSolver.OnlyShowSameLevelTechniquesInFindAllSteps ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowStepLabel_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxShowStepLabel.IsChecked = Settings.ShowStepLabel ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowStepDifficulty_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxShowStepDifficulty.IsChecked = Settings.ShowStepDifficulty ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxSearchExtendedUniqueRectangle_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxSearchExtendedUniqueRectangle.IsChecked = _manualSolver.SearchExtendedUniqueRectangles ^= true;

		/// <inheritdoc cref="Events.SizeChanged(object?, EventArgs)"/>
		private void NumericUpDownMaxPetalsOfDeathBlossom_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_manualSolver.MaxPetalsOfDeathBlossom = (int)_numericUpDownMaxPetalsOfDeathBlossom.CurrentValue;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxCheckAdvancedInExocet_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxCheckAdvancedInExocet.IsChecked = _manualSolver.CheckAdvancedInExocet ^= true;

		/// <inheritdoc cref="Events.SizeChanged(object?, EventArgs)"/>
		private void NumericUpDownMaximumSizeHobiwanFish_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_manualSolver.HobiwanFishMaximumSize = (int)_numericUpDownMaximumSizeHobiwanFish.CurrentValue;

		/// <inheritdoc cref="Events.SizeChanged(object?, EventArgs)"/>
		private void NumericUpDownExofinsHobiwanFish_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_manualSolver.HobiwanFishMaximumExofinsCount = (int)_numericUpDownMaximumExofinsHobiwanFish.CurrentValue;

		/// <inheritdoc cref="Events.SizeChanged(object?, EventArgs)"/>
		private void NumericUpDownMaximumEndofinsHobiwanFish_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_manualSolver.HobiwanFishMaximumEndofinsCount = (int)_numericUpDownMaximumEndofinsHobiwanFish.CurrentValue;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxHobiwanFishCheckTemplates_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxHobiwanFishCheckTemplates.IsChecked = _manualSolver.HobiwanFishCheckTemplates ^= true;

		/// <inheritdoc cref="Events.SizeChanged(object?, EventArgs)"/>
		private void NumericUpDownGridLineWidth_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () => Settings.GridLineWidth = (float)_numericUpDownGridLineWidth.CurrentValue;

		/// <inheritdoc cref="Events.SizeChanged(object?, EventArgs)"/>
		private void NumericUpDownBlockLineWidth_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () => Settings.BlockLineWidth = (float)_numericUpDownBlockLineWidth.CurrentValue;

		/// <inheritdoc cref="Events.SizeChanged(object?, EventArgs)"/>
		private void NumericUpDownValueScale_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () => Settings.ValueScale = _numericUpDownValueScale.CurrentValue;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void NumericUpDownCandidateScale_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () => Settings.CandidateScale = _numericUpDownCandidateScale.CurrentValue;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonBackgroundColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.BackgroundColor = z.ToDColor();
					ChangeColor(_buttonBackgroundColor, z);
				};
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonGivenColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.GivenColor = z.ToDColor();
					ChangeColor(_buttonGivenColor, z);
				};
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonModifiableColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.ModifiableColor = z.ToDColor();
					ChangeColor(_buttonModifiableColor, z);
				};
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCandidateColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.CandidateColor = z.ToDColor();
					ChangeColor(_buttonCandidateColor, z);
				};
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonFocusColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.FocusedCellColor = z.ToDColor();
					ChangeColor(_buttonFocusColor, z);
				};
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonGridLineColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.GridLineColor = z.ToDColor();
					ChangeColor(_buttonGridLineColor, z);
				};
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonBlockLineColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.BlockLineColor = z.ToDColor();
					ChangeColor(_buttonBlockLineColor, z);
				};
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonChainColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.ChainColor = z.ToDColor();
					ChangeColor(_buttonChainColor, z);
				};
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCrosshatchingOutlineColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.CrosshatchingInnerColor = z.ToDColor();
					ChangeColor(_buttonCrosshatchingOutlineColor, z);
				};
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCrosshatchingValuesColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.CrosshatchingOutlineColor = z.ToDColor();
					ChangeColor(_buttonCrosshatchingValuesColor, z);
				};
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCrossSignColor_Click(object sender, RoutedEventArgs e)
		{
			if (ColorPicker.ShowDialog(out var color))
			{
				_assigments += () =>
				{
					var z = color.Value;
					Settings.CrossSignColor = z.ToDColor();
					ChangeColor(_buttonCrossSignColor, z);
				};
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor1_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 1);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor2_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 2);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor3_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 3);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor4_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 4);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor5_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 5);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor6_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 6);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor7_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 7);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor8_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 8);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor9_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 9);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor10_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 10);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor11_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 11);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor12_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 12);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor13_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 13);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor14_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 14);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonColor15_Click(object sender, RoutedEventArgs e) => HandleColor(sender, Settings, 15);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxAllowOverlappingAlses_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxAllowOverlappingAlses.IsChecked = _manualSolver.AllowOverlappingAlses ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxHighlightRegions_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxHighlightRegions.IsChecked = _manualSolver.AlsHighlightRegionInsteadOfCell ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxAllowAlsCycles_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxAllowAlsCycles.IsChecked = _manualSolver.AllowAlsCycles ^= true;

		/// <inheritdoc cref="Events.ValueChanged(object?, EventArgs)"/>
		private void NumericUpDownBowmanBingoMaxLength_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_manualSolver.BowmanBingoMaximumLength = (int)_numericUpDownBowmanBingoMaxLength.CurrentValue;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxAllowAlq_Click(object sender, RoutedEventArgs e) =>
			_assigments += () => _checkBoxAllowAlq.IsChecked = _manualSolver.CheckAlmostLockedQuadruple ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxCheckIncompleteUr_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxCheckIncompleteUr.IsChecked = _manualSolver.CheckIncompleteUniquenessPatterns ^= true;

		/// <inheritdoc cref="Events.ValueChanged(object?, EventArgs)"/>
		private void NumericUpDownMaxRegularWingSize_ValueChanged(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_manualSolver.CheckRegularWingSize = (int)_numericUpDownMaxRegularWingSize.CurrentValue;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxUseExtendedBugSearcher_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxUseExtendedBugSearcher.IsChecked = _manualSolver.UseExtendedBugSearcher ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxEnableGcForcedly_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxEnableGcForcedly.IsEnabled = _manualSolver.EnableGarbageCollectionForcedly ^= true;

		/// <inheritdoc cref="Events.SelectionChanged(object?, EventArgs)"/>
		private void ListBoxPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (
				sender is ListBox
				{
					SelectedIndex: not -1,
					SelectedItem: ListBoxItem { Content: StepTriplet triplet } listBoxItem
				} listBox)
			{
				var (_, priority, type, _) = triplet;
				var (isEnabled, isReadOnly) = TechniqueProperties.GetPropertiesFrom(type)!;
				_checkBoxIsEnabled.IsChecked = isEnabled;
				_checkBoxIsEnabled.IsEnabled = !isReadOnly;
				_textBoxPriority.Text = priority.ToString();
				_textBoxPriority.IsReadOnly = isReadOnly;
			}
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowDirectLines_Click(object sender, RoutedEventArgs e) =>
			_assigments += () =>
			_checkBoxShowDirectLines.IsEnabled = Settings.ShowDirectLines = _manualSolver.ShowDirectLines ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxIsEnabled_Click(object sender, RoutedEventArgs e)
		{
			if (sender is CheckBox checkBox)
			{
				TechniqueProperties.GetPropertiesFrom(
					((StepTriplet)((ListBoxItem)_listBoxPriority.SelectedItem).Content).Item3
				)!.IsEnabled = checkBox.IsChecked ?? false;
			}
		}

		/// <inheritdoc cref="Events.TextChanged(object?, EventArgs)"/>
		private void TextBoxPriority_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox && int.TryParse(textBox.Text, out int value))
			{
				TechniqueProperties.GetPropertiesFrom(
					((StepTriplet)((ListBoxItem)_listBoxPriority.SelectedItem).Content).Item3
				)!.Priority = value;
			}
		}
	}
}
