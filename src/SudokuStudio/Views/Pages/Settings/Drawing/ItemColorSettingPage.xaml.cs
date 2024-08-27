namespace SudokuStudio.Views.Pages.Settings.Drawing;

/// <summary>
/// Represents item color setting page.
/// </summary>
public sealed partial class ItemColorSettingPage : Page
{
	/// <summary>
	/// Initializes an <see cref="ItemColorSettingPage"/> instance.
	/// </summary>
	public ItemColorSettingPage() => InitializeComponent();


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

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.SudokuPaneBorderColor = value,
			_ => ((App)Application.Current).Preference.UIPreferences.SudokuPaneBorderColor_Dark = value
		};
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

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.CursorBackgroundColor = value,
			_ => ((App)Application.Current).Preference.UIPreferences.CursorBackgroundColor_Dark = value
		};
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

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.ChainColor = value,
			_ => ((App)Application.Current).Preference.UIPreferences.ChainColor_Dark = value
		};
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

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.NormalColor = value,
			_ => ((App)Application.Current).Preference.UIPreferences.NormalColor_Dark = value
		};
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

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AssignmentColor = value,
			_ => ((App)Application.Current).Preference.UIPreferences.AssignmentColor_Dark = value
		};
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

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.OverlappedAssignmentColor = value,
			_ => ((App)Application.Current).Preference.UIPreferences.OverlappedAssignmentColor_Dark = value
		};
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

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.EliminationColor = value,
			_ => ((App)Application.Current).Preference.UIPreferences.EliminationColor_Dark = value
		};
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

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.CannibalismColor = value,
			_ => ((App)Application.Current).Preference.UIPreferences.CannibalismColor_Dark = value
		};
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

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.ExofinColor = value,
			_ => ((App)Application.Current).Preference.UIPreferences.ExofinColor_Dark = value
		};
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

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.EndofinColor = value,
			_ => ((App)Application.Current).Preference.UIPreferences.EndofinColor_Dark = value
		};
	}

	/// <summary>
	/// The grouped node stroke color.
	/// </summary>
	internal Color GroupedNodeStrokeColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.GroupedNodeStrokeColor,
			_ => ((App)Application.Current).Preference.UIPreferences.GroupedNodeStrokeColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.GroupedNodeStrokeColor,
			_ => ((App)Application.Current).Preference.UIPreferences.GroupedNodeStrokeColor_Dark
		};
	}

	/// <summary>
	/// Indicates grouped node background color.
	/// </summary>
	internal Color GroupedNodeBackgroundColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.GroupedNodeBackgroundColor,
			_ => ((App)Application.Current).Preference.UIPreferences.GroupedNodeBackgroundColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.GroupedNodeBackgroundColor,
			_ => ((App)Application.Current).Preference.UIPreferences.GroupedNodeBackgroundColor_Dark
		};
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

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AuxiliaryColors = value,
			_ => ((App)Application.Current).Preference.UIPreferences.AuxiliaryColors_Dark = value
		};
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

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors = value,
			_ => ((App)Application.Current).Preference.UIPreferences.AlmostLockedSetsColors_Dark = value
		};
	}


	/// <summary>
	/// Try to set color to the specified <see cref="ColorPalette"/> instance.
	/// </summary>
	/// <param name="palette">The instance.</param>
	/// <param name="index">The index to be set.</param>
	/// <param name="newColor">The new color to be set.</param>
	private void ChangeColor(ColorPalette palette, int index, Color newColor) => palette[index] = newColor;


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

	private void GroupedNodeStrokeColorSelector_ColorChanged(object sender, Color e) => GroupedNodeStrokeColor = e;

	private void GroupedNodeBackgroundColorSelector_ColorChanged(object sender, Color e) => GroupedNodeBackgroundColor = e;

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
}
