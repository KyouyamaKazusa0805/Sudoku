namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Normal Fish</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="baseSetsMask"><inheritdoc/></param>
/// <param name="coverSetsMask"><inheritdoc/></param>
/// <param name="fins">Indicates the fins.</param>
/// <param name="isSashimi">
/// Indicates whether the fish instance is a sashimi fish. All possible values are as below:
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
/// </param>
public sealed partial class NormalFishStep(
	Conclusion[] conclusions,
	View[]? views,
	Digit digit,
	HouseMask baseSetsMask,
	HouseMask coverSetsMask,
	[PrimaryConstructorParameter] scoped in CellMap fins,
	[PrimaryConstructorParameter] bool? isSashimi
) : FishStep(conclusions, views, digit, baseSetsMask, coverSetsMask)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 3.2M;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[]
		{
			(ExtraDifficultyCaseNames.Size, Size switch { 2 => 0, 3 => 0.6M, 4 => 2.0M }),
			(ExtraDifficultyCaseNames.Sashimi, IsSashimi switch { true => Size switch { 2 or 3 => .3M, 4 => .4M }, false => .2M, _ => 0 })
		};

	/// <inheritdoc/>
	public override Technique Code
	{
		get
		{
			scoped var buffer = (stackalloc char[InternalName.Length]);
			var i = 0;
			foreach (var ch in InternalName)
			{
				if (ch is not (' ' or '-'))
				{
					buffer[i++] = ch;
				}
			}

			return Enum.Parse<Technique>(buffer[..i]);
		}
	}

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ EnglishLanguage, new[] { DigitStr, BaseSetStr, CoverSetStr, FinsStr } },
			{ ChineseLanguage, new[] { DigitStr, BaseSetStr, CoverSetStr, FinsStr } }
		};

	/// <summary>
	/// Indicates the internal name.
	/// </summary>
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

	private string FinsStr => Fins ? $"{GetString("Fin")!}{Fins}" : string.Empty;
}
