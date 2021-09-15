namespace Sudoku.Solving.Manual.Steps.Fishes;

/// <summary>
/// Provides with a step that is an <b>Normal Fish</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
/// <param name="BaseSetsMask"><inheritdoc/></param>
/// <param name="CoverSetsMask"><inheritdoc/></param>
/// <param name="Fins">Indicates the fins.</param>
/// <param name="IsSashimi">
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
/// </param>
public sealed record class NormalFishStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	int Digit,
	int BaseSetsMask,
	int CoverSetsMask,
	in Cells Fins,
	bool? IsSashimi
) : FishStep(Conclusions, Views, Digit, BaseSetsMask, CoverSetsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		Size switch { 2 => 3.2M, 3 => 3.8M, 4 => 5.2M }
		+ IsSashimi switch { null => 0, true => Size switch { 2 => .3M, 3 => .3M, 4 => .4M }, false => .2M };

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

	/// <inheritdoc/>
	public override Rarity Rarity => Size switch { 2 => Rarity.Sometimes, 3 or 4 => Rarity.Seldom };

	/// <summary>
	/// Indicates the internal name.
	/// </summary>
	private string InternalName => $@"{
		IsSashimi switch { true => "Sashimi ", false => "Finned ", _ => string.Empty }
	}{
		Size switch { 2 => "X-Wing", 3 => "Swordfish", 4 => "Jellyfish" }
	}";

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
		get => new RegionCollection(BaseSetsMask.GetAllSets()).ToString();
	}

	[FormatItem]
	private string CoverSetStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(CoverSetsMask.GetAllSets()).ToString();
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