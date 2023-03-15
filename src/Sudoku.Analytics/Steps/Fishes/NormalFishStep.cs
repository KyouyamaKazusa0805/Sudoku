namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Normal Fish</b> technique.
/// </summary>
public sealed class NormalFishStep(
	Conclusion[] conclusions,
	View[]? views,
	int digit,
	int baseSetsMask,
	int coverSetsMask,
	scoped in CellMap fins,
	bool? isSashimi
) : FishStep(conclusions, views, digit, baseSetsMask, coverSetsMask)
{
	/// <summary>
	/// Indicates whether the fish instance is a sashimi fish. All possible values are as belows:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The fish is a sashimi fish.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>The fish is a finned fish.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The fish is a normal fish without any fins.</description>
	/// </item>
	/// </list>
	/// </summary>
	public bool? IsSashimi { get; } = isSashimi;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 3.2M;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Size, Size switch { 2 => 0, 3 => 0.6M, 4 => 2.0M }),
			new(ExtraDifficultyCaseNames.Sashimi, IsSashimi switch { true => Size switch { 2 or 3 => .3M, 4 => .4M, }, false => .2M, _ => 0 })
		};

	/// <summary>
	/// Indicates the fins.
	/// </summary>
	public CellMap Fins { get; } = fins;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override unsafe Technique Code
	{
		get
		{
			fixed (char* pName = InternalName)
			{
				scoped var buffer = (stackalloc char[InternalName.Length]);

				var i = 0;
				for (var p = pName; *p != '\0'; p++)
				{
					if (*p is var ch and not (' ' or '-'))
					{
						buffer[i++] = ch;
					}
				}

				return Enum.Parse<Technique>(buffer[..i].ToString());
			}
		}
	}

	/// <inheritdoc/>
	public override TechniqueGroup Group => TechniqueGroup.NormalFish;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitStr, BaseSetStr, CoverSetStr, FinsStr } },
			{ "zh", new[] { DigitStr, BaseSetStr, CoverSetStr, FinsStr } }
		};

	/// <summary>
	/// Indicates the internal name.
	/// </summary>
	[DebuggerHidden]
	private string InternalName
	{
		get
		{
			var finModifier = isSashimi switch { true => "Sashimi ", false => "Finned ", _ => string.Empty };
			var fishName = Size switch { 2 => "X-Wing", 3 => "Swordfish", 4 => "Jellyfish" };
			return $"{finModifier}{fishName}";
		}
	}

	private string DigitStr => (Digit + 1).ToString();

	private string BaseSetStr => HouseFormatter.Format(BaseSetsMask);

	private string CoverSetStr => HouseFormatter.Format(CoverSetsMask);

	private string FinsStr => Fins ? $"{R["Fin"]!}{Fins}" : string.Empty;
}
