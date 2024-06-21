namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a data structure that describes for a technique of <b>Complex Single</b>.
/// </summary>
/// <param name="conclusions"><inheritdoc cref="Step.Conclusions" path="/summary"/></param>
/// <param name="views"><inheritdoc cref="Step.Views" path="/summary"/></param>
/// <param name="options"><inheritdoc cref="Step.Options" path="/summary"/></param>
/// <param name="cell"><inheritdoc cref="SingleStep.Cell" path="/summary"/></param>
/// <param name="digit"><inheritdoc cref="SingleStep.Digit" path="/summary"/></param>
/// <param name="subtype"><inheritdoc cref="SingleStep.Subtype" path="/summary"/></param>
/// <param name="basedOn"><inheritdoc cref="ComplexSingleBaseStep.BasedOn" path="/summary"/></param>
/// <param name="indirectTechniques"><inheritdoc cref="ComplexSingleBaseStep.IndirectTechniques" path="/summary"/></param>
public sealed class ComplexSingleStep(
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
		};

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
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [TechniqueNotationEnUs]), new(ChineseLanguage, [TechniqueNotationZhCn])];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new ComplexSingleFactor()];

	private string TechniqueNotationEnUs
		=> string.Join(
			" -> ",
			from techniqueGroup in IndirectTechniques
			let tt = string.Join(", ", from subtechnique in techniqueGroup select subtechnique.GetName(EnglishCulture))
			select techniqueGroup.Length == 1 ? tt : $"({tt})"
		);

	private string TechniqueNotationZhCn
		=> string.Join(
			" -> ",
			from techniqueGroup in IndirectTechniques
			let tt = string.Join(", ", from subtechnique in techniqueGroup select subtechnique.GetName(ChineseCulture))
			select techniqueGroup.Length == 1 ? tt : $"({tt})"
		);


	/// <inheritdoc/>
	public override int CompareTo(Step? other)
	{
		if (other is not ComplexSingleStep comparer)
		{
			return -1;
		}

		var (countThis, countOther) = (IndirectTechniques.Length, comparer.IndirectTechniques.Length);
		if (countThis != countOther)
		{
			return countThis > countOther ? 1 : -1;
		}

		var (sortKeyThis, sortKeyOther) = (0, 0);
		for (var i = 0; i < IndirectTechniques.Length; i++)
		{
			sortKeyThis += IndirectTechniques[i].Sum(getSortKey);
			sortKeyOther += comparer.IndirectTechniques[i].Sum(getSortKey);
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
	public override string GetName(IFormatProvider? formatProvider)
	{
		var (hasLockedCandidates, hasSubset) = (false, false);
		foreach (var techniqueGroup in IndirectTechniques)
		{
			foreach (var technique in techniqueGroup)
			{
				switch (technique.GetGroup())
				{
					case TechniqueGroup.LockedCandidates: { hasLockedCandidates = true; break; }
					case TechniqueGroup.Subset: { hasSubset = true; break; }
				}
			}
		}

		var culture = formatProvider as CultureInfo ?? Culture;
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
