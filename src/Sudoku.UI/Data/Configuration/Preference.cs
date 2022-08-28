namespace Sudoku.UI.Data.Configuration;

using PeerSelectionMode = PeerFocusingMode;

/// <summary>
/// Defines the user preferences in the program.
/// </summary>
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
	public bool ShowCandidates { get; set; } = true;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	public bool ShowCandidateBorderLines { get; set; } = false;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see cref="PeerSelectionMode.FocusedCellAndPeerCells"/>.
	/// </remarks>
	public int PeerFocusingMode { get; set; } = (int)PeerSelectionMode.FocusedCellAndPeerCells;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#300000FF</c> (i.e. <see cref="Colors.Blue"/> with alpha 48).
	/// </remarks>
	public Color FocusedCellColor { get; set; } = Colors.Blue with { A = 48 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#200000FF</c> (i.e. <see cref="Colors.Blue"/> with alpha 32).
	/// </remarks>
	public Color PeersFocusedCellColor { get; set; } = Colors.Blue with { A = 32 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool EnableDeltaValuesDisplaying { get; set; } = true;

	/// <summary>
	/// Indicates whether the program will use zero character <c>'0'</c> as the placeholder to describe empty cells
	/// in a sudoku grid that we should copied.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool PlaceholderIsZero { get; set; } = true;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>{ FontName = "Cascadia Mono", FontScale = .8 }</c> in debugging mode.
	/// </remarks>
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

	/// <summary>
	/// Indicates whether the picture will also be saved when a drawing data file is saved to local.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool AlsoSavePictureWhenSaveDrawingData { get; set; } = true;

	/// <summary>
	/// Indicates whether the program always opens the home page if you open the program non-programmatically.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	public bool AlwaysShowHomePageWhenOpen { get; set; } = true;
	#endregion

	#region Solving Options
	/// <inheritdoc cref="ManualSolver.IsFullApplying"/>
	public bool IsFullApplying
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => DefaultSolver.IsFullApplying;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => DefaultSolver.IsFullApplying = value;
	}

	/// <inheritdoc cref="ManualSolver.AlmostLockedSetsXzStepSearcher_AllowCollision"/>
	public bool AllowCollisionOnChainingAlsTechniques
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _allowCollision;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_allowCollision = value;
			DefaultSolver.AlmostLockedSetsXzStepSearcher_AllowCollision = value;
			DefaultSolver.AlmostLockedSetsXyWingStepSearcher_AllowCollision = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.AlmostLockedSetsXzStepSearcher_AllowLoopedPatterns"/>
	public bool AllowDoublyLinkedAls
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _allowDoublyLinkedAls;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_allowDoublyLinkedAls = value;
			DefaultSolver.AlmostLockedSetsXzStepSearcher_AllowLoopedPatterns = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.AlmostLockedCandidatesStepSearcher_CheckAlmostLockedQuadruple"/>
	public bool CheckAlmostLockedQuadruple
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _checkAlq;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_checkAlq = value;
			DefaultSolver.AlmostLockedCandidatesStepSearcher_CheckAlmostLockedQuadruple = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.BivalueUniversalGraveStepSearcher_SearchExtendedTypes"/>
	public bool SearchBivalueUniversalGraveExtendedTypes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _searchBugExtendedTypes;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_searchBugExtendedTypes = value;
			DefaultSolver.BivalueUniversalGraveStepSearcher_SearchExtendedTypes = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.JuniorExocetStepSearcher_CheckAdvanced"/>
	public bool CheckAdvancedJuniorExocets
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _checkAdvancedJe;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_checkAdvancedJe = value;
			DefaultSolver.JuniorExocetStepSearcher_CheckAdvanced = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.SeniorExocetStepSearcher_CheckAdvanced"/>
	public bool CheckAdvancedSeniorExocets
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _checkAdvancedSe;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_checkAdvancedSe = value;
			DefaultSolver.SeniorExocetStepSearcher_CheckAdvanced = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.SingleStepSearcher_EnableFullHouse"/>
	public bool EnableFullHouse
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _enableFullHouse;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_enableFullHouse = value;
			DefaultSolver.SingleStepSearcher_EnableFullHouse = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.SingleStepSearcher_EnableLastDigit"/>
	public bool EnableLastDigit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _enableLastDigit;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_enableLastDigit = value;
			DefaultSolver.SingleStepSearcher_EnableLastDigit = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.SingleStepSearcher_HiddenSinglesInBlockFirst"/>
	public bool HiddenSinglesInBlockFirst
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _hiddenSingleHouseFirst;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_hiddenSingleHouseFirst = value;
			DefaultSolver.SingleStepSearcher_HiddenSinglesInBlockFirst = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.TemplateStepSearcher_TemplateDeleteOnly"/>
	public bool TemplateDeleteOnly
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _templateDeleteOnly;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_templateDeleteOnly = value;
			DefaultSolver.TemplateStepSearcher_TemplateDeleteOnly = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.UniqueRectangleStepSearcher_AllowIncompleteUniqueRectangles"/>
	public bool AllowIncompleteUniqueRectangles
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _allowIncompleteUr;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_allowIncompleteUr = value;
			DefaultSolver.UniqueRectangleStepSearcher_AllowIncompleteUniqueRectangles = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.UniqueRectangleStepSearcher_SearchForExtendedUniqueRectangles"/>
	public bool SearchForExtendedUniqueRectangles
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _searcherUrExtendedTypes;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_searcherUrExtendedTypes = value;
			DefaultSolver.UniqueRectangleStepSearcher_SearchForExtendedUniqueRectangles = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.RegularWingStepSearcher_MaxSize"/>
	public int RegularWingMaxSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _regularWingMaxSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_regularWingMaxSize = value;
			DefaultSolver.RegularWingStepSearcher_MaxSize = value;
		}
	}

	/// <inheritdoc cref="ManualSolver.ComplexFishStepSearcher_MaxSize"/>
	public int ComplexFishMaxSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _complexFishSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_complexFishSize = value;
			DefaultSolver.ComplexFishStepSearcher_MaxSize = value;
		}
	}
	#endregion

	#region Rendering Options
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>0</c>.
	/// </remarks>
	public double OutsideBorderWidth { get; set; } = 0;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>4</c>.
	/// </remarks>
	public double BlockBorderWidth { get; set; } = 4;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>1</c>.
	/// </remarks>
	public double CellBorderWidth { get; set; } = 1;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>1</c>.
	/// </remarks>
	public double CandidateBorderWidth { get; set; } = 1;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>{ FontName = "Times New Roman", FontScale = .8 }</c>.
	/// </remarks>
	public FontData UnknownValueFont { get; set; } = new() { FontName = "Times New Roman", FontScale = .8 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF0000</c> (i.e. <see cref="Colors.Red"/>).
	/// </remarks>
	public Color UnknownValueColor { get; set; } = Colors.Red;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	public Color OutsideBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFFFFF</c> (i.e. <see cref="Colors.White"/>).
	/// </remarks>
	public Color GridBackgroundFillColor { get; set; } = Colors.White;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	public Color BlockBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	public Color CellBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFD3D3D3</c> (i.e. <see cref="Colors.LightGray"/>).
	/// </remarks>
	public Color CandidateBorderColor { get; set; } = Colors.LightGray;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	public Color GivenColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF0000FF</c> (i.e. <see cref="Colors.Blue"/>).
	/// </remarks>
	public Color ModifiableColor { get; set; } = Colors.Blue;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF696969</c> (i.e. <see cref="Colors.DimGray"/>).
	/// </remarks>
	public Color CandidateColor { get; set; } = Colors.DimGray;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF0000</c> (i.e. <see cref="Colors.Red"/>).
	/// </remarks>
	public Color CellDeltaColor { get; set; } = Colors.Red;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFB9B9</c>.
	/// </remarks>
	public Color CandidateDeltaColor { get; set; } = Color.FromArgb(255, 255, 185, 185);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	public Color MaskEllipseColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>6</c>.
	/// </remarks>
	public double HouseViewNodeStrokeThickness { get; set; } = 6;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF3FDA65</c>.
	/// </remarks>
	public Color NormalColor { get; set; } = Color.FromArgb(255, 63, 218, 101);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF3FDA65</c>.
	/// </remarks>
	public Color AssignmentColor { get; set; } = Color.FromArgb(255, 63, 218, 101);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF7684</c>.
	/// </remarks>
	public Color EliminationColor { get; set; } = Color.FromArgb(255, 255, 118, 132);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF7FBBFF</c>.
	/// </remarks>
	public Color ExofinColor { get; set; } = Color.FromArgb(255, 127, 187, 255);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFD8B2FF</c>.
	/// </remarks>
	public Color EndofinColor { get; set; } = Color.FromArgb(255, 216, 178, 255);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFEB0000</c>.
	/// </remarks>
	public Color CannibalismColor { get; set; } = Color.FromArgb(255, 235, 0, 0);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF0000</c> (Red).
	/// </remarks>
	public Color LinkColor { get; set; } = Color.FromArgb(255, 255, 0, 0);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFFF00</c> (Yellow).
	/// </remarks>
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
	/// <summary>
	/// <para>
	/// Indicates whether the info bar controls will always be updated and inserted into the first place
	/// of the whole info bar board. If <see langword="true"/>, descending ordered mode will be enabled,
	/// the behavior will be like the above; otherwise, the new controls will be appended into the last place
	/// of the board.
	/// </para>
	/// <para>
	/// Sets the value to <see langword="true"/> may help you check new hints more quickly than
	/// the case setting the value to <see langword="false"/>.
	/// </para>
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool DescendingOrderedInfoBarBoard { get; set; } = true;

	/// <summary>
	/// Indicates whether the program always check the battery status at first, when you open the program
	/// non-programmatically.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool CheckBatteryStatusWhenOpen { get; set; } = true;
	#endregion

	#region Background Options
	/// <summary>
	/// Indicates whether the program is the first time to be used.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool IsFirstMeet { get; set; } = true;

	/// <inheritdoc/>
	public int RenderingCellSize { get; set; } = 60;
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
