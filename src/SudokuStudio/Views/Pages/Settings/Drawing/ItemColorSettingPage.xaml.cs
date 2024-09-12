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
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.SudokuPaneBorderColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.SudokuPaneBorderColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.SudokuPaneBorderColor = value,
			_ => Application.Current.AsApp().Preference.UIPreferences.SudokuPaneBorderColor_Dark = value
		};
	}

	/// <summary>
	/// Indicates cursor background color.
	/// </summary>
	internal Color CursorBackgroundColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.CursorBackgroundColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.CursorBackgroundColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.CursorBackgroundColor = value,
			_ => Application.Current.AsApp().Preference.UIPreferences.CursorBackgroundColor_Dark = value
		};
	}

	/// <summary>
	/// Indicates chain color.
	/// </summary>
	internal Color ChainColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.ChainColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.ChainColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.ChainColor = value,
			_ => Application.Current.AsApp().Preference.UIPreferences.ChainColor_Dark = value
		};
	}

	/// <summary>
	/// The normal color.
	/// </summary>
	internal Color NormalColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.NormalColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.NormalColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.NormalColor = value,
			_ => Application.Current.AsApp().Preference.UIPreferences.NormalColor_Dark = value
		};
	}

	/// <summary>
	/// The assignment color.
	/// </summary>
	internal Color AssignmentColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.AssignmentColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.AssignmentColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.AssignmentColor = value,
			_ => Application.Current.AsApp().Preference.UIPreferences.AssignmentColor_Dark = value
		};
	}

	/// <summary>
	/// The overlapped assignment color.
	/// </summary>
	internal Color OverlappedAssignmentColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.OverlappedAssignmentColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.OverlappedAssignmentColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.OverlappedAssignmentColor = value,
			_ => Application.Current.AsApp().Preference.UIPreferences.OverlappedAssignmentColor_Dark = value
		};
	}

	/// <summary>
	/// The elimination color.
	/// </summary>
	internal Color EliminationColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.EliminationColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.EliminationColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.EliminationColor = value,
			_ => Application.Current.AsApp().Preference.UIPreferences.EliminationColor_Dark = value
		};
	}

	/// <summary>
	/// The cannibalism color.
	/// </summary>
	internal Color CannibalismColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.CannibalismColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.CannibalismColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.CannibalismColor = value,
			_ => Application.Current.AsApp().Preference.UIPreferences.CannibalismColor_Dark = value
		};
	}

	/// <summary>
	/// The exofin color.
	/// </summary>
	internal Color ExofinColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.ExofinColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.ExofinColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.ExofinColor = value,
			_ => Application.Current.AsApp().Preference.UIPreferences.ExofinColor_Dark = value
		};
	}

	/// <summary>
	/// The endofin color.
	/// </summary>
	internal Color EndofinColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.EndofinColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.EndofinColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.EndofinColor = value,
			_ => Application.Current.AsApp().Preference.UIPreferences.EndofinColor_Dark = value
		};
	}

	/// <summary>
	/// The grouped node stroke color.
	/// </summary>
	internal Color GroupedNodeStrokeColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.GroupedNodeStrokeColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.GroupedNodeStrokeColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.GroupedNodeStrokeColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.GroupedNodeStrokeColor_Dark
		};
	}

	/// <summary>
	/// Indicates grouped node background color.
	/// </summary>
	internal Color GroupedNodeBackgroundColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.GroupedNodeBackgroundColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.GroupedNodeBackgroundColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.GroupedNodeBackgroundColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.GroupedNodeBackgroundColor_Dark
		};
	}

	/// <summary>
	/// Indicates active cell color used in pattern-based generator page.
	/// </summary>
	internal Color ActiveCellColor
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.ActiveCellColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.ActiveCellColor_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.ActiveCellColor,
			_ => Application.Current.AsApp().Preference.UIPreferences.ActiveCellColor_Dark
		};
	}

	/// <summary>
	/// The auxiliary colors.
	/// </summary>
	internal ColorPalette AuxiliaryColors
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.AuxiliaryColors,
			_ => Application.Current.AsApp().Preference.UIPreferences.AuxiliaryColors_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.AuxiliaryColors = value,
			_ => Application.Current.AsApp().Preference.UIPreferences.AuxiliaryColors_Dark = value
		};
	}

	/// <summary>
	/// The almost locked set colors.
	/// </summary>
	internal ColorPalette AlmostLockedSetsColors
	{
		get => App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.AlmostLockedSetsColors,
			_ => Application.Current.AsApp().Preference.UIPreferences.AlmostLockedSetsColors_Dark
		};

		set => _ = App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.AlmostLockedSetsColors = value,
			_ => Application.Current.AsApp().Preference.UIPreferences.AlmostLockedSetsColors_Dark = value
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
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.AuxiliaryColors,
			_ => Application.Current.AsApp().Preference.UIPreferences.AuxiliaryColors_Dark
		}, 0, e);

	private void AuxiliaryColor2Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.AuxiliaryColors,
			_ => Application.Current.AsApp().Preference.UIPreferences.AuxiliaryColors_Dark
		}, 1, e);

	private void AuxiliaryColor3Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.AuxiliaryColors,
			_ => Application.Current.AsApp().Preference.UIPreferences.AuxiliaryColors_Dark
		}, 2, e);

	private void AlmostLockedSetsColor1Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.AlmostLockedSetsColors,
			_ => Application.Current.AsApp().Preference.UIPreferences.AlmostLockedSetsColors_Dark
		}, 0, e);

	private void AlmostLockedSetsColor2Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.AlmostLockedSetsColors,
			_ => Application.Current.AsApp().Preference.UIPreferences.AlmostLockedSetsColors_Dark
		}, 1, e);

	private void AlmostLockedSetsColor3Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.AlmostLockedSetsColors,
			_ => Application.Current.AsApp().Preference.UIPreferences.AlmostLockedSetsColors_Dark
		}, 2, e);

	private void AlmostLockedSetsColor4Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.AlmostLockedSetsColors,
			_ => Application.Current.AsApp().Preference.UIPreferences.AlmostLockedSetsColors_Dark
		}, 3, e);

	private void AlmostLockedSetsColor5Selector_ColorChanged(object sender, Color e)
		=> ChangeColor(App.CurrentTheme switch
		{
			ApplicationTheme.Light => Application.Current.AsApp().Preference.UIPreferences.AlmostLockedSetsColors,
			_ => Application.Current.AsApp().Preference.UIPreferences.AlmostLockedSetsColors_Dark
		}, 4, e);

	private void ActiveCellColorSelector_ColorChanged(object sender, Color e) => ActiveCellColor = e;
}
