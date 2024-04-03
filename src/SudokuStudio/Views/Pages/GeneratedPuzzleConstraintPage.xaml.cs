namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents generated puzzle constraint page.
/// </summary>
public sealed partial class GeneratedPuzzleConstraintPage : Page
{
	/// <summary>
	/// Indicates the default spacing.
	/// </summary>
	private static readonly double DefaultSpacing = 3;

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
	[SuppressMessage("Style", "IDE0039:Use local function", Justification = "<Pending>")]
	private void AddControl(Constraint constraint, bool createNew)
	{
		(
			constraint switch
			{
				BottleneckStepRatingConstraint instance => () => callback(Create_BottleneckStepRating, instance),
				BottleneckTechniqueConstraint instance => () => callback(Create_BottleneckTechnique, instance),
				ConclusionConstraint instance => () => callback(Create_Conclusion, instance),
				CountBetweenConstraint instance => () => callback(Create_CountBetween, instance),
				DiamondConstraint instance => () => callback(Create_PearlOrDiamond, instance),
				DifficultyLevelConstraint instance => () => callback(Create_DifficultyLevel, instance),
				EliminationCountConstraint instance => () => callback(Create_EliminationCount, instance),
				IttoryuConstraint instance => () => callback(Create_Ittoryu, instance),
				IttoryuLengthConstraint instance => () => callback(Create_IttoryuLength, instance),
				MinimalConstraint instance => () => callback(Create_Minimal, instance),
				PearlConstraint instance => () => callback(Create_PearlOrDiamond, instance),
				PrimarySingleConstraint instance => () => callback(Create_PrimarySingle, instance),
				SymmetryConstraint instance => () => callback(Create_Symmetry, instance),
				TechniqueConstraint instance => () => callback(Create_Technique, instance),
				TechniqueCountConstraint instance => () => callback(Create_TechniqueCount, instance),
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
						new() { Width = new(1, GridUnitType.Auto) },
						new() { Width = new(1, GridUnitType.Auto) }
					}
				};
				GridLayout.SetColumn(control, 0);
				grid.Children.Add(control);

				var negatingButton = new ToggleButton
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_NegatingLogic", App.CurrentCulture),
					Margin = new(6),
					VerticalAlignment = VerticalAlignment.Center,
					IsChecked = instance.IsNegated
				};
				GridLayout.SetColumn(negatingButton, 2);
				grid.Children.Add(negatingButton);

				RoutedEventHandler setNegated = (_, _) => instance.IsNegated = true;
				RoutedEventHandler unsetNegated = (_, _) => instance.IsNegated = false;
				var disableControl = static void (ToggleButton negatingButton) => negatingButton.IsEnabled = false;
				var setHandlers = (ToggleButton negatingButton) =>
				{
					negatingButton.Checked += setNegated;
					negatingButton.Unchecked += unsetNegated;
				};
				(instance.GetMetadata()?.AllowsNegation ?? false ? setHandlers : disableControl)(negatingButton);

				var deleteButton = new Button
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_Delete", App.CurrentCulture),
					Foreground = new SolidColorBrush(Colors.White),
					Background = new SolidColorBrush(Colors.Red),
					Margin = new(6),
					VerticalAlignment = VerticalAlignment.Center
				};
				deleteButton.Click += (_, _) =>
				{
					_controls.Remove(grid);
					ConstraintsEntry.Remove((Constraint)control.Tag!);
					negatingButton.Checked -= setNegated;
					negatingButton.Unchecked -= unsetNegated;
				};
				GridLayout.SetColumn(deleteButton, 3);
				grid.Children.Add(deleteButton);

				_controls.Add(grid);
				if (createNew)
				{
					ConstraintsEntry.Add(constraint);
				}
			}
		}
	}

	private SettingsCard? Create_BottleneckStepRating(BottleneckStepRatingConstraint constraint)
	{
		if (constraint is not { Minimum: var min, Maximum: var max, BetweenRule: var rule })
		{
			return null;
		}

		var scale = ((App)Application.Current).Preference.TechniqueInfoPreferences.RatingScale;
		var scaleInteger = AnalyzeConversion.GetScaleUnit(scale);
		var r = Application.Current.Resources;
		var maximum = (double)((int)r["MaximumRatingValue"]! * (double)r["MaximumRatingScaleValue"]!);

		//
		// Rating control
		//
		var ratingMinControl = new NumberBox { Width = 200, Minimum = 0, Maximum = maximum, Value = (double)min };
		var ratingMaxControl = new NumberBox { Width = 200, Minimum = 0, Maximum = maximum, Value = (double)max };
		ratingMinControl.ValueChanged += (_, _) =>
		{
			constraint.Minimum = f(ratingMinControl.Value, scaleInteger);
			ratingMaxControl.Minimum = ratingMinControl.Value;
		};
		ratingMaxControl.ValueChanged += (_, _) =>
		{
			constraint.Maximum = f(ratingMaxControl.Value, scaleInteger);
			ratingMinControl.Maximum = ratingMaxControl.Value;
		};

		//
		// Text blocks
		//
		var textBlock1 = new TextBlock
		{
			Text = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_BottleneckStepConstraintPart1", App.CurrentCulture),
			VerticalAlignment = VerticalAlignment.Center
		};
		var textBlock2 = new TextBlock
		{
			Text = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_BottleneckStepConstraintPart2", App.CurrentCulture),
			VerticalAlignment = VerticalAlignment.Center
		};

		//
		// Between rule
		//
		var betweenRuleControl = BetweenRuleControl(constraint, rule);

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_BottleneckRating", App.CurrentCulture),
			Margin = DefaultMargin,
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = DefaultSpacing,
				Children = { textBlock1, ratingMinControl, textBlock2, ratingMaxControl, betweenRuleControl }
			},
			Tag = constraint
		};


		static decimal f(double value, int scaleInteger) => (decimal)(scaleInteger == -1 ? value : Round(value, scaleInteger));
	}

	private SettingsExpander? Create_BottleneckTechnique(BottleneckTechniqueConstraint constraint)
	{
		if (constraint is not { Techniques: var techniques })
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
			Text = techniques.GetTechniqueString()
		};

		//
		// technique view
		//
		var techniqueControl = new TechniqueView
		{
			SelectionMode = TechniqueViewSelectionMode.Multiple,
			SelectedTechniques = techniques,
			Margin = new(-56, 0, 0, 0)
		};
		techniqueControl.SelectedTechniquesChanged += (_, e) => displayerControl.Text = (constraint.Techniques = e.TechniqueSet).GetTechniqueString();

		// Fixes #558: 'SettingsExpander' always requires 'SettingsCard' as the children control
		// for 'SettingsExpander.Items' property. See https://github.com/SunnieShine/Sudoku/issues/558
		// Reference link: https://github.com/CommunityToolkit/Windows/issues/302#issuecomment-1857686711
		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_BottleneckTechnique", App.CurrentCulture),
			Margin = DefaultMargin,
			Items = { new SettingsCard { Content = techniqueControl, ContentAlignment = ContentAlignment.Left } },
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = DefaultSpacing,
				Children = { displayerControl }
			},
			Tag = constraint
		};
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
			PlaceholderText = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_ChooseDifficultyLevel", App.CurrentCulture),
			Items =
			{
				new ComboBoxItem { Content = DifficultyLevel.Easy.GetName(App.CurrentCulture), Tag = DifficultyLevel.Easy },
				new ComboBoxItem { Content = DifficultyLevel.Moderate.GetName(App.CurrentCulture), Tag = DifficultyLevel.Moderate },
				new ComboBoxItem { Content = DifficultyLevel.Hard.GetName(App.CurrentCulture), Tag = DifficultyLevel.Hard },
				new ComboBoxItem { Content = DifficultyLevel.Fiendish.GetName(App.CurrentCulture), Tag = DifficultyLevel.Fiendish },
				new ComboBoxItem { Content = DifficultyLevel.Nightmare.GetName(App.CurrentCulture), Tag = DifficultyLevel.Nightmare }
			}
		};
		EnumBinder<ComboBox, ComboBoxItem, DifficultyLevel>(difficultyLevelControl, difficultyLevel, value => constraint.DifficultyLevel = value);

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_DifficultyLevel", App.CurrentCulture),
			Margin = DefaultMargin,
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = DefaultSpacing,
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
			Style = (Style)Application.Current.Resources["ButtonSegmentedStyle"]!,
			Items =
			{
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_Central", App.CurrentCulture), Tag = SymmetricType.Central },
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_Diagonal", App.CurrentCulture), Tag = SymmetricType.Diagonal },
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_Diagonal", App.CurrentCulture), Tag = SymmetricType.AntiDiagonal },
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_YAxis", App.CurrentCulture), Tag = SymmetricType.YAxis },
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_XAxis", App.CurrentCulture), Tag = SymmetricType.XAxis },
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_AxisBoth", App.CurrentCulture), Tag = SymmetricType.AxisBoth },
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_DiagonalBoth", App.CurrentCulture), Tag = SymmetricType.DiagonalBoth },
				new SegmentedItem { Content = ResourceDictionary.Get("SymmetricType_All", App.CurrentCulture), Tag = SymmetricType.All }
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

			// Special case: If a user has unselected all symmetric types, we should define as an invalid value.
			if (constraint.SymmetricTypes == 0)
			{
				constraint.SymmetricTypes = SymmetryConstraint.InvalidSymmetricType;
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
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_Symmetry", App.CurrentCulture),
			Margin = DefaultMargin,
			Content = symmetryControl,
			Tag = constraint
		};
	}

	private SettingsCard? Create_Conclusion(ConclusionConstraint constraint)
	{
		if (constraint is not { Conclusion: var conclusion, ShouldAppear: var shouldAppear })
		{
			return null;
		}

		//
		// conclusion type selector
		//
		var conclusionTypeControl = new ComboBox
		{
			Items =
			{
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_ComparisonOperator_Equality", App.CurrentCulture),
					Tag = Assignment
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_ComparisonOperator_Inequality", App.CurrentCulture),
					Tag = Elimination
				}
			},
			VerticalAlignment = VerticalAlignment.Center
		};
		EnumBinder<ComboBox, ComboBoxItem, ConclusionType>(
			conclusionTypeControl,
			constraint.Conclusion.ConclusionType,
			value => constraint.Conclusion = new(value, constraint.Conclusion.Candidate)
		);

		var candidatePicker = new CandidatePicker { SelectedCandidate = constraint.Conclusion.Candidate };
		candidatePicker.SelectedCandidateChanged += selectedCandidateCallback;

		//
		// appear label
		//
		var appearLabelControl = new TextBlock
		{
			Text = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_ShouldAppearLabel", App.CurrentCulture),
			VerticalAlignment = VerticalAlignment.Center
		};

		//
		// appear control
		//
		var appearControl = new ToggleSwitch { IsOn = shouldAppear };
		appearControl.RegisterPropertyChangedCallback(ToggleSwitch.IsOnProperty, appearControlCallback);

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_Conclusion", App.CurrentCulture),
			Margin = DefaultMargin,
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = DefaultSpacing,
				Children = { conclusionTypeControl, candidatePicker, appearLabelControl, appearControl }
			},
			Tag = constraint
		};


		void selectedCandidateCallback(CandidatePicker _, CandidatePickerSelectedCandidateChangedEventArgs __)
			=> constraint.Conclusion = new(constraint.Conclusion.ConclusionType, candidatePicker.SelectedCandidate);

		void appearControlCallback(DependencyObject d, DependencyProperty _) => constraint.ShouldAppear = ((ToggleSwitch)d).IsOn;
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
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_Minimal", App.CurrentCulture),
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
			Header = ResourceDictionary.Get($"GeneratedPuzzleConstraintPage_{(checkPearl ? "Pearl" : "Diamond")}", App.CurrentCulture),
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
			rangeSetter();
		};
		maximumControl.ValueChanged += (_, _) =>
		{
			minimumControl.Maximum = maximumControl.Value - 1;
			if (maximumControl.Value <= minimumControl.Value)
			{
				minimumControl.Value--;
			}
			rangeSetter();
		};

		//
		// cell-state selector
		//
		var cellStateControl = new ComboBox
		{
			PlaceholderText = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_ChooseCellState", App.CurrentCulture),
			Items =
			{
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_GivensCount", App.CurrentCulture),
					Tag = CellState.Given
				},
#if false
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_ModifiablesCount", App.CurrentCulture),
					Tag = CellState.Modifiable
				},
#endif
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_EmptiesCount", App.CurrentCulture),
					Tag = CellState.Empty
				}
			}
		};
		EnumBinder<ComboBox, ComboBoxItem, CellState>(
			cellStateControl,
			cellState,
			value =>
			{
				constraint.CellState = value;

				// Sync for minimum and maximum value for integer boxes.
				if (value is CellState.Given or CellState.Empty)
				{
					((minimumControl.Minimum, minimumControl.Maximum), (maximumControl.Minimum, maximumControl.Maximum)) = value switch
					{
						CellState.Given => ((17, 80), (18, 81)),
						CellState.Empty => ((1, 63), (2, 64))
					};
				}
			}
		);

		//
		// "and" text block
		//
		var andTextBlockControl = new TextBlock
		{
			Text = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_AndTextBlock", App.CurrentCulture),
			VerticalAlignment = VerticalAlignment.Center
		};

		//
		// between rule selector
		//
		var betweenRuleControl = BetweenRuleControl(constraint, rule);

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_CountBetween", App.CurrentCulture),
			Margin = DefaultMargin,
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = DefaultSpacing,
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


		void rangeSetter() => constraint.Range = minimumControl.Value..maximumControl.Value;
	}

	private SettingsCard? Create_Ittoryu(IttoryuConstraint constraint)
	{
		if (constraint is not { LimitedSingle: var limitedSingle, Operator: var @operator, Rounds: var rounds })
		{
			return null;
		}

		//
		// highest technique displayer
		//
		var highestTechniqueControl = new TextBlock
		{
			Text = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_HighestSingleTechnique", App.CurrentCulture),
			VerticalAlignment = VerticalAlignment.Center
		};

		//
		// limited single
		//
		var singleControl = new Segmented
		{
			Style = (Style)Application.Current.Resources["ButtonSegmentedStyle"]!,
			Items =
			{
				new SegmentedItem
				{
					Content = SingleTechnique.HiddenSingleBlock.GetName(App.CurrentCulture),
					Tag = SingleTechnique.HiddenSingleBlock
				},
				new SegmentedItem
				{
					Content = SingleTechnique.HiddenSingleRow.GetName(App.CurrentCulture),
					Tag = SingleTechnique.HiddenSingleRow
				},
				new SegmentedItem
				{
					Content = SingleTechnique.HiddenSingleColumn.GetName(App.CurrentCulture),
					Tag = SingleTechnique.HiddenSingleColumn
				},
				new SegmentedItem
				{
					Content = SingleTechnique.NakedSingle.GetName(App.CurrentCulture),
					Tag = SingleTechnique.NakedSingle
				}
			}
		};
		EnumBinder<Segmented, SegmentedItem, SingleTechnique>(singleControl, limitedSingle, value => constraint.LimitedSingle = value);

		//
		// operator selector
		//
		var operatorControl = ComparisonOperatorControl(@operator, constraint);

		//
		// rounds box
		//
		var roundsControl = LimitCountControl(rounds, constraint);

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_Ittoryu", App.CurrentCulture),
			Margin = DefaultMargin,
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = DefaultSpacing,
				Children = { highestTechniqueControl, singleControl, operatorControl, roundsControl }
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
		var lengthControl = LimitCountControl(length, constraint);

		//
		// operator
		//
		var operatorControl = ComparisonOperatorControl(@operator, constraint);

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_IttoryuLength", App.CurrentCulture),
			Margin = DefaultMargin,
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = DefaultSpacing,
				Children = { operatorControl, lengthControl }
			},
			Tag = constraint
		};
	}

	private SettingsExpander? Create_Technique(TechniqueConstraint constraint)
	{
		if (constraint is not { Techniques: var techniques })
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
			Text = techniques.GetTechniqueString()
		};

		//
		// technique view
		//
		var techniqueControl = new TechniqueView
		{
			SelectionMode = TechniqueViewSelectionMode.Multiple,
			SelectedTechniques = techniques,
			Margin = new(-56, 0, 0, 0)
		};
		techniqueControl.SelectedTechniquesChanged += (_, e) => displayerControl.Text = (constraint.Techniques = e.TechniqueSet).GetTechniqueString();

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_Technique", App.CurrentCulture),
			Margin = DefaultMargin,
			Items = { new SettingsCard { Content = techniqueControl, ContentAlignment = ContentAlignment.Left } },
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = DefaultSpacing,
				Children = { displayerControl }
			},
			Tag = constraint
		};
	}

	private SettingsExpander? Create_TechniqueCount(TechniqueCountConstraint constraint)
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
			Text = $"{technique.GetName(App.CurrentCulture)}{ResourceDictionary.Get("_Token_Comma2", App.CurrentCulture)}{ResourceDictionary.Get("GeneratedPuzzleConstraintPage_AppearingTimes", App.CurrentCulture)}"
		};

		//
		// technique view
		//
		var techniqueControl = new TechniqueView
		{
			SelectionMode = TechniqueViewSelectionMode.Single,
			SelectedTechniques = [technique],
			Margin = new(-56, 0, 0, 0)
		};
		techniqueControl.CurrentSelectedTechniqueChanged += (_, e) =>
		{
			var technique = e.Technique;
			constraint.Technique = technique;
			displayerControl.Text = $"{technique.GetName(App.CurrentCulture)}{ResourceDictionary.Get("_Token_Comma2", App.CurrentCulture)}{ResourceDictionary.Get("GeneratedPuzzleConstraintPage_AppearingTimes", App.CurrentCulture)}";
		};

		//
		// appearing times
		//
		var appearingTimesControl = LimitCountControl(appearingTimes, constraint);

		//
		// comparison operator
		//
		var operatorControl = ComparisonOperatorControl(@operator, constraint);

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_TechniqueCount", App.CurrentCulture),
			Margin = DefaultMargin,
			Items = { new SettingsCard { Content = techniqueControl, ContentAlignment = ContentAlignment.Left } },
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = DefaultSpacing,
				Children = { displayerControl, operatorControl, appearingTimesControl }
			},
			Tag = constraint
		};
	}

	private SettingsExpander? Create_EliminationCount(EliminationCountConstraint constraint)
	{
		if (constraint is not { LimitCount: var limitCount, Operator: var @operator, Technique: var technique })
		{
			return null;
		}

		//
		// Operator control
		//
		var operatorControl = ComparisonOperatorControl(@operator, constraint);

		//
		// Number control
		//
		var limitCountControl = LimitCountControl(limitCount, constraint);

		//
		// chosen techniques displayer
		//
		var displayerControl = new TextBlock
		{
			MaxWidth = 400,
			TextWrapping = TextWrapping.WrapWholeWords,
			VerticalAlignment = VerticalAlignment.Center,
			Text = $"{technique.GetName(App.CurrentCulture)}{ResourceDictionary.Get("_Token_Comma2", App.CurrentCulture)}"
		};

		//
		// technique view
		//
		var techniqueControl = new TechniqueView
		{
			SelectionMode = TechniqueViewSelectionMode.Single,
			ShowMode = TechniqueViewShowMode.OnlyEliminations,
			Margin = new(-56, 0, 0, 0),
			SelectedTechniques = [technique]
		};
		techniqueControl.CurrentSelectedTechniqueChanged += (_, e) =>
		{
			var technique = e.Technique;
			constraint.Technique = technique;
			displayerControl.Text = $"{technique.GetName(App.CurrentCulture)}{ResourceDictionary.Get("_Token_Comma2", App.CurrentCulture)}";
		};

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_EliminationCount", App.CurrentCulture),
			Margin = DefaultMargin,
			Items = { new SettingsCard { Content = techniqueControl, ContentAlignment = ContentAlignment.Left } },
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = DefaultSpacing,
				Children = { displayerControl, operatorControl, limitCountControl }
			},
			Tag = constraint
		};
	}

	private SettingsCard? Create_PrimarySingle(PrimarySingleConstraint constraint)
	{
		if (constraint is not { Primary: var prefer, AllowsHiddenSingleInLines: var allowsForLine })
		{
			return null;
		}

		//
		// prefer label
		//
		var preferredLabel1 = new TextBlock
		{
			Text = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_PrimaryTechniqueLabelPart1", App.CurrentCulture),
			VerticalAlignment = VerticalAlignment.Center
		};
		var preferredLabel2 = new TextBlock
		{
			Text = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_PrimaryTechniqueLabelPart2", App.CurrentCulture),
			VerticalAlignment = VerticalAlignment.Center
		};

		//
		// technique selector
		//
		var techniqueSelectorControl = new Segmented
		{
			Style = (Style)Application.Current.Resources["ButtonSegmentedStyle"]!,
			Items =
			{
				new SegmentedItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_PrimaryFullHouse", App.CurrentCulture),
					Tag = SingleTechnique.FullHouse
				},
				new SegmentedItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_PrimaryHiddenSingle", App.CurrentCulture),
					Tag = SingleTechnique.HiddenSingle
				},
				new SegmentedItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_PrimaryNakedSingle", App.CurrentCulture),
					Tag = SingleTechnique.NakedSingle
				}
			}
		};
		EnumBinder<Segmented, SegmentedItem, SingleTechnique>(techniqueSelectorControl, constraint.Primary, value => constraint.Primary = value);

		//
		// allow line control
		//
		var allowsLineControl = new ToggleSwitch { IsOn = constraint.AllowsHiddenSingleInLines };
		allowsLineControl.RegisterPropertyChangedCallback(
			ToggleSwitch.IsOnProperty,
			(d, _) => constraint.AllowsHiddenSingleInLines = ((ToggleSwitch)d).IsOn
		);
		techniqueSelectorControl.SelectionChanged += (sender, _) => allowsLineControl.IsEnabled = sender is Segmented
		{
			SelectedItem: SegmentedItem { Tag: SingleTechnique.HiddenSingle }
		};

		return new()
		{
			Header = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_PrimarySingle", App.CurrentCulture),
			Margin = DefaultMargin,
			Content = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = DefaultSpacing,
				Children = { preferredLabel1, techniqueSelectorControl, preferredLabel2, allowsLineControl }
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
			PlaceholderText = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_ChooseComparisonOperator", App.CurrentCulture),
			VerticalAlignment = VerticalAlignment.Center,
			Items =
			{
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_ComparisonOperator_Equality", App.CurrentCulture),
					Tag = ComparisonOperator.Equality
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_ComparisonOperator_Inequality", App.CurrentCulture),
					Tag = ComparisonOperator.Inequality
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_ComparisonOperator_GreaterThan", App.CurrentCulture),
					Tag = ComparisonOperator.GreaterThan
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_ComparisonOperator_GreaterThanOrEqual", App.CurrentCulture),
					Tag = ComparisonOperator.GreaterThanOrEqual
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_ComparisonOperator_LessThan", App.CurrentCulture),
					Tag = ComparisonOperator.LessThan
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("_ComparisonOperator_LessThanOrEqual", App.CurrentCulture),
					Tag = ComparisonOperator.LessThanOrEqual
				}
			}
		};
		EnumBinder<ComboBox, ComboBoxItem, ComparisonOperator>(operatorControl, @operator, value => constraint.Operator = value);

		return operatorControl;
	}

	/// <summary>
	/// Creates an <see cref="IntegerBox"/> object for limit count displaying.
	/// </summary>
	/// <typeparam name="T">The type of the constraint.</typeparam>
	/// <param name="limitCount">The limit count.</param>
	/// <param name="constraint">The constraint.</param>
	/// <returns>An <see cref="IntegerBox"/> instance.</returns>
	private static IntegerBox LimitCountControl<T>(int limitCount, T constraint) where T : Constraint, ILimitCountConstraint<int>
	{
		var limitCountControl = new IntegerBox
		{
			Width = 150,
			Minimum = T.Minimum,
			Maximum = T.Maximum,
			Value = limitCount,
			VerticalAlignment = VerticalAlignment.Center
		};
		limitCountControl.ValueChanged += (_, _) => constraint.LimitCount = limitCountControl.Value;
		return limitCountControl;
	}

	/// <summary>
	/// Creates a between rule control.
	/// </summary>
	/// <param name="constraint">Indicates the constraint.</param>
	/// <param name="rule">Indicates the rule.</param>
	/// <returns>A <see cref="ComboBox"/> result.</returns>
	private static ComboBox BetweenRuleControl<TConstraint>(TConstraint constraint, BetweenRule rule)
		where TConstraint : Constraint, IBetweenRuleConstraint
	{
		var betweenRuleControl = new ComboBox
		{
			PlaceholderText = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_ChooseBetweenRule", App.CurrentCulture),
			Items =
			{
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_BothOpen", App.CurrentCulture),
					Tag = BetweenRule.BothOpen
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_OnlyLeftOpen", App.CurrentCulture),
					Tag = BetweenRule.LeftOpen
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_OnlyRightOpen", App.CurrentCulture),
					Tag = BetweenRule.RightOpen
				},
				new ComboBoxItem
				{
					Content = ResourceDictionary.Get("GeneratedPuzzleConstraintPage_BothClosed", App.CurrentCulture),
					Tag = BetweenRule.BothClosed
				}
			}
		};

		EnumBinder<ComboBox, ComboBoxItem, BetweenRule>(betweenRuleControl, rule, value => constraint.BetweenRule = value);
		return betweenRuleControl;
	}

	/// <summary>
	/// The core method that binds a field of type <typeparamref name="TEnum"/> to a <typeparamref name="TControl"/> instance.
	/// </summary>
	/// <typeparam name="TItemControl">The type of the item control.</typeparam>
	/// <typeparam name="TControl">The type of the control.</typeparam>
	/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
	/// <param name="control">The control to be operated.</param>
	/// <param name="valueToCompare">The value to compare.</param>
	/// <param name="constraintCallback">The constraint callback method.</param>
	private static unsafe void EnumBinder<TControl, TItemControl, TEnum>(TControl control, TEnum valueToCompare, Action<TEnum> constraintCallback)
		where TControl : Selector
		where TItemControl : SelectorItem
		where TEnum : unmanaged, Enum
	{
		var selectedIndex = 0;
		foreach (var element in control.Items)
		{
			if (element is not TItemControl { Tag: TEnum enumValue })
			{
				selectedIndex++;
				continue;
			}

			switch (sizeof(TEnum))
			{
				case 1 or 2 or 4:
				{
					var opRawValue = Unsafe.As<TEnum, int>(ref enumValue);
					var comparisonValueRawValue = Unsafe.As<TEnum, int>(ref valueToCompare);
					if (opRawValue != comparisonValueRawValue)
					{
						selectedIndex++;
						continue;
					}
					break;
				}
				case 8:
				{
					var opRawValue = Unsafe.As<TEnum, long>(ref enumValue);
					var comparisonValueRawValue = Unsafe.As<TEnum, long>(ref valueToCompare);
					if (opRawValue != comparisonValueRawValue)
					{
						selectedIndex++;
						continue;
					}
					break;
				}
			}

			break;
		}
		control.SelectedIndex = selectedIndex;
		control.SelectionChanged += (_, _) =>
		{
			if (control.SelectedIndex is var index and not -1 && control.Items[index] is TItemControl { Tag: TEnum value })
			{
				constraintCallback(value);
			}
		};
	}


	private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
	{
		if (sender is MenuFlyoutItem { Tag: Constraint constraint })
		{
			AddControl(constraint.Clone(), true);
		}
	}

	private void MenuFlyout_Opening(object sender, object e)
	{
		foreach (var element in MenuFlyout.Items)
		{
			if (element is MenuFlyoutItem { Tag: Constraint constraint })
			{
				var type = constraint.GetType();
				var allowsMultiple = type.GetCustomAttribute<ConstraintOptionsAttribute>()?.AllowsMultiple ?? false;
				element.IsEnabled = allowsMultiple || !ConstraintsEntry.Exists(c => c.GetType() == type);
			}
		}
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
#pragma warning disable format
	/// <summary>
	/// Try to fetch the string representation of the techniques chosen in UI.
	/// </summary>
	/// <param name="this">The techniques chosen.</param>
	/// <returns>The string representation.</returns>
	public static string GetTechniqueString(this TechniqueSet @this)
		=> @this switch
		{
			[] => ResourceDictionary.Get("GeneratedPuzzleConstraintPage_NoTechniquesSelected", App.CurrentCulture),
			[var technique] => technique.GetName(App.CurrentCulture),
			_ => string.Join(
				ResourceDictionary.Get("_Token_Comma", App.CurrentCulture),
				[.. from technique in @this select technique.GetName(App.CurrentCulture)]
			)
		};
#pragma warning restore format
}
