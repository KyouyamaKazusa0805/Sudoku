namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents generated puzzle constraint page.
/// </summary>
public sealed partial class GeneratedPuzzleConstraintPage : Page
{
	/// <summary>
	/// Indicates the default margin.
	/// </summary>
	private static readonly Thickness DefaultMargin = new(0, 6, 0, 6);


	/// <summary>
	/// Indicates the internal controls.
	/// </summary>
	private readonly ObservableCollection<FrameworkElement> _controls = [];


	/// <summary>
	/// Initializes a <see cref="GeneratedPuzzleConstraintPage"/> instance.
	/// </summary>
	public GeneratedPuzzleConstraintPage()
	{
		InitializeComponent();
		CreateControlsViaProperties();
	}


	/// <summary>
	/// Indicates entry that visits constraints property in preference set <see cref="ConstraintPreferenceGroup.Constraints"/>.
	/// </summary>
	/// <seealso cref="ConstraintPreferenceGroup.Constraints"/>
	private static ConstraintCollection ConstraintsEntry => ((App)Application.Current).Preference.ConstraintPreferences.Constraints;


	/// <summary>
	/// Create controls via properties.
	/// </summary>
	private void CreateControlsViaProperties()
	{
		foreach (var constraint in ConstraintsEntry)
		{
			AddControl(constraint, false);
		}
	}

	/// <summary>
	/// Add a new control using the specified constraint.
	/// </summary>
	/// <param name="constraint">The constraint.</param>
	/// <param name="createNew">
	/// Indicates whether the constraint control is created, and also appended into property <see cref="ConstraintsEntry"/>.
	/// </param>
	/// <seealso cref="ConstraintsEntry"/>
	private void AddControl(Constraint constraint, bool createNew)
	{
		(
			constraint switch
			{
				DifficultyLevelConstraint instance => () => callback(Create_DifficultyLevel, instance),
				SymmetryConstraint instance => () => callback(Create_Symmetry, instance),
				CountBetweenConstraint instance => () => callback(Create_CountBetween, instance),
				TechniqueConstraint instance => () => callback(Create_Technique, instance),
				MinimalConstraint instance => () => callback(Create_Minimal, instance),
				PearlConstraint instance => () => callback(Create_PearlOrDiamond, instance),
				DiamondConstraint instance => () => callback(Create_PearlOrDiamond, instance),
				IttoryuConstraint instance => () => callback(Create_Ittoryu, instance),
				IttoryuLengthConstraint instance => () => callback(Create_IttoryuLength, instance),
				_ => default(Action)
			}
		)?.Invoke();


		void callback<TConstraint, TControl>(Func<TConstraint, TControl?> method, TConstraint instance)
			where TConstraint : Constraint
			where TControl : Control
		{
			if (method(instance) is { } control)
			{
				var grid = new GridLayout
				{
					ColumnDefinitions =
					{
						new() { Width = new(1, GridUnitType.Star) },
						new() { Width = new(20) },
						new() { Width = new(1, GridUnitType.Auto) }
					}
				};
				var deleteButton = new Button
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_Delete"),
					Foreground = new SolidColorBrush(Colors.White),
					Background = new SolidColorBrush(Colors.Red),
					Margin = new(6),
					VerticalAlignment = VerticalAlignment.Center
				};
				deleteButton.Click += (_, _) => { _controls.Remove(grid); ConstraintsEntry.Remove((Constraint)control.Tag!); };
				GridLayout.SetColumn(control, 0);
				GridLayout.SetColumn(deleteButton, 2);
				grid.Children.Add(control);
				grid.Children.Add(deleteButton);

				_controls.Add(grid);
				if (createNew)
				{
					ConstraintsEntry.Add(constraint);
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
		var operatorControl = ComparisonOperatorControl(@operator, constraint);

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
		ComboBoxBindingHandler(difficultyLevelControl, difficultyLevel, value => constraint.DifficultyLevel = value);

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_DifficultyLevel"),
			Margin = DefaultMargin,
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = 3,
				Children = { operatorControl, difficultyLevelControl }
			},
			Tag = constraint
		};
	}

	private SettingsCard? Create_Symmetry(SymmetryConstraint constraint)
	{
		// There may exist a bug that we cannot select "SymmetricType.None" because the flag value is 0, no flags can use this bit.

		if (constraint is not { SymmetricTypes: var symmetricTypes })
		{
			return null;
		}

		//
		// symmetry selection
		//
		var symmetryControl = new Segmented
		{
			SelectionMode = ListViewSelectionMode.Multiple,
			Items =
			{
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_Central"), Tag = SymmetricType.Central },
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_Diagonal"), Tag = SymmetricType.Diagonal },
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_Diagonal"), Tag = SymmetricType.AntiDiagonal },
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_YAxis"), Tag = SymmetricType.YAxis },
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_XAxis"), Tag = SymmetricType.XAxis },
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_AxisBoth"), Tag = SymmetricType.AxisBoth },
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_DiagonalBoth"), Tag = SymmetricType.DiagonalBoth },
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_All"), Tag = SymmetricType.All }
			}
		};
		symmetryControl.SelectionChanged += (_, _) =>
		{
			foreach (SegmentedItem element in symmetryControl.Items)
			{
				var type = (SymmetricType)element.Tag!;
				var possibleItemInChosenItemsList = symmetryControl.SelectedItems.FirstOrDefault(e => ReferenceEquals(e, element));
				if (possibleItemInChosenItemsList is not null)
				{
					constraint.SymmetricTypes |= type;
				}
				else
				{
					constraint.SymmetricTypes &= ~type;
				}
			}
		};
		foreach (SegmentedItem element in symmetryControl.Items)
		{
			if (symmetricTypes.HasFlag((SymmetricType)element.Tag!))
			{
				symmetryControl.SelectedItems.Add(element);
			}
		}

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_Symmetry"),
			Margin = DefaultMargin,
			Content = symmetryControl,
			Tag = constraint
		};
	}

	private SettingsCard? Create_Minimal(MinimalConstraint constraint)
	{
		if (constraint is not { ShouldBeMinimal: var value })
		{
			return null;
		}

		//
		// minimal selector
		//
		var minimalControl = new ToggleSwitch { IsOn = value };
		minimalControl.RegisterPropertyChangedCallback(ToggleSwitch.IsOnProperty, (d, _) => constraint.ShouldBeMinimal = ((ToggleSwitch)d).IsOn);

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_Minimal"),
			Margin = DefaultMargin,
			Content = minimalControl,
			Tag = constraint
		};
	}

	private SettingsCard? Create_PearlOrDiamond<TConstraint>(TConstraint constraint) where TConstraint : PearlOrDiamondConstraint
	{
		if (constraint is not { CheckPearl: var checkPearl, ShouldBePearlOrDiamond: var value })
		{
			return null;
		}

		//
		// pearl or diamond selector
		//
		var control = new ToggleSwitch { IsOn = value };
		control.RegisterPropertyChangedCallback(ToggleSwitch.IsOnProperty, (d, _) => constraint.ShouldBePearlOrDiamond = ((ToggleSwitch)d).IsOn);

		return new()
		{
			Header = ResourceDictionary.Get($"GeneratedPuzzleConstraintPage_{(checkPearl ? "Pearl" : "Diamond")}"),
			Margin = DefaultMargin,
			Content = control,
			Tag = constraint
		};
	}

	private SettingsCard? Create_CountBetween(CountBetweenConstraint constraint)
	{
		if (constraint is not
			{
				Range: { Start.Value: var min, End.Value: var max },
				CellState: var cellState,
				BetweenRule: var rule
			})
		{
			return null;
		}

		//
		// cell-state selector
		//
		var cellStateControl = new ComboBox
		{
			PlaceholderText = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_ChooseCellState"),
			Items =
			{
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_GivensCount"),
					Tag = CellState.Given
				},
#if false
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_ModifiablesCount"),
					Tag = CellState.Modifiable
				},
#endif
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_EmptiesCount"),
					Tag = CellState.Empty
				}
			}
		};
		ComboBoxBindingHandler(cellStateControl, cellState, value => constraint.CellState = value);

		//
		// minimum value box
		//
		var minimumControl = new IntegerBox { Width = 150, Minimum = 17, Maximum = 80, SmallChange = 1, LargeChange = 5, Value = min };
		var maximumControl = new IntegerBox { Width = 150, Minimum = 18, Maximum = 81, SmallChange = 1, LargeChange = 5, Value = max };
		minimumControl.ValueChanged += (_, _) =>
		{
			maximumControl.Minimum = minimumControl.Value + 1;
			if (minimumControl.Value >= maximumControl.Value)
			{
				maximumControl.Value++;
			}
			setter();
		};
		maximumControl.ValueChanged += (_, _) =>
		{
			minimumControl.Maximum = maximumControl.Value - 1;
			if (maximumControl.Value <= minimumControl.Value)
			{
				minimumControl.Value--;
			}
			setter();
		};

		//
		// "and" text block
		//
		var andTextBlockControl = new TextBlock
		{
			Text = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_AndTextBlock"),
			VerticalAlignment = VerticalAlignment.Center
		};

		//
		// between rule selector
		//
		var betweenRuleControl = new ComboBox
		{
			PlaceholderText = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_ChooseBetweenRule"),
			Items =
			{
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_BothOpen"),
					Tag = BetweenRule.BothOpen
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_OnlyLeftOpen"),
					Tag = BetweenRule.LeftOpen
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_OnlyRightOpen"),
					Tag = BetweenRule.RightOpen
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_BothClosed"),
					Tag = BetweenRule.BothClosed
				}
			}
		};
		ComboBoxBindingHandler(betweenRuleControl, rule, value => constraint.BetweenRule = value);

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_CountBetween"),
			Margin = DefaultMargin,
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = 3,
				Children =
				{
					cellStateControl,
					minimumControl,
					andTextBlockControl,
					maximumControl,
					betweenRuleControl
				}
			},
			Tag = constraint
		};


		void setter() => constraint.Range = minimumControl.Value..maximumControl.Value;
	}

	private SettingsCard? Create_Ittoryu(IttoryuConstraint constraint)
	{
		if (constraint is not { Operator: var @operator, Rounds: var rounds })
		{
			return null;
		}

		//
		// operator selector
		//
		var operatorControl = ComparisonOperatorControl(@operator, constraint);

		//
		// rounds box
		//
		var roundsControl = new IntegerBox { Width = 150, Minimum = 1, Maximum = 10, Value = rounds };
		roundsControl.ValueChanged += (_, _) => constraint.Rounds = roundsControl.Value;

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_Ittoryu"),
			Margin = DefaultMargin,
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = 3,
				Children = { operatorControl, roundsControl }
			},
			Tag = constraint
		};
	}

	private SettingsCard? Create_IttoryuLength(IttoryuLengthConstraint constraint)
	{
		if (constraint is not { Length: var length, Operator: var @operator })
		{
			return null;
		}

		//
		// length
		//
		var lengthControl = new IntegerBox { Width = 150, Minimum = 0, Maximum = 9, Value = length };
		lengthControl.ValueChanged += (_, _) => constraint.Length = length;

		//
		// operator
		//
		var operatorControl = ComparisonOperatorControl(@operator, constraint);

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_IttoryuLength"),
			Margin = DefaultMargin,
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = 3,
				Children = { operatorControl, lengthControl }
			},
			Tag = constraint
		};
	}

	private SettingsExpander? Create_Technique(TechniqueConstraint constraint)
	{
		if (constraint is not { Technique: var technique, LimitCount: var appearingTimes, Operator: var @operator })
		{
			return null;
		}

		//
		// chosen techniques displayer
		//
		var displayerControl = new TextBlock
		{
			MaxWidth = 400,
			TextWrapping = TextWrapping.WrapWholeWords,
			VerticalAlignment = VerticalAlignment.Center,
			Text = $"{technique.GetName(App.CurrentCulture)}{ResourceDictionary.Get("_Token_Comma2")}{ResourceDictionary.Get("GeneratedPuzzleConstraintPage_AppearingTimes")}"
		};

		//
		// technique view
		//
		var techniqueControl = new TechniqueView { SelectionMode = TechniqueViewSelectionMode.Single, SelectedTechniques = [technique] };
		techniqueControl.CurrentSelectedTechniqueChanged += (_, e) =>
		{
			var technique = e.Technique;
			constraint.Technique = technique;
			displayerControl.Text = $"{technique.GetName(App.CurrentCulture)}{ResourceDictionary.Get("_Token_Comma2")}{ResourceDictionary.Get("GeneratedPuzzleConstraintPage_AppearingTimes")}";
		};

		//
		// appearing times
		//
		var appearingTimesControl = new IntegerBox { Width = 150, Minimum = 1, Maximum = 20 };
		appearingTimesControl.ValueChanged += (_, _) => constraint.LimitCount = appearingTimesControl.Value;

		//
		// comparison operator
		//
		var operatorControl = ComparisonOperatorControl(@operator, constraint);

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_Technique"),
			Margin = DefaultMargin,
			Items = { techniqueControl },
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = 3,
				Children = { displayerControl, operatorControl, appearingTimesControl }
			},
			Tag = constraint
		};
	}


	/// <summary>
	/// Creates a <see cref="ComboBox"/> object for comparison operator displaying.
	/// </summary>
	/// <typeparam name="T">The type of the constraint.</typeparam>
	/// <param name="operator">The operator value.</param>
	/// <param name="constraint">The constraint.</param>
	/// <returns>A <see cref="ComboBox"/> instance.</returns>
	private static ComboBox ComparisonOperatorControl<T>(ComparisonOperator @operator, T constraint)
		where T : Constraint, IComparisonOperatorConstraint
	{
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
		ComboBoxBindingHandler(operatorControl, @operator, value => constraint.Operator = value);

		return operatorControl;
	}

	/// <summary>
	/// The core method that binds a field of type <typeparamref name="TEnum"/> to a <see cref="ComboBox"/> instance.
	/// </summary>
	/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
	/// <param name="control">The control to be operated.</param>
	/// <param name="comparisonValue">The value to compare.</param>
	/// <param name="constraintCallback">The constraint callback binder.</param>
	private static void ComboBoxBindingHandler<TEnum>(ComboBox control, TEnum comparisonValue, Action<TEnum> constraintCallback)
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


	private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
	{
		if (sender is MenuFlyoutItem { Tag: Constraint constraint })
		{
			AddControl(constraint, true);
		}
	}

	private void MenuFlyout_Opening(object sender, object e)
	{
		foreach (var element in MenuFlyout.Items)
		{
			if (element is MenuFlyoutItem { Tag: Constraint { AllowDuplicate: var allowDuplicate } constraint })
			{
				element.Visibility = !allowDuplicate && ConstraintsEntry.Exists(c => c.GetType() == constraint.GetType())
					? Visibility.Collapsed
					: Visibility.Visible;
			}
		}
	}
}
