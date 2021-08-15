namespace Sudoku.Solving.Manual.Fishes;

/// <summary>
/// Provides a usage of <b>normal fish</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Digit">The digit used.</param>
/// <param name="BaseSets">The base sets.</param>
/// <param name="CoverSets">The cover sets.</param>
/// <param name="Fins">All fin cells.</param>
/// <param name="IsSashimi">
/// Indicates whether the fish instance is sashimi.
/// The value can be:
/// <list type="table">
/// <item>
/// <term><see langword="true"/></term><description>Sashimi finned fish.</description>
/// </item>
/// <item>
/// <term><see langword="false"/></term><description>Normal finned fish.</description>
/// </item>
/// <item>
/// <term><see langword="null"/></term><description>Normal fish.</description>
/// </item>
/// </list>
/// </param>
public sealed record NormalFishStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit,
	IReadOnlyList<int> BaseSets, IReadOnlyList<int> CoverSets, in Cells Fins, bool? IsSashimi
) : FishStepInfo(Conclusions, Views, Digit, BaseSets, CoverSets)
{
	/// <inheritdoc/>
	public override decimal Difficulty => BaseDifficulty + SashimiExtraDifficulty;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override unsafe Technique TechniqueCode
	{
		get
		{
			fixed (char* pName = InternalName)
			{
				var buffer = (stackalloc char[InternalName.Length]);

				int i = 0;
				for (char* p = pName; *p != '\0'; p++)
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
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.NormalFish;

	/// <summary>
	/// Indicates the base difficulty.
	/// </summary>
	private decimal BaseDifficulty => Size switch { 2 => 3.2M, 3 => 3.8M, 4 => 5.2M };

	/// <summary>
	/// Indicates the extra difficulty on sashimi judgement.
	/// </summary>
	private decimal SashimiExtraDifficulty => IsSashimi switch
	{
		null => 0,
		true => Size switch { 2 => .3M, 3 => .3M, 4 => .4M },
		false => .2M
	};

	/// <summary>
	/// Indicates the internal name.
	/// </summary>
	private string InternalName =>
		$"{IsSashimi switch { true => "Sashimi ", false => "Finned ", _ => string.Empty }}{FishNames[Size]}";

	[FormatItem]
	private string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit + 1).ToString();
	}

	[FormatItem]
	private string BaseSetStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(BaseSets).ToString();
	}

	[FormatItem]
	private string CoverSetStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(CoverSets).ToString();
	}

	[FormatItem]
	private string FinSnippet
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TextResources.Current.FinSnippet;
	}

	[FormatItem]
	private string FinsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Fins.IsEmpty ? string.Empty : $"{FinSnippet}{Fins}";
	}
}
