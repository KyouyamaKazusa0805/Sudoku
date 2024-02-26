using Sudoku.Strategying;
using Sudoku.Strategying.Constraints;

namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents generated puzzle constraint page.
/// </summary>
public sealed partial class GeneratedPuzzleConstraintPage : Page
{
	/// <summary>
	/// Indicates the internal controls.
	/// </summary>
	private readonly ObservableCollection<UIElement> _controls = [];


	/// <summary>
	/// Initializes a <see cref="GeneratedPuzzleConstraintPage"/> instance.
	/// </summary>
	public GeneratedPuzzleConstraintPage()
	{
		InitializeComponent();
		CreateControlsViaProperties();
	}


	/// <summary>
	/// Create controls via properties.
	/// </summary>
	private void CreateControlsViaProperties()
	{
		foreach (var constraint in ((App)Application.Current).Preference.ConstraintPreferences.Constraints)
		{
			switch (constraint)
			{
				case DifficultyLevelConstraint instance when Create_DifficultyLevel(instance) is { } control:
				{
					_controls.Add(control);
					break;
				}
			}
		}
	}

	private SettingsCard? Create_DifficultyLevel(DifficultyLevelConstraint constraint)
	{
		if (constraint is not { DifficultyLevel: var difficultyLevel, Operator: var @operator })
		{
			return null;
		}

		//
		// operator selection
		//
		var operatorControl = new ComboBox
		{
			PlaceholderText = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_ChooseComparisonOperator"),
			Items =
			{
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_ComparisonOperator_Equality"),
					Tag = ComparisonOperator.Equality
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_ComparisonOperator_Inequality"),
					Tag = ComparisonOperator.Inequality
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_ComparisonOperator_GreaterThan"),
					Tag = ComparisonOperator.GreaterThan
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_ComparisonOperator_GreaterThanOrEqual"),
					Tag = ComparisonOperator.GreaterThanOrEqual
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_ComparisonOperator_LessThan"),
					Tag = ComparisonOperator.LessThan
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_ComparisonOperator_LessThanOrEqual"),
					Tag = ComparisonOperator.LessThanOrEqual
				}
			}
		};
		comboxItemBindCore(operatorControl, @operator, value => constraint.Operator = value);

		//
		// difficulty level selection
		//
		var difficultyLevelControl = new ComboBox
		{
			PlaceholderText = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_ChooseDifficultyLevel"),
			Items =
			{
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_DifficultyLevel_Easy"),
					Tag = DifficultyLevel.Easy
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_DifficultyLevel_Moderate"),
					Tag = DifficultyLevel.Moderate
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_DifficultyLevel_Hard"),
					Tag = DifficultyLevel.Hard
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_DifficultyLevel_Fiendish"),
					Tag = DifficultyLevel.Fiendish
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_DifficultyLevel_Nightmare"),
					Tag = DifficultyLevel.Nightmare
				}
			}
		};
		comboxItemBindCore(difficultyLevelControl, difficultyLevel, value => constraint.DifficultyLevel = value);

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_DifficultyLevel"),
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = 3,
				Children = { operatorControl, difficultyLevelControl }
			},
			Tag = constraint
		};


		void comboxItemBindCore<TEnum>(ComboBox control, TEnum comparisonValue, Action<TEnum> constraintCallback)
			where TEnum : unmanaged, Enum
		{
			var selectedIndex = 0;
			foreach (var element in control.Items)
			{
				if (element is not ComboBoxItem { Tag: TEnum op })
				{
					selectedIndex++;
					continue;
				}

				var opRawValue = Unsafe.As<TEnum, int>(ref op);
				var comparisonValueRawValue = Unsafe.As<TEnum, int>(ref comparisonValue);
				if (opRawValue != comparisonValueRawValue)
				{
					selectedIndex++;
					continue;
				}

				break;
			}
			control.SelectedIndex = selectedIndex;
			control.SelectionChanged += (_, _) =>
			{
				if (control.SelectedIndex is var index and not -1
					&& control.Items[index] is ComboBoxItem { Tag: TEnum value })
				{
					constraintCallback(value);
				}
			};
		}
	}
}
