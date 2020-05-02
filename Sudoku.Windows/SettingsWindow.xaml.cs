using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Sudoku.Drawing.Extensions;
using Sudoku.Extensions;
using Sudoku.Solving;
using Sudoku.Solving.Manual;
using Sudoku.Windows.Extensions;
using Sudoku.Windows.Tooling;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

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


		/// <include file='../GlobalDocComments.xml' path='comments/defaultConstructor'/>
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
			_checkBoxEnableGcForcedly.IsChecked = Settings.EnableGarbageCollectionForcedly;
			_checkBoxSolveFromCurrent.IsChecked = Settings.SolveFromCurrent;
			_checkBoxTextFormatPlaceholdersAreZero.IsChecked = Settings.TextFormatPlaceholdersAreZero;
			_checkBoxPmGridCompatible.IsChecked = Settings.PmGridCompatible;
			_textBoxMaxLength.Text = Settings.AicMaximumLength.ToString();
			_checkBoxCheckLoop.IsChecked = Settings.CheckContinuousNiceLoop;
			_checkBoxCheckHeadCollision.IsChecked = Settings.CheckHeadCollision;
			_checkBoxOnlyRecordShortestPathAic.IsChecked = Settings.OnlySaveShortestPathAic;
			_checkBoxReductDifferentPathAic.IsChecked = Settings.ReductDifferentPathAic;
			_checkBoxAllowOverlappingAlses.IsChecked = Settings.AllowOverlapAlses;
			_checkBoxHighlightRegions.IsChecked = Settings.AlsHighlightRegionInsteadOfCell;
			_textBoxBowmanBingoMaxLength.Text = Settings.BowmanBingoMaximumLength.ToString();
			_checkBoxAllowAlq.IsChecked = Settings.CheckAlmostLockedQuadruple;
			_checkBoxCheckUncompletedUr.IsChecked = Settings.CheckUncompletedUniquenessPatterns;
			_textBoxMaxRegularWingSize.Text = Settings.CheckRegularWingSize.ToString();
			_checkBoxUseExtendedBugSearcher.IsChecked = Settings.UseExtendedBugSearcher;
			_checkBoxSearchExtendedUniqueRectangle.IsChecked = Settings.SearchExtendedUniqueRectangles;
			_textBoxMaxPetalsOfDeathBlossom.Text = Settings.MaxPetalsOfDeathBlossom.ToString();
			_checkBoxCheckAdvancedInExocet.IsChecked = Settings.CheckAdvancedInExocet;
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
		/// Initialize priority controls.
		/// </summary>
		/// <exception cref="SudokuRuntimeException">
		/// Throws when the technique searcher is not marked <see cref="TechniqueDisplayAttribute"/>.
		/// </exception>
		private void InitializePriorityControls()
		{
			_listBoxPriority.ClearValue(ItemsControl.ItemsSourceProperty);

			var list = new List<ListBoxItem>();
			foreach (var type in from type in Assembly.Load("Sudoku.Solving").GetTypes()
								 where !type.IsAbstract && type.IsSubclassOf(typeof(TechniqueSearcher))
								 select type)
			{
				if (!type.HasMarkedAttribute<TechniqueDisplayAttribute>(false, out var attributes))
				{
					throw new SudokuRuntimeException(
						"The specified searcher does not contain any displaying information.");
				}

				var attribute = attributes.First();
				int priority = (int)(
					type.GetProperty("Priority", BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!);
				var item = new ListBoxItem
				{
					Content = new PrimaryElementTuple<string, int, Type>(attribute.DisplayName, priority, type)
				};

				list.Add(item);
			}

			list.Sort((a, b) =>
			{
				(_, int priority1, _) = (PrimaryElementTuple<string, int, Type>)a.Content;
				(_, int priority2, _) = (PrimaryElementTuple<string, int, Type>)b.Content;
				return priority1.CompareTo(priority2);
			});
			_listBoxPriority.ItemsSource = list;
			_listBoxPriority.SelectedIndex = 0;
			(_, int selectionPriority, _) = (PrimaryElementTuple<string, int, Type>)(
				(ListBoxItem)_listBoxPriority.SelectedItem).Content;
			_textBoxPriority.Text = selectionPriority.ToString();
		}

		/// <summary>
		/// To handle the color settings.
		/// </summary>
		/// <param name="sender">The object to trigger the event.</param>
		/// <param name="e">The event.</param>
		/// <param name="settings">The setting target instance.</param>
		/// <param name="colorIndex">The index.</param>
		private void HandleColor(object sender, RoutedEventArgs e, Settings settings, int colorIndex)
		{
			var dialog = new ColorDialog();

			if (!(sender is Button button) || !(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			typeof(Settings).GetProperty($"Color{colorIndex}")!.SetValue(settings, dialog.SelectedColor);
			button.Background = new SolidColorBrush(dialog.SelectedColor.ToWColor());
		}


		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;

			Close();
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;

			Close();
		}

		private void CheckBoxAskWhileQuitting_Click(object sender, RoutedEventArgs e) =>
			_checkBoxAskWhileQuitting.IsChecked = Settings.AskWhileQuitting ^= true;

		private void CheckBoxSolveFromCurrent_Click(object sender, RoutedEventArgs e) =>
			_checkBoxSolveFromCurrent.IsChecked = Settings.SolveFromCurrent ^= true;

		private void CheckBoxTextFormatPlaceholdersAreZero_Click(object sender, RoutedEventArgs e) =>
			_checkBoxTextFormatPlaceholdersAreZero.IsChecked = Settings.TextFormatPlaceholdersAreZero ^= true;

		private void CheckBoxPmGridCompatible_Click(object sender, RoutedEventArgs e) =>
			_checkBoxPmGridCompatible.IsChecked = Settings.PmGridCompatible ^= true;

		private void CheckBoxSearchExtendedUniqueRectangle_Click(object sender, RoutedEventArgs e) =>
			_checkBoxSearchExtendedUniqueRectangle.IsChecked = Settings.SearchExtendedUniqueRectangles = _manualSolver.SearchExtendedUniqueRectangles ^= true;

		private void TextBoxMaxPetalsOfDeathBlossom_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!(sender is TextBox textBox) || !int.TryParse(textBox.Text, out int value))
			{
				e.Handled = true;
				return;
			}

			if (value >= 2 && value <= 9)
			{
				Settings.MaxPetalsOfDeathBlossom = _manualSolver.MaxPetalsOfDeathBlossom = value;
			}
			else
			{
				MessageBox.Show("The value is invalid.", "Info");
			}
		}

		private void CheckBoxCheckAdvancedInExocet_Click(object sender, RoutedEventArgs e) =>
			_checkBoxCheckAdvancedInExocet.IsChecked = Settings.CheckAdvancedInExocet = _manualSolver.CheckAdvancedInExocet ^= true;

		private void TextBoxGridLineWidth_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (!float.TryParse(textBox.Text, out float value))
				{
					e.Handled = true;
					return;
				}

				Settings.GridLineWidth = value;
			}
		}

		private void TextBoxBlockLineWidth_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (!float.TryParse(textBox.Text, out float value))
				{
					e.Handled = true;
					return;
				}

				Settings.BlockLineWidth = value;
			}
		}

		private void TextBoxValueScale_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (!decimal.TryParse(textBox.Text, out decimal value))
				{
					e.Handled = true;
					return;
				}

				Settings.ValueScale = value;
			}
		}

		private void TextBoxCandidateScale_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (!decimal.TryParse(textBox.Text, out decimal value))
				{
					e.Handled = true;
					return;
				}

				Settings.CandidateScale = value;
			}
		}

		private void ButtonGivenFontName_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FontDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_labelGivenFontName.FontFamily = new FontFamily(
				Settings.GivenFontName = dialog.SelectedFont.Name);
			_labelGivenFontName.Content = dialog.SelectedFont.Name;
		}

		private void ButtonModifiableFontName_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FontDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_labelModifiableFontName.FontFamily = new FontFamily(
				Settings.ModifiableFontName = dialog.SelectedFont.Name);
			_labelModifiableFontName.Content = dialog.SelectedFont.Name;
		}

		private void ButtonCandidateFontName_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FontDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_labelCandidateFontName.FontFamily = new FontFamily(
				Settings.CandidateFontName = dialog.SelectedFont.Name);
			_labelCandidateFontName.Content = dialog.SelectedFont.Name;
		}

		private void ButtonBackgroundColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonBackgroundColor.Background = new SolidColorBrush(
				(Settings.BackgroundColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonGivenColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonGivenColor.Background = new SolidColorBrush(
				(Settings.GivenColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonModifiableColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonModifiableColor.Background = new SolidColorBrush(
				(Settings.ModifiableColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonCandidateColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonCandidateColor.Background = new SolidColorBrush(
				(Settings.CandidateColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonFocusColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonFocusColor.Background = new SolidColorBrush(
				(Settings.FocusedCellColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonGridLineColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonGridLineColor.Background = new SolidColorBrush(
				(Settings.GridLineColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonBlockLineColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonBlockLineColor.Background = new SolidColorBrush(
				(Settings.BlockLineColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonChainColor_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ColorDialog();
			if (!(dialog.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			_buttonChainColor.Background = new SolidColorBrush(
				(Settings.ChainColor = dialog.SelectedColor).ToWColor());
		}

		private void ButtonColor1_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 1);

		private void ButtonColor2_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 2);

		private void ButtonColor3_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 3);

		private void ButtonColor4_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 4);

		private void ButtonColor5_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 5);

		private void ButtonColor6_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 6);

		private void ButtonColor7_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 7);

		private void ButtonColor8_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 8);

		private void ButtonColor9_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 9);

		private void ButtonColor10_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 10);

		private void ButtonColor11_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 11);

		private void ButtonColor12_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 12);

		private void ButtonColor13_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 13);

		private void ButtonColor14_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 14);

		private void ButtonColor15_Click(object sender, RoutedEventArgs e) =>
			HandleColor(sender, e, Settings, 15);

		private void TextBoxMaxLength_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox && int.TryParse(textBox.Text, out int value))
			{
				if (value >= 3 && value <= 20)
				{
					_manualSolver.AicMaximumLength = Settings.AicMaximumLength = value;
				}
				else
				{
					MessageBox.Show("The value is invalid.", "Info");
				}
			}
		}

		private void CheckBoxAllowOverlappingAlses_Click(object sender, RoutedEventArgs e) =>
			_checkBoxAllowOverlappingAlses.IsChecked = _manualSolver.AllowOverlapAlses = Settings.AllowOverlapAlses ^= true;

		private void CheckBoxHighlightRegions_Click(object sender, RoutedEventArgs e) =>
			_checkBoxHighlightRegions.IsChecked = Settings.AlsHighlightRegionInsteadOfCell = _manualSolver.AlsHighlightRegionInsteadOfCell ^= true;

		private void TextBoxBowmanBingoMaxLength_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!(sender is TextBox textBox) || !int.TryParse(textBox.Text, out int value))
			{
				e.Handled = true;
				return;
			}

			if (value >= 1 && value <= 64)
			{
				_manualSolver.BowmanBingoMaximumLength = Settings.BowmanBingoMaximumLength = value;
			}
			else
			{
				MessageBox.Show("The value is invalid.", "Info");
			}
		}

		private void CheckBoxAllowAlq_Click(object sender, RoutedEventArgs e) =>
			_checkBoxAllowAlq.IsChecked = _manualSolver.CheckAlmostLockedQuadruple = Settings.CheckAlmostLockedQuadruple ^= true;

		private void CheckBoxCheckLoop_Click(object sender, RoutedEventArgs e) =>
			_checkBoxCheckLoop.IsChecked = Settings.CheckContinuousNiceLoop = _manualSolver.CheckContinuousNiceLoop ^= true;

		private void CheckBoxCheckHeadCollision_Click(object sender, RoutedEventArgs e) =>
			_checkBoxCheckHeadCollision.IsChecked = Settings.CheckHeadCollision = _manualSolver.CheckHeadCollision ^= true;

		private void CheckBoxCheckUncompletedUr_Click(object sender, RoutedEventArgs e) =>
			_checkBoxCheckUncompletedUr.IsChecked = Settings.CheckUncompletedUniquenessPatterns = _manualSolver.CheckIncompletedUniquenessPatterns ^= true;

		private void TextBoxMaxRegularWingSize_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox && int.TryParse(textBox.Text, out int value))
			{
				if (value >= 3 && value <= 5)
				{
					_manualSolver.CheckRegularWingSize = Settings.CheckRegularWingSize = value;
				}
				else
				{
					MessageBox.Show("The value is invalid.", "Info");
				}
			}
		}

		private void CheckBoxOnlyRecordShortestPathAic_Click(object sender, RoutedEventArgs e) =>
			_checkBoxOnlyRecordShortestPathAic.IsChecked = Settings.OnlySaveShortestPathAic = _manualSolver.OnlySaveShortestPathAic ^= true;

		private void CheckBoxReductDifferentPathAic_Click(object sender, RoutedEventArgs e) =>
			_checkBoxReductDifferentPathAic.IsChecked = Settings.ReductDifferentPathAic = _manualSolver.ReductDifferentPathAic ^= true;

		private void CheckBoxUseExtendedBugSearcher_Click(object sender, RoutedEventArgs e) =>
			_checkBoxUseExtendedBugSearcher.IsChecked = Settings.UseExtendedBugSearcher = _manualSolver.UseExtendedBugSearcher ^= true;

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
			_checkBoxEnableGcForcedly.IsEnabled = Settings.EnableGarbageCollectionForcedly = _manualSolver.EnableGarbageCollectionForcedly ^= true;

		private void ListBoxPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListBox listBox
				&& listBox.SelectedIndex != -1
				&& listBox.SelectedItem is ListBoxItem listBoxItem
				&& listBoxItem.Content is PrimaryElementTuple<string, int, Type> triplet)
			{
				var (_, priority, type) = triplet;
				_checkBoxIsEnabled.IsChecked = (bool)type.GetProperty("IsEnabled")!.GetValue(null)!;
				_textBoxPriority.Text = priority.ToString();
			}
		}
	}
}
