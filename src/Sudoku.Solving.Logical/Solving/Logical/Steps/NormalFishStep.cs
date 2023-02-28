﻿namespace Sudoku.Solving.Logical.Steps;

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
internal sealed record NormalFishStep(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit,
	int BaseSetsMask,
	int CoverSetsMask,
	scoped in CellMap Fins,
	bool? IsSashimi
) : FishStep(Conclusions, Views, Digit, BaseSetsMask, CoverSetsMask)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 3.2M;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Size, Size switch { 2 => 0, 3 => 0.6M, 4 => 2.0M }),
			new(ExtraDifficultyCaseNames.Sashimi, IsSashimi switch { true => Size switch { 2 or 3 => .3M, 4 => .4M, }, false => .2M, _ => 0 })
		};

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override unsafe Technique TechniqueCode
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
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.NormalFish;

	/// <inheritdoc/>
	public override Rarity Rarity => Size switch { 2 => Rarity.Sometimes, 3 or 4 => Rarity.Seldom };


	/// <summary>
	/// Indicates the internal name.
	/// </summary>
	[DebuggerHidden]
	private string InternalName
	{
		get
		{
			var finModifier = IsSashimi switch { true => "Sashimi ", false => "Finned ", _ => string.Empty };
			var fishName = Size switch { 2 => "X-Wing", 3 => "Swordfish", 4 => "Jellyfish" };
			return $"{finModifier}{fishName}";
		}
	}


	[ResourceTextFormatter]
	internal string DigitStr() => (Digit + 1).ToString();

	[ResourceTextFormatter]
	internal string BaseSetStr() => HouseFormatter.Format(BaseSetsMask);

	[ResourceTextFormatter]
	internal string CoverSetStr() => HouseFormatter.Format(CoverSetsMask);

	[ResourceTextFormatter]
	internal string FinSnippet() => R["Fin"]!;

	[ResourceTextFormatter]
	internal string FinsStr() => Fins ? $"{FinSnippet()}{Fins}" : string.Empty;
}
