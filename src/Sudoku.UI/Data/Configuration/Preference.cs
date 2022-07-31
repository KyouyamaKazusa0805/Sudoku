#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
#pragma warning disable IDE1006
#endif

namespace Sudoku.UI.Data.Configuration;

/// <summary>
/// Defines the user preferences in the program.
/// </summary>
/// <remarks>
/// Due to the special initialization mechanism, the properties of the type should satisfy the following conditions:
/// <list type="bullet">
/// <item>
/// Only properties that contains <b>both</b> getter and normal setter (and not a <see langword="init"/> setter)
/// can be loaded, initialized and displayed to the settings page.
/// </item>
/// <item>
/// The property must be applied both attribute types <see cref="PreferenceAttribute{TSettingItem}"/>
/// and <see cref="PreferenceGroupAttribute"/>.
/// </item>
/// <item>
/// If the property is a non-formal preference item, a double extra leading underscore characters "<c>__</c>"
/// will be inserted into the property name; otherwise, a normal property name is applied.
/// </item>
/// </list>
/// </remarks>
/// <seealso cref="PreferenceAttribute{TSettingItem}"/>
/// <seealso cref="PreferenceGroupAttribute"/>
public sealed class Preference : IDrawingPreference
{
	/// <summary>
	/// Indicates the default solver.
	/// </summary>
	[JsonIgnore]
	public static readonly ManualSolver DefaultSolver = new();


	/// <inheritdoc cref="IAlmostLockedSetsXzStepSearcher.AllowCollision"/>
	private bool _allowCollision = true;

	/// <inheritdoc cref="IAlmostLockedSetsXzStepSearcher.AllowLoopedPatterns"/>
	private bool _allowDoublyLinkedAls = true;

	/// <inheritdoc cref="IAlmostLockedCandidatesStepSearcher.CheckAlmostLockedQuadruple"/>
	private bool _checkAlq = false;

	/// <inheritdoc cref="IBivalueUniversalGraveStepSearcher.SearchExtendedTypes"/>
	private bool _searchBugExtendedTypes = true;

	/// <inheritdoc cref="IExocetStepSearcher.CheckAdvanced"/>
	private bool _checkAdvancedJe = false;

	/// <inheritdoc cref="IExocetStepSearcher.CheckAdvanced"/>
	private bool _checkAdvancedSe = false;

	/// <inheritdoc cref="ISingleStepSearcher.EnableFullHouse"/>
	private bool _enableFullHouse = true;

	/// <inheritdoc cref="ISingleStepSearcher.EnableLastDigit"/>
	private bool _enableLastDigit = true;

	/// <inheritdoc cref="ISingleStepSearcher.HiddenSinglesInBlockFirst"/>
	private bool _hiddenSingleHouseFirst = true;

	/// <inheritdoc cref="ITemplateStepSearcher.TemplateDeleteOnly"/>
	private bool _templateDeleteOnly = false;

	/// <inheritdoc cref="IUniqueRectangleStepSearcher.AllowIncompleteUniqueRectangles"/>
	private bool _allowIncompleteUr = true;

	/// <inheritdoc cref="IUniqueRectangleStepSearcher.SearchForExtendedUniqueRectangles"/>
	private bool _searcherUrExtendedTypes = true;

	/// <inheritdoc cref="IRegularWingStepSearcher.MaxSize"/>
	private int _regularWingMaxSize = 9;

	/// <inheritdoc cref="IFishStepSearcher.MaxSize"/>
	private int _complexFishSize = 5;


	#region Basic Options
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 0)]
	public bool ShowCandidates { get; set; } = true;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 1)]
	public bool ShowCandidateBorderLines { get; set; } = false;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see cref="PeerFocusingMode.FocusedCellAndPeerCells"/>.
	/// </remarks>
	[Preference<PeerFocusingModeComboBoxSettingItem>(nameof(PeerFocusingModeComboBoxSettingItem.OptionContents), 3)]
	[PreferenceGroup(PreferenceGroupNames.Basic, 2)]
	public PeerFocusingMode PeerFocusingMode { get; set; } = PeerFocusingMode.FocusedCellAndPeerCells;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#300000FF</c> (i.e. <see cref="Colors.Blue"/> with alpha 48).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 3)]
	public Color FocusedCellColor { get; set; } = Colors.Blue with { A = 48 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#200000FF</c> (i.e. <see cref="Colors.Blue"/> with alpha 32).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 4)]
	public Color PeersFocusedCellColor { get; set; } = Colors.Blue with { A = 32 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 5)]
	public bool EnableDeltaValuesDisplaying { get; set; } = true;

	/// <summary>
	/// Indicates whether the program will use zero character <c>'0'</c> as the placeholder to describe empty cells
	/// in a sudoku grid that we should copied.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 6)]
	public bool PlaceholderIsZero { get; set; } = true;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>{ FontName = "Cascadia Mono", FontScale = .8 }</c> in debugging mode.
	/// </remarks>
	[Preference<FontPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 7)]
	public FontData ValueFont { get; set; } = new()
	{
		FontName =
#if DEBUG
			"Cascadia Mono",
#else
			"Tahoma",
#endif
		FontScale = .8
	};

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>{ FontName = "Cascadia Mono", FontScale = .25 }</c> in debugging mode.
	/// </remarks>
	[Preference<FontPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 8)]
	public FontData CandidateFont { get; set; } = new()
	{
		FontName =
#if DEBUG
			"Cascadia Mono",
#else
			"Tahoma",
#endif
		FontScale = .25
	};

#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <summary>
	/// Indicates whether the old shape should be covered when diffused.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 9)]
	public bool __CoverOldShapeWhenDiffused { get; set; } = false;
#endif

	/// <summary>
	/// Indicates whether the picture will also be saved when a drawing data file is saved to local.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 10)]
	public bool AlsoSavePictureWhenSaveDrawingData { get; set; } = true;

	/// <summary>
	/// Indicates whether the program always opens the home page if you open the program non-programmatically.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 11)]
	public bool AlwaysShowHomePageWhenOpen { get; set; } = true;
	#endregion

	#region Solving Options
	/// <inheritdoc cref="ManualSolver.IsHodokuMode"/>
	public bool IsHodokuMode
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => DefaultSolver.IsHodokuMode;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => DefaultSolver.IsHodokuMode = value;
	}

	/// <inheritdoc cref="ManualSolver.IsFullApplying"/>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Solving, 0)]
	public bool IsFullApplying
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => DefaultSolver.IsFullApplying;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => DefaultSolver.IsFullApplying = value;
	}

	/// <inheritdoc cref="ManualSolver.AllowCollisionOnAlsXz"/>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Solving, 1)]
	public bool AllowCollisionOnChainingAlsTechniques
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _allowCollision;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_allowCollision = value;
			DefaultSolver.AllowCollisionOnAlsXz = value;
			DefaultSolver.AllowCollisionOnAlsXyWing = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.AllowLoopedPatternsOnAlsXz"/>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Solving, 2)]
	public bool AllowDoublyLinkedAls
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _allowDoublyLinkedAls;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_allowDoublyLinkedAls = value;
			DefaultSolver.AllowLoopedPatternsOnAlsXz = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.CheckAlmostLockedQuadruple"/>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Solving, 3)]
	public bool CheckAlmostLockedQuadruple
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _checkAlq;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_checkAlq = value;
			DefaultSolver.CheckAlmostLockedQuadruple = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.SearchBivalueUniversalGraveExtendedTypes"/>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Solving, 4)]
	public bool SearchBivalueUniversalGraveExtendedTypes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _searchBugExtendedTypes;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_searchBugExtendedTypes = value;
			DefaultSolver.SearchBivalueUniversalGraveExtendedTypes = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.CheckAdvancedJuniorExocets"/>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Solving, 5)]
	public bool CheckAdvancedJuniorExocets
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _checkAdvancedJe;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_checkAdvancedJe = value;
			DefaultSolver.CheckAdvancedJuniorExocets = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.CheckAdvancedSeniorExocets"/>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Solving, 6)]
	public bool CheckAdvancedSeniorExocets
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _checkAdvancedSe;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_checkAdvancedSe = value;
			DefaultSolver.CheckAdvancedSeniorExocets = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.EnableFullHouse"/>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Solving, 7)]
	public bool EnableFullHouse
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _enableFullHouse;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_enableFullHouse = value;
			DefaultSolver.EnableFullHouse = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.EnableLastDigit"/>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Solving, 8)]
	public bool EnableLastDigit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _enableLastDigit;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_enableLastDigit = value;
			DefaultSolver.EnableLastDigit = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.HiddenSinglesInBlockFirst"/>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Solving, 9)]
	public bool HiddenSinglesInBlockFirst
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _hiddenSingleHouseFirst;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_hiddenSingleHouseFirst = value;
			DefaultSolver.HiddenSinglesInBlockFirst = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.TemplateDeleteOnly"/>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Solving, 10)]
	public bool TemplateDeleteOnly
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _templateDeleteOnly;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_templateDeleteOnly = value;
			DefaultSolver.TemplateDeleteOnly = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.AllowIncompleteUniqueRectangles"/>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Solving, 11)]
	public bool AllowIncompleteUniqueRectangles
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _allowIncompleteUr;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_allowIncompleteUr = value;
			DefaultSolver.AllowIncompleteUniqueRectangles = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.SearchForExtendedUniqueRectangles"/>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Solving, 12)]
	public bool SearchForExtendedUniqueRectangles
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _searcherUrExtendedTypes;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_searcherUrExtendedTypes = value;
			DefaultSolver.SearchForExtendedUniqueRectangles = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.RegularWingMaxSize"/>
	[Preference<Int32SliderSettingItem>(
		nameof(Int32SliderSettingItem.StepFrequency), 1, nameof(Int32SliderSettingItem.TickFrequency), 1,
		nameof(Int32SliderSettingItem.MinValue), 3, nameof(Int32SliderSettingItem.MaxValue), 9)]
	[PreferenceGroup(PreferenceGroupNames.Solving, 13)]
	public int RegularWingMaxSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _regularWingMaxSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_regularWingMaxSize = value;
			DefaultSolver.RegularWingMaxSize = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.ComplexFishMaxSize"/>
	[Preference<Int32SliderSettingItem>(
		nameof(Int32SliderSettingItem.StepFrequency), 1, nameof(Int32SliderSettingItem.TickFrequency), 1,
		nameof(Int32SliderSettingItem.MinValue), 2, nameof(Int32SliderSettingItem.MaxValue), 7)]
	[PreferenceGroup(PreferenceGroupNames.Solving, 14)]
	public int ComplexFishMaxSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _complexFishSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_complexFishSize = value;
			DefaultSolver.ComplexFishMaxSize = value;
		}
	}
	#endregion

	#region Rendering Options
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>0</c>.
	/// </remarks>
	[Preference<SliderSettingItem>(
		nameof(SliderSettingItem.StepFrequency), .1, nameof(SliderSettingItem.TickFrequency), .3,
		nameof(SliderSettingItem.MinValue), 0D, nameof(SliderSettingItem.MaxValue), 3D)]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 0)]
	public double OutsideBorderWidth { get; set; } = 0;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>4</c>.
	/// </remarks>
	[Preference<SliderSettingItem>(
		nameof(SliderSettingItem.StepFrequency), .5, nameof(SliderSettingItem.TickFrequency), .5,
		nameof(SliderSettingItem.MinValue), 0D, nameof(SliderSettingItem.MaxValue), 5D)]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 1)]
	public double BlockBorderWidth { get; set; } = 4;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>1</c>.
	/// </remarks>
	[Preference<SliderSettingItem>(
		nameof(SliderSettingItem.StepFrequency), .1, nameof(SliderSettingItem.TickFrequency), .3,
		nameof(SliderSettingItem.MinValue), 0D, nameof(SliderSettingItem.MaxValue), 3D)]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 2)]
	public double CellBorderWidth { get; set; } = 1;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>1</c>.
	/// </remarks>
	[Preference<SliderSettingItem>(
		nameof(SliderSettingItem.StepFrequency), .1, nameof(SliderSettingItem.TickFrequency), .3,
		nameof(SliderSettingItem.MinValue), 0D, nameof(SliderSettingItem.MaxValue), 3D)]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 3)]
	public double CandidateBorderWidth { get; set; } = 1;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>{ FontName = "Times New Roman", FontScale = .8 }</c>.
	/// </remarks>
	[Preference<FontPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 4)]
	public FontData UnknownValueFont { get; set; } = new() { FontName = "Times New Roman", FontScale = .8 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF0000</c> (i.e. <see cref="Colors.Red"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 5)]
	public Color UnknownValueColor { get; set; } = Colors.Red;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 6)]
	public Color OutsideBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFFFFF</c> (i.e. <see cref="Colors.White"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 7)]
	public Color GridBackgroundFillColor { get; set; } = Colors.White;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 8)]
	public Color BlockBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 9)]
	public Color CellBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFD3D3D3</c> (i.e. <see cref="Colors.LightGray"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 10)]
	public Color CandidateBorderColor { get; set; } = Colors.LightGray;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 11)]
	public Color GivenColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF0000FF</c> (i.e. <see cref="Colors.Blue"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 12)]
	public Color ModifiableColor { get; set; } = Colors.Blue;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF696969</c> (i.e. <see cref="Colors.DimGray"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 13)]
	public Color CandidateColor { get; set; } = Colors.DimGray;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF0000</c> (i.e. <see cref="Colors.Red"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 14)]
	public Color CellDeltaColor { get; set; } = Colors.Red;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFB9B9</c>.
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 15)]
	public Color CandidateDeltaColor { get; set; } = Color.FromArgb(255, 255, 185, 185);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 16)]
	public Color MaskEllipseColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>6</c>.
	/// </remarks>
	[Preference<SliderSettingItem>(
		nameof(SliderSettingItem.StepFrequency), .5, nameof(SliderSettingItem.TickFrequency), 1D,
		nameof(SliderSettingItem.MinValue), 0D, nameof(SliderSettingItem.MaxValue), 6D)]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 17)]
	public double HouseViewNodeStrokeThickness { get; set; } = 6;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF3FDA65</c>.
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 18)]
	public Color NormalColor { get; set; } = Color.FromArgb(255, 63, 218, 101);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF3FDA65</c>.
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 19)]
	public Color AssignmentColor { get; set; } = Color.FromArgb(255, 63, 218, 101);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF7684</c>.
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 20)]
	public Color EliminationColor { get; set; } = Color.FromArgb(255, 255, 118, 132);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF7FBBFF</c>.
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 21)]
	public Color ExofinColor { get; set; } = Color.FromArgb(255, 127, 187, 255);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFD8B2FF</c>.
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 22)]
	public Color EndofinColor { get; set; } = Color.FromArgb(255, 216, 178, 255);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFEB0000</c>.
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 23)]
	public Color CannibalismColor { get; set; } = Color.FromArgb(255, 235, 0, 0);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF0000</c> (Red).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 24)]
	public Color LinkColor { get; set; } = Color.FromArgb(255, 255, 0, 0);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFFF00</c> (Yellow).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 25)]
	public Color GroupedLinkNodeColor { get; set; } = Colors.Yellow;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is an array of 3 elements:
	/// <list type="number">
	/// <item>#FF7FBBFF</item>
	/// <item>#FFD8B2FF</item>
	/// <item>#FFFFFF96</item>
	/// </list>
	/// </remarks>
	[Preference<ColorSelectorGroupSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 26)]
	public Color[] AuxiliaryColors { get; set; } =
	{
		Color.FromArgb(255, 127, 187, 255), // FF7FBBFF
		Color.FromArgb(255, 216, 178, 255), // FFD8B2FF
		Color.FromArgb(255, 255, 255, 150) // FFFFFF96
	};

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is an array of 5 elements:
	/// <list type="number">
	/// <item>#FFC5E88C</item>
	/// <item>#FFFFCBCB</item>
	/// <item>#FFB2DFDF</item>
	/// <item>#FFFCDCA5</item>
	/// <item>#FFFFFF96</item>
	/// </list>
	/// The former 4 items of this array are referenced from sudoku project
	/// <see href="https://sourceforge.net/projects/hodoku/">Hodoku</see>.
	/// </remarks>
	[Preference<ColorSelectorGroupSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 27)]
	public Color[] AlmostLockedSetColors { get; set; } =
	{
		Color.FromArgb(255, 197, 232, 140), // FFC5E88C
		Color.FromArgb(255, 255, 203, 203), // FFFFCBCB
		Color.FromArgb(255, 178, 223, 223), // FFB2DFDF
		Color.FromArgb(255, 252, 220, 165), // FFFCDCA5
		Color.FromArgb(255, 255, 255, 150) // FFFFFF96
	};

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is an array of 10 elements:
	/// <list type="number">
	/// <item>#FFFFC059 (Orange)</item>
	/// <item>#FFB1A5F3 (Light purple)</item>
	/// <item>#FFF7A5A7 (Red)</item>
	/// <item>#FF86E8D0 (Sky blue)</item>
	/// <item>#FF86F280 (Light green)</item>
	/// <item>#FFF7DE8F (Light orange)</item>
	/// <item>#FFDCD4FC (Whitey purple)</item>
	/// <item>#FFFFD2D2 (Light red)</item>
	/// <item>#FFCEFBED (Whitey blue)</item>
	/// <item>#FFD7FFD7 (Whitey green)</item>
	/// </list>
	/// All values of this array are referenced from sudoku project
	/// <see href="https://sourceforge.net/projects/hodoku/">Hodoku</see>.
	/// </remarks>
	[Preference<ColorSelectorGroupSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 28)]
	public Color[] PaletteColors { get; set; } =
	{
		Color.FromArgb(255, 255, 192, 89), // FFFFC059
		Color.FromArgb(255, 177, 165, 243), // FFB1A5F3
		Color.FromArgb(255, 247, 165, 167), // FFF7A5A7
		Color.FromArgb(255, 134, 232, 208), // FF86E8D0
		Color.FromArgb(255, 134, 242, 128), // FF86F280
		Color.FromArgb(255, 247, 222, 143), // FFF7DE8F
		Color.FromArgb(255, 220, 212, 252), // FFDCD4FC
		Color.FromArgb(255, 255, 210, 210), // FFFFD2D2
		Color.FromArgb(255, 206, 251, 237), // FFCEFBED
		Color.FromArgb(255, 215, 255, 215) // FFD7FFD7
	};
	#endregion

	#region Miscellaneous Options
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Miscellaneous, 0)]
	public bool DescendingOrderedInfoBarBoard { get; set; } = true;

	/// <summary>
	/// Indicates whether the program always check the battery status at first, when you open the program
	/// non-programmatically.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Miscellaneous, 1)]
	public bool CheckBatteryStatusWhenOpen { get; set; } = true;
	#endregion

	#region Background Options
	/// <summary>
	/// Indicates whether the program is the first time to be used.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	[BackgroundPreference]
	public bool IsFirstMeet { get; set; } = true;
	#endregion

	#region Deprecated Options
#if AUTHOR_FEATURE_CELL_MARKS
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>4</c>.
	/// </remarks>
	public double __CrossMarkStrokeThickness { get; set; } = 4;
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>2</c>.
	/// </remarks>
	public double __CandidateMarkStrokeThickness { get; set; } = 2;
#endif

#if AUTHOR_FEATURE_CELL_MARKS
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __CellRectangleFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __CellCircleFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __CrossMarkStrokeColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __StarFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __TriangleFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __DiamondFillColor { get; set; } = Colors.Black with { A = 64 };
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#80000000</c> (i.e. <see cref="Colors.Black"/> with alpha 128).
	/// </remarks>
	public Color __CandidateMarkStrokeColor { get; set; } = Colors.Black with { A = 128 };
#endif
	#endregion


	/// <summary>
	/// Covers the config file by the specified preference instance.
	/// </summary>
	/// <param name="preference">
	/// The preference instance. If the value is <see langword="null"/>, no operation will be handled.
	/// </param>
	public void CoverPreferenceBy(Preference? preference)
	{
		if (preference is null)
		{
			return;
		}

		((IDrawingPreference)this).CoverPreferenceBy(preference);
	}
}
