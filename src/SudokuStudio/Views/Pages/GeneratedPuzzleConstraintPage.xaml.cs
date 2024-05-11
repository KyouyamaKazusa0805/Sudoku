namespace SudokuStudio.Views.Pages;

using ConstraintControlCreator = Func<GeneratedPuzzleConstraintPage, Constraint, Control?>;

/// <summary>
/// Represents generated puzzle constraint page.
/// </summary>
public sealed partial class GeneratedPuzzleConstraintPage : Page
{
	/// <summary>
	/// Indicates a dictionary type that can create controls, distincted by its real type.
	/// </summary>
	private static readonly Dictionary<Type, ConstraintControlCreator> ControlCreatorFactory = new(EqualityComparing.Create<Type>(static (a, b) => a == b, static v => v.GetHashCode()))
	{
		{ typeof(BottleneckStepRatingConstraint), static (@this, s) => @this.Create_BottleneckStepRating((BottleneckStepRatingConstraint) s) },
		{ typeof(BottleneckTechniqueConstraint), static (@this, s) => @this.Create_BottleneckTechnique((BottleneckTechniqueConstraint) s) },
		{ typeof(ConclusionConstraint), static (@this, s) => @this.Create_Conclusion((ConclusionConstraint)s) },
		{ typeof(CountBetweenConstraint), static (@this, s) => @this.Create_CountBetween((CountBetweenConstraint)s) },
		{ typeof(DiamondConstraint), static (@this, s) => @this.Create_PearlOrDiamond((DiamondConstraint)s) },
		{ typeof(DifficultyLevelConstraint), static (@this, s) => @this.Create_DifficultyLevel((DifficultyLevelConstraint)s) },
		{ typeof(EliminationCountConstraint), static (@this, s) => @this.Create_EliminationCount((EliminationCountConstraint)s) },
		{ typeof(IttoryuConstraint), static (@this, s) => @this.Create_Ittoryu((IttoryuConstraint)s) },
		{ typeof(IttoryuLengthConstraint), static (@this, s) => @this.Create_IttoryuLength((IttoryuLengthConstraint)s) },
		{ typeof(LastingConstraint), static (@this, s) => @this.Create_Lasting((LastingConstraint)s) },
		{ typeof(MinimalConstraint), static (@this, s) => @this.Create_Minimal((MinimalConstraint)s) },
		{ typeof(PearlConstraint), static (@this, s) => @this.Create_PearlOrDiamond((PearlConstraint)s) },
		{ typeof(PrimarySingleConstraint), static (@this, s) => @this.Create_PrimarySingle((PrimarySingleConstraint)s) },
		{ typeof(SymmetryConstraint), static (@this, s) => @this.Create_Symmetry((SymmetryConstraint)s) },
		{ typeof(TechniqueConstraint), static (@this, s) => @this.Create_Technique((TechniqueConstraint)s) },
		{ typeof(TechniqueCountConstraint), static (@this, s) => @this.Create_TechniqueCount((TechniqueCountConstraint)s) },
		{ typeof(TechniqueSetConstraint), static (@this, s) => @this.Create_TechniqueSet((TechniqueSetConstraint)s) }
	};


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
		if (ControlCreatorFactory[constraint.GetType()](this, constraint) is { } control)
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
				IsChecked = constraint.IsNegated
			};
			GridLayout.SetColumn(negatingButton, 2);
			grid.Children.Add(negatingButton);

			RoutedEventHandler
				setNegated = (_, _) => constraint.IsNegated = true,
				unsetNegated = (_, _) => constraint.IsNegated = false;
			Action<ToggleButton>
				disableControl = static negatingButton => negatingButton.IsEnabled = false,
				setHandlers = button => { button.Checked += setNegated; button.Unchecked += unsetNegated; };

			(constraint.GetMetadata()?.AllowsNegation ?? false ? setHandlers : disableControl)(negatingButton);

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

	private partial SettingsCard? Create_BottleneckStepRating(BottleneckStepRatingConstraint constraint);
	private partial SettingsExpander? Create_BottleneckTechnique(BottleneckTechniqueConstraint constraint);
	private partial SettingsCard? Create_DifficultyLevel(DifficultyLevelConstraint constraint);
	private partial SettingsCard? Create_Symmetry(SymmetryConstraint constraint);
	private partial SettingsCard? Create_Conclusion(ConclusionConstraint constraint);
	private partial SettingsCard? Create_Lasting(LastingConstraint constraint);
	private partial SettingsCard? Create_Minimal(MinimalConstraint constraint);
	private partial SettingsCard? Create_PearlOrDiamond<TConstraint>(TConstraint constraint) where TConstraint : PearlOrDiamondConstraint;
	private partial SettingsCard? Create_CountBetween(CountBetweenConstraint constraint);
	private partial SettingsCard? Create_Ittoryu(IttoryuConstraint constraint);
	private partial SettingsCard? Create_IttoryuLength(IttoryuLengthConstraint constraint);
	private partial SettingsExpander? Create_Technique(TechniqueConstraint constraint);
	private partial SettingsExpander? Create_TechniqueCount(TechniqueCountConstraint constraint);
	private partial SettingsExpander? Create_TechniqueSet(TechniqueSetConstraint constraint);
	private partial SettingsExpander? Create_EliminationCount(EliminationCountConstraint constraint);
	private partial SettingsCard? Create_PrimarySingle(PrimarySingleConstraint constraint);


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
