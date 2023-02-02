namespace SudokuStudio.Configuration;

/// <summary>
/// Defines a list of UI-related preference items. Some items in this group may not be found in settings page
/// because they are controlled by UI only, not by users.
/// </summary>
public sealed class UISetting : PreferenceGroup
{
	/// <inheritdoc cref="SudokuPane.DisplayCandidates"/>
	public bool DisplayCandidates
	{
		get => Pane.DisplayCandidates;

		set => Pane.DisplayCandidates = value;
	}

	/// <inheritdoc cref="SudokuPane.DisplayCursors"/>
	public bool DisplayCursors
	{
		get => Pane.DisplayCursors;

		set => Pane.DisplayCursors = value;
	}

	/// <inheritdoc cref="SudokuPane.UseDifferentColorToDisplayDeltaDigits"/>
	public bool DistinctWithDeltaDigits
	{
		get => Pane.UseDifferentColorToDisplayDeltaDigits;

		set => Pane.UseDifferentColorToDisplayDeltaDigits = value;
	}

	/// <inheritdoc cref="SudokuPane.HighlightCandidateCircleScale"/>
	public decimal HighlightedPencilmarkBackgroundEllipseScale
	{
		get => (decimal)Pane.HighlightCandidateCircleScale;

		set => Pane.HighlightCandidateCircleScale = (double)value;
	}

	/// <inheritdoc cref="SudokuPane.HighlightBackgroundOpacity"/>
	public decimal HighlightedBackgroundOpacity
	{
		get => (decimal)Pane.HighlightBackgroundOpacity;

		set => Pane.HighlightBackgroundOpacity = (double)value;
	}

	/// <inheritdoc cref="SudokuPane.ChainStrokeThickness"/>
	public decimal ChainStrokeThickness
	{
		get => (decimal)Pane.ChainStrokeThickness;

		set => Pane.ChainStrokeThickness = (double)value;
	}

	/// <inheritdoc cref="SudokuPane.CoordinateLabelDisplayKind"/>
	public CoordinateLabelDisplayKind CoordinateLabelDisplayKind
	{
		get => Pane.CoordinateLabelDisplayKind;

		set => Pane.CoordinateLabelDisplayKind = value;
	}

	/// <inheritdoc cref="SudokuPane.CoordinateLabelDisplayMode"/>
	public CoordinateLabelDisplayMode CoordinateLabelDisplayMode
	{
		get => Pane.CoordinateLabelDisplayMode;

		set => Pane.CoordinateLabelDisplayMode = value;
	}

	/// <inheritdoc cref="SudokuPane.DeltaCellColor"/>
	public Color DeltaValueColor
	{
		get => Pane.DeltaCellColor;

		set => Pane.DeltaCellColor = value;
	}

	/// <inheritdoc cref="SudokuPane.DeltaCandidateColor"/>
	public Color DeltaPencilmarkColor
	{
		get => Pane.DeltaCandidateColor;

		set => Pane.DeltaCandidateColor = value;
	}

	/// <inheritdoc cref="SudokuPane.BorderColor"/>
	public Color SudokuPaneBorderColor
	{
		get => Pane.BorderColor;

		set => Pane.BorderColor = value;
	}

	/// <inheritdoc cref="SudokuPane.CursorBackgroundColor"/>
	public Color CursorBackgroundColor
	{
		get => Pane.CursorBackgroundColor;

		set => Pane.CursorBackgroundColor = value;
	}

	/// <inheritdoc cref="SudokuPane.LinkColor"/>
	public Color ChainColor
	{
		get => Pane.LinkColor;

		set => Pane.LinkColor = value;
	}

	/// <inheritdoc cref="SudokuPane.StrongLinkDashStyle"/>
	public DashArray StrongLinkDashStyle
	{
		get => Pane.StrongLinkDashStyle;

		set => Pane.StrongLinkDashStyle = value;
	}

	/// <inheritdoc cref="SudokuPane.WeakLinkDashStyle"/>
	public DashArray WeakLinkDashStyle
	{
		get => Pane.WeakLinkDashStyle;

		set => Pane.WeakLinkDashStyle = value;
	}

	/// <inheritdoc cref="SudokuPane.CycleLikeLinkDashStyle"/>
	public DashArray CyclingCellLinkDashStyle
	{
		get => Pane.CycleLikeLinkDashStyle;

		set => Pane.CycleLikeLinkDashStyle = value;
	}

	/// <inheritdoc cref="SudokuPane.OtherLinkDashStyle"/>
	public DashArray OtherLinkDashStyle
	{
		get => Pane.OtherLinkDashStyle;

		set => Pane.OtherLinkDashStyle = value;
	}

	/// <summary>
	/// Indicates the font data of given digits.
	/// </summary>
	public FontSerializationData GivenFontData
	{
		get => new() { FontName = Pane.ValueFont.Source, FontScale = (decimal)Pane.ValueFontScale, FontColor = Pane.GivenColor };

		set
		{
			if (GivenFontData == value)
			{
				return;
			}

			Pane.ValueFont = new(value.FontName);
			Pane.ValueFontScale = (double)value.FontScale;
			Pane.GivenColor = value.FontColor;
		}
	}

	/// <summary>
	/// Indicates the font data of modifiable digits.
	/// </summary>
	public FontSerializationData ModifiableFontData
	{
		get => new() { FontName = Pane.ValueFont.Source, FontScale = (decimal)Pane.ValueFontScale, FontColor = Pane.ModifiableColor };

		set
		{
			if (ModifiableFontData == value)
			{
				return;
			}

			Pane.ValueFont = new(value.FontName);
			Pane.ValueFontScale = (double)value.FontScale;
			Pane.ModifiableColor = value.FontColor;
		}
	}

	/// <summary>
	/// Indicates the font data of pencilmarked digits.
	/// </summary>
	public FontSerializationData PencilmarkFontData
	{
		get => new() { FontName = Pane.PencilmarkFont.Source, FontScale = (decimal)Pane.PencilmarkFontScale, FontColor = Pane.PencilmarkColor };

		set
		{
			if (PencilmarkFontData == value)
			{
				return;
			}

			Pane.PencilmarkFont = new(value.FontName);
			Pane.PencilmarkFontScale = (double)value.FontScale;
			Pane.PencilmarkColor = value.FontColor;
		}
	}

	/// <summary>
	/// Indicates the font data of Baba-grouping characters.
	/// </summary>
	public FontSerializationData BabaGroupingFontData
	{
		get => new()
		{
			FontName = Pane.BabaGroupLabelFont.Source,
			FontScale = (decimal)Pane.BabaGroupLabelFontScale,
			FontColor = Pane.BabaGroupLabelColor
		};

		set
		{
			if (BabaGroupingFontData == value)
			{
				return;
			}

			Pane.BabaGroupLabelFont = new(value.FontName);
			Pane.BabaGroupLabelFontScale = (double)value.FontScale;
			Pane.BabaGroupLabelColor = value.FontColor;
		}
	}

	/// <summary>
	/// Indicates the font data of coordinate labels.
	/// </summary>
	public FontSerializationData CoordinateLabelFontData
	{
		get => new()
		{
			FontName = Pane.CoordinateLabelFont.Source,
			FontScale = (decimal)Pane.CoordinateLabelFontScale,
			FontColor = Pane.CoordinateLabelColor
		};

		set
		{
			if (CoordinateLabelFontData == value)
			{
				return;
			}

			Pane.CoordinateLabelFont = new(value.FontName);
			Pane.CoordinateLabelFontScale = (double)value.FontScale;
			Pane.CoordinateLabelColor = value.FontColor;
		}
	}

	/// <summary>
	/// The faster entry for the sudoku pane.
	/// </summary>
	[JsonIgnore]
	private SudokuPane Pane => ((App)Application.Current).SudokuPane!;
}
