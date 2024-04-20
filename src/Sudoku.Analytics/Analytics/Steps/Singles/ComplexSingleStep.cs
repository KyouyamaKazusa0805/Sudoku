
namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Complex Single</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="subtype"><inheritdoc/></param>
/// <param name="basedOn"><inheritdoc/></param>
/// <param name="indirectTechniques"><inheritdoc/></param>
public sealed partial class ComplexSingleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Cell cell,
	Digit digit,
	SingleSubtype subtype,
	Technique basedOn,
	Technique[][] indirectTechniques
) : ComplexSingleBaseStep(conclusions, views, options, cell, digit, subtype, basedOn, indirectTechniques)
{
	/// <inheritdoc/>
	public override int BaseDifficulty
		=> BasedOn switch
		{
			Technique.FullHouse => 10,
			Technique.CrosshatchingBlock => 12,
			Technique.CrosshatchingRow => 15,
			Technique.CrosshatchingColumn => 15,
			Technique.NakedSingle => 23
		} + 5;

	/// <inheritdoc/>
	public override Technique Code
		=> BasedOn switch
		{
			Technique.FullHouse => Technique.ComplexFullHouse,
			Technique.CrosshatchingBlock => Technique.ComplexCrosshatchingBlock,
			Technique.CrosshatchingRow => Technique.ComplexCrosshatchingRow,
			Technique.CrosshatchingColumn => Technique.ComplexCrosshatchingColumn,
			Technique.NakedSingle => Technique.ComplexNakedSingle
		};

	/// <inheritdoc/>
	public override FactorCollection Factors => [new ComplexSingleFactor()];


	/// <inheritdoc/>
	public override int CompareTo(Step? other)
	{
		if (other is not ComplexSingleStep comparer)
		{
			return 1;
		}

		var (countThis, countOther) = (IndirectTechniques.Length, comparer.IndirectTechniques.Length);
		if (countThis != countOther)
		{
			return countThis > countOther ? 1 : -1;
		}

		var (sortKeyThis, sortKeyOther) = (0, 0);
		for (var i = 0; i < IndirectTechniques.Length; i++)
		{
			sortKeyThis += getSortKey(IndirectTechniques[i][0]);
			sortKeyOther += getSortKey(comparer.IndirectTechniques[i][0]);
		}

		return sortKeyThis.CompareTo(sortKeyOther);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int getSortKey(Technique technique)
			=> technique switch
			{
				Technique.Pointing => 1,
				Technique.Claiming => 2,
				Technique.LockedPair or Technique.LockedHiddenPair => 3,
				Technique.LockedTriple or Technique.LockedHiddenTriple => 4,
				Technique.NakedPair or Technique.HiddenPair or Technique.NakedPairPlus => 5,
				Technique.NakedTriple or Technique.HiddenTriple or Technique.NakedTriplePlus => 6,
				Technique.NakedQuadruple or Technique.HiddenQuadruple or Technique.NakedQuadruplePlus => 7,
				_ => 10
			};
	}

	/// <inheritdoc/>
	public override string GetName(CultureInfo? culture = null)
	{
		var (hasLockedCandidates, hasSubset) = (false, false);
		foreach (var technique in IndirectTechniques)
		{
			switch (technique[0].GetGroup())
			{
				case TechniqueGroup.LockedCandidates: { hasLockedCandidates = true; break; }
				case TechniqueGroup.Subset: { hasSubset = true; break; }
			}
		}

		var lockedCandidatesName = ResourceDictionary.Get("Concept_LockedCandidates", culture);
		var subsetName = ResourceDictionary.Get("Concept_Subset", culture);
		var basedOnName = BasedOn.GetName(culture);
		var isChinese = culture?.Name is ['Z' or 'z', 'H' or 'h', ..];
		var spacing = isChinese ? string.Empty : " ";
		var prefix = (hasLockedCandidates, hasSubset) switch
		{
			(true, true) => $"{lockedCandidatesName}{spacing}{subsetName}",
			(true, false) => $"{lockedCandidatesName}",
			(false, true) => $"{subsetName}"
		};
		return isChinese
			? $"{base.GetName(culture)}{ResourceDictionary.Get("_Token_CenterDot", culture)}{prefix}{basedOnName}"
			: $"{base.GetName(culture)} ({prefix}{spacing}{basedOnName})";
	}
}
