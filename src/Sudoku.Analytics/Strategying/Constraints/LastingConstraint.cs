namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a lasting constraint.
/// </summary>
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
public sealed partial class LastingConstraint : Constraint, ILimitCountConstraint<int>, IComparisonOperatorConstraint
{
	/// <inheritdoc/>
	[HashCodeMember]
	[StringMember]
	public int LimitCount { get; set; }

	/// <summary>
	/// Indicates the technique used.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public SingleTechniqueFlag TechniqueFlag { get; set; }

	/// <inheritdoc/>
	[HashCodeMember]
	[StringMember]
	public ComparisonOperator Operator { get; set; }

	/// <inheritdoc/>
	static int ILimitCountConstraint<int>.Minimum => 1;

	/// <inheritdoc/>
	static int ILimitCountConstraint<int>.Maximum => 8;


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is LastingConstraint comparer
		&& (LimitCount, TechniqueFlag, Operator) == (comparer.LimitCount, comparer.TechniqueFlag, comparer.Operator);

	/// <inheritdoc/>
	public override string ToString(IFormatProvider? formatProvider)
	{
		var culture = formatProvider as CultureInfo;
		return string.Format(
			SR.Get("LastingConstraint", culture),
			[TechniqueFlag.GetName(culture), Operator.GetOperatorString(), LimitCount.ToString()]
		);
	}

	/// <inheritdoc/>
	public override LastingConstraint Clone()
		=> new() { LimitCount = LimitCount, TechniqueFlag = TechniqueFlag, Operator = Operator, IsNegated = IsNegated };

	/// <inheritdoc/>
	protected override bool CheckCore(ConstraintCheckingContext context)
	{
		var techniquesUsed = TechniqueSets.None;
		foreach (var step in context.AnalyzerResult)
		{
			if (step is SingleStep { Subtype: var st, Code: var technique } and ILastingTrait { Lasting: var l }
				&& (st.GetSingleTechnique() != TechniqueFlag || Operator.GetOperator<int>()(l, LimitCount)))
			{
				techniquesUsed.Add(technique);
				continue;
			}

			return false;
		}

		return techniquesUsed.Contains(
			TechniqueFlag switch
			{
				SingleTechniqueFlag.HiddenSingleBlock => Technique.CrosshatchingBlock,
				SingleTechniqueFlag.HiddenSingleRow => Technique.CrosshatchingRow,
				SingleTechniqueFlag.HiddenSingleColumn => Technique.CrosshatchingColumn,
				SingleTechniqueFlag.NakedSingle => Technique.NakedSingle
			}
		);
	}
}
