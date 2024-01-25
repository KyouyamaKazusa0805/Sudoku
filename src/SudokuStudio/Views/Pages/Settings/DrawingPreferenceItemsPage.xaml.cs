namespace SudokuStudio.Views.Pages.Settings;

/// <summary>
/// Represents drawing preferences page.
/// </summary>
public sealed partial class DrawingPreferenceItemsPage : Page
{
	/// <summary>
	/// The default view unit.
	/// </summary>
	private readonly ViewUnitBindableSource _defaultViewUnit = new()
	{
		View = [
			new CandidateViewNode(ColorIdentifier.Normal, 8),
			new CandidateViewNode(ColorIdentifier.Normal, 17),
			new CandidateViewNode(ColorIdentifier.Exofin, 49 * 9 + 7),
			new CandidateViewNode(ColorIdentifier.Exofin, 50 * 9 + 7),
			new CandidateViewNode(ColorIdentifier.Endofin, 30 * 9 + 1),
			new CandidateViewNode(ColorIdentifier.Endofin, 31 * 9 + 1),
			new CandidateViewNode(ColorIdentifier.Auxiliary1, 8 * 9 + 3),
			new CandidateViewNode(ColorIdentifier.Auxiliary2, 17 * 9 + 6),
			new CandidateViewNode(ColorIdentifier.Auxiliary3, 26 * 9 + 0),
			new CandidateViewNode(ColorIdentifier.AlmostLockedSet1, 58 * 9 + 6),
			new CandidateViewNode(ColorIdentifier.AlmostLockedSet1, 58 * 9 + 7),
			new CandidateViewNode(ColorIdentifier.AlmostLockedSet2, 67 * 9 + 0),
			new CandidateViewNode(ColorIdentifier.AlmostLockedSet2, 67 * 9 + 1),
			new CandidateViewNode(ColorIdentifier.AlmostLockedSet3, 76 * 9 + 0),
			new CandidateViewNode(ColorIdentifier.AlmostLockedSet3, 76 * 9 + 1),
			new CandidateViewNode(ColorIdentifier.AlmostLockedSet4, 77 * 9 + 1),
			new CandidateViewNode(ColorIdentifier.AlmostLockedSet4, 77 * 9 + 4),
			new CandidateViewNode(ColorIdentifier.AlmostLockedSet5, 72 * 9 + 2),
			new CandidateViewNode(ColorIdentifier.AlmostLockedSet5, 72 * 9 + 5),
			new CandidateViewNode(ColorIdentifier.Auxiliary1, 36 * 9 + 2),
			new CandidateViewNode(ColorIdentifier.Normal, 37 * 9 + 2),
			new CandidateViewNode(ColorIdentifier.Auxiliary1, 44 * 9 + 6),
			new CandidateViewNode(ColorIdentifier.Normal, 43 * 9 + 6),
			new CandidateViewNode(ColorIdentifier.Normal, 45 * 9 + 5),
			new CandidateViewNode(ColorIdentifier.Auxiliary1, 46 * 9 + 5),
			new CandidateViewNode(ColorIdentifier.Normal, 35 * 9 + 3),
			new CandidateViewNode(ColorIdentifier.Auxiliary1, 34 * 9 + 3),
			new LinkViewNode(ColorIdentifier.Normal, new(2, CellsMap[36]), new(2, CellsMap[37]), Inference.Strong),
			new LinkViewNode(ColorIdentifier.Normal, new(6, CellsMap[44]), new(6, CellsMap[43]), Inference.Strong),
			new LinkViewNode(ColorIdentifier.Normal, new(5, CellsMap[45]), new(5, CellsMap[46]), Inference.Weak),
			new LinkViewNode(ColorIdentifier.Normal, new(3, CellsMap[35]), new(3, CellsMap[34]), Inference.Weak)
		],
		Conclusions = [new(Assignment, 1, 8), new(Assignment, 79, 0), new(Elimination, 0, 8), new(Elimination, 80, 0)]
	};


	/// <summary>
	/// Initializes a <see cref="DrawingPreferenceItemsPage"/> instance.
	/// </summary>
	public DrawingPreferenceItemsPage() => InitializeComponent();


	/// <summary>
	/// The theme description.
	/// </summary>
	internal string ThemeDescription
		=> string.Format(
			ResourceDictionary.Get("SettingsPage_CurrentlySelectedThemeIs", App.CurrentCulture),
			ResourceDictionary.Get(App.CurrentTheme switch { ApplicationTheme.Light => "SettingsPage_LightThemeFullName", _ => "SettingsPage_DarkThemeFullName" }, App.CurrentCulture),
			ResourceDictionary.Get(App.CurrentTheme switch { ApplicationTheme.Light => "SettingsPage_DarkThemeFullName", _ => "SettingsPage_LightThemeFullName" }, App.CurrentCulture)
		);

	/// <summary>
	/// The delta value color.
	/// </summary>
	internal Color DeltaValueColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.DeltaValueColor,
			_ => ((App)Application.Current).Preference.UIPreferences.DeltaValueColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.DeltaValueColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.DeltaValueColor_Dark = value
			};
			SampleSudokuGrid.DeltaCellColor = value;
		}
	}

	/// <summary>
	/// The delta pencilmark color.
	/// </summary>
	internal Color DeltaPencilmarkColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.DeltaPencilmarkColor,
			_ => ((App)Application.Current).Preference.UIPreferences.DeltaPencilmarkColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.DeltaPencilmarkColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.DeltaPencilmarkColor_Dark = value
			};
			SampleSudokuGrid.DeltaCandidateColor = value;
		}
	}

	/// <summary>
	/// Indicates the border color.
	/// </summary>
	internal Color SudokuPaneBorderColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.SudokuPaneBorderColor,
			_ => ((App)Application.Current).Preference.UIPreferences.SudokuPaneBorderColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.SudokuPaneBorderColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.SudokuPaneBorderColor_Dark = value
			};
			SampleSudokuGrid.BorderColor = value;
		}
	}

	/// <summary>
	/// Indicates cursor background color.
	/// </summary>
	internal Color CursorBackgroundColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.CursorBackgroundColor,
			_ => ((App)Application.Current).Preference.UIPreferences.CursorBackgroundColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.CursorBackgroundColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.CursorBackgroundColor_Dark = value
			};
			SampleSudokuGrid.CursorBackgroundColor = value;
		}
	}

	/// <summary>
	/// Indicates chain color.
	/// </summary>
	internal Color ChainColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.ChainColor,
			_ => ((App)Application.Current).Preference.UIPreferences.ChainColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.ChainColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.ChainColor_Dark = value
			};
			SampleSudokuGrid.LinkColor = value;
		}
	}

	/// <summary>
	/// The normal color.
	/// </summary>
	internal Color NormalColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.NormalColor,
			_ => ((App)Application.Current).Preference.UIPreferences.NormalColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.NormalColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.NormalColor_Dark = value
			};
			SampleSudokuGrid.NormalColor = value;
		}
	}

	/// <summary>
	/// The assignment color.
	/// </summary>
	internal Color AssignmentColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AssignmentColor,
			_ => ((App)Application.Current).Preference.UIPreferences.AssignmentColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AssignmentColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.AssignmentColor_Dark = value
			};
			SampleSudokuGrid.AssignmentColor = value;
		}
	}

	/// <summary>
	/// The overlapped assignment color.
	/// </summary>
	internal Color OverlappedAssignmentColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.OverlappedAssignmentColor,
			_ => ((App)Application.Current).Preference.UIPreferences.OverlappedAssignmentColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.OverlappedAssignmentColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.OverlappedAssignmentColor_Dark = value
			};
			SampleSudokuGrid.OverlappedAssignmentColor = value;
		}
	}

	/// <summary>
	/// The elimination color.
	/// </summary>
	internal Color EliminationColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.EliminationColor,
			_ => ((App)Application.Current).Preference.UIPreferences.EliminationColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.EliminationColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.EliminationColor_Dark = value
			};
			SampleSudokuGrid.EliminationColor = value;
		}
	}

	/// <summary>
	/// The cannibalism color.
	/// </summary>
	internal Color CannibalismColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.CannibalismColor,
			_ => ((App)Application.Current).Preference.UIPreferences.CannibalismColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.CannibalismColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.CannibalismColor_Dark = value
			};
			SampleSudokuGrid.CannibalismColor = value;
		}
	}

	/// <summary>
	/// The exofin color.
	/// </summary>
	internal Color ExofinColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.ExofinColor,
			_ => ((App)Application.Current).Preference.UIPreferences.ExofinColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.ExofinColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.ExofinColor_Dark = value
			};
			SampleSudokuGrid.ExofinColor = value;
		}
	}

	/// <summary>
	/// The endofin color.
	/// </summary>
	internal Color EndofinColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.EndofinColor,
			_ => ((App)Application.Current).Preference.UIPreferences.EndofinColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.EndofinColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.EndofinColor_Dark = value
			};
			SampleSudokuGrid.EndofinColor = value;
		}
	}

	/// <summary>
	/// The given color.
	/// </summary>
	internal Color GivenFontColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.GivenFontColor,
			_ => ((App)Application.Current).Preference.UIPreferences.GivenFontColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.GivenFontColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.GivenFontColor_Dark = value
			};
			SampleSudokuGrid.GivenColor = value;
		}
	}

	/// <summary>
	/// The modifiable color.
	/// </summary>
	internal Color ModifiableFontColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.ModifiableFontColor,
			_ => ((App)Application.Current).Preference.UIPreferences.ModifiableFontColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.ModifiableFontColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.ModifiableFontColor_Dark = value
			};
			SampleSudokuGrid.ModifiableColor = value;
		}
	}

	/// <summary>
	/// The pencilmark color.
	/// </summary>
	internal Color PencilmarkFontColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.PencilmarkFontColor,
			_ => ((App)Application.Current).Preference.UIPreferences.PencilmarkFontColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.PencilmarkFontColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.PencilmarkFontColor_Dark = value
			};
			SampleSudokuGrid.PencilmarkColor = value;
		}
	}

	/// <summary>
	/// The coordinate label color.
	/// </summary>
	internal Color CoordinateLabelFontColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.CoordinateLabelFontColor,
			_ => ((App)Application.Current).Preference.UIPreferences.CoordinateLabelFontColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.CoordinateLabelFontColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.CoordinateLabelFontColor_Dark = value
			};
			SampleSudokuGrid.CoordinateLabelColor = value;
		}
	}

	/// <summary>
	/// The baba grouping label color.
	/// </summary>
	internal Color BabaGroupingFontColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.BabaGroupingFontColor,
			_ => ((App)Application.Current).Preference.UIPreferences.BabaGroupingFontColor_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.BabaGroupingFontColor = value,
				_ => ((App)Application.Current).Preference.UIPreferences.BabaGroupingFontColor_Dark = value
			};
			SampleSudokuGrid.BabaGroupLabelColor = value;
		}
	}

	/// <summary>
	/// The auxiliary colors.
	/// </summary>
	internal ColorPalette AuxiliaryColors
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AuxiliaryColors,
			_ => ((App)Application.Current).Preference.UIPreferences.AuxiliaryColors_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AuxiliaryColors = value,
				_ => ((App)Application.Current).Preference.UIPreferences.AuxiliaryColors_Dark = value
			};
			SampleSudokuGrid.AuxiliaryColors = value;
		}
	}

	/// <summary>
	/// The almost locked set colors.
	/// </summary>
	internal ColorPalette AlmostLockedSetsColors
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors,
			_ => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors_Dark
		};

		set
		{
			_ = App.CurrentTheme switch
			{
				ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors = value,
				_ => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors_Dark = value
			};
			SampleSudokuGrid.AlmostLockedSetsColors = value;
		}
	}


	/// <summary>
	/// Try to set color to the specified <see cref="ColorPalette"/> instance.
	/// </summary>
	/// <param name="palette">The instance.</param>
	/// <param name="index">The index to be set.</param>
	/// <param name="newColor">The new color to be set.</param>
	private void ChangeColor(ColorPalette palette, int index, Color newColor) => palette[index] = newColor;


	private void Page_Loaded(object sender, RoutedEventArgs e) => SampleSudokuGrid.ViewUnit = _defaultViewUnit;

	private void SampleSudokuGrid_Loaded(object sender, RoutedEventArgs e)
		=> ((App)Application.Current).CoverSettingsToSudokuPaneViaApplicationTheme(SampleSudokuGrid);

	private void SampleSudokuGrid_ActualThemeChanged(FrameworkElement sender, object args)
		=> ((App)Application.Current).CoverSettingsToSudokuPaneViaApplicationTheme(SampleSudokuGrid);

	private void DeltaCellColorSelector_ColorChanged(object sender, Color e) => DeltaValueColor = e;

	private void DeltaCandidateColorSelector_ColorChanged(object sender, Color e) => DeltaPencilmarkColor = e;

	private void BorderColorSelector_ColorChanged(object sender, Color e) => SudokuPaneBorderColor = e;

	private void CursorBackgroundColorSelector_ColorChanged(object sender, Color e) => CursorBackgroundColor = e;

	private void ChainColorSelector_ColorChanged(object sender, Color e) => ChainColor = e;

	private void NormalColorSelector_ColorChanged(object sender, Color e) => NormalColor = e;

	private void AssignmentColorSelector_ColorChanged(object sender, Color e) => AssignmentColor = e;

	private void OverlappedAssignmentColorSelector_ColorChanged(object sender, Color e) => OverlappedAssignmentColor = e;

	private void EliminationColorSelector_ColorChanged(object sender, Color e) => EliminationColor = e;

	private void CannibalismColorSelector_ColorChanged(object sender, Color e) => CannibalismColor = e;

	private void ExofinColorSelector_ColorChanged(object sender, Color e) => ExofinColor = e;

	private void EndofinColorSelector_ColorChanged(object sender, Color e) => EndofinColor = e;

	private void AuxiliaryColor1Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AuxiliaryColors,
			_ => ((App)Application.Current).Preference.UIPreferences.AuxiliaryColors_Dark
		}, 0, e);

	private void AuxiliaryColor2Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AuxiliaryColors,
			_ => ((App)Application.Current).Preference.UIPreferences.AuxiliaryColors_Dark
		}, 1, e);

	private void AuxiliaryColor3Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AuxiliaryColors,
			_ => ((App)Application.Current).Preference.UIPreferences.AuxiliaryColors_Dark
		}, 2, e);

	private void AlmostLockedSetsColor1Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors,
			_ => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors_Dark
		}, 0, e);

	private void AlmostLockedSetsColor2Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors,
			_ => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors_Dark
		}, 1, e);

	private void AlmostLockedSetsColor3Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors,
			_ => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors_Dark
		}, 2, e);

	private void AlmostLockedSetsColor4Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors,
			_ => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors_Dark
		}, 3, e);

	private void AlmostLockedSetsColor5Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors,
			_ => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors_Dark
		}, 4, e);

	private void GivenFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.GivenFontName = e;

	private void GivenFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.GivenFontScale = e;

	private void GivenFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.GivenFontColor = e;

	private void ModifiableFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.ModifiableFontName = e;

	private void ModifiableFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.ModifiableFontScale = e;

	private void ModifiableFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.ModifiableFontColor = e;

	private void PencilmarkFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.PencilmarkFontName = e;

	private void PencilmarkFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.PencilmarkFontScale = e;

	private void PencilmarkFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.PencilmarkFontColor = e;

	private void CoordinateFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.CoordinateLabelFontName = e;

	private void CoordinateFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.CoordinateLabelFontScale = e;

	private void CoordinateFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.CoordinateLabelFontColor = e;

	private void BabaGroupingFontPicker_SelectedFontChanged(object sender, string e)
		=> ((App)Application.Current).Preference.UIPreferences.BabaGroupingFontName = e;

	private void BabaGroupingFontPicker_SelectedFontScaleChanged(object sender, decimal e)
		=> ((App)Application.Current).Preference.UIPreferences.BabaGroupingFontScale = e;

	private void BabaGroupingFontPicker_SelectedFontColorChanged(object sender, Color e)
		=> ((App)Application.Current).Preference.UIPreferences.BabaGroupingFontColor = e;

	private void StrongLinkDashStyleBox_DashArrayChanged(object sender, DashArray e)
		=> ((App)Application.Current).Preference.UIPreferences.StrongLinkDashStyle = e;

	private void WeakLinkDashStyleBox_DashArrayChanged(object sender, DashArray e)
		=> ((App)Application.Current).Preference.UIPreferences.WeakLinkDashStyle = e;

	private void CycleLikeCellLinkDashStyleBox_DashArrayChanged(object sender, DashArray e)
		=> ((App)Application.Current).Preference.UIPreferences.CyclingCellLinkDashStyle = e;
}
