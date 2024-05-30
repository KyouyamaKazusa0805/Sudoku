namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that limits a puzzle that can only use such techniques to be finished.
/// </summary>
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
public sealed partial class TechniqueSetConstraint : Constraint
{
	/// <summary>
	/// Indicates the technique used.
	/// </summary>
	[HashCodeMember]
	public TechniqueSet Techniques { get; set; } = TechniqueSets.None;

	[StringMember]
	private string TechniquesString => Techniques.ToString();


	/// <inheritdoc/>
	public override TechniqueSetConstraint Clone() => new() { Techniques = Techniques };

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is TechniqueSetConstraint comparer && Techniques == comparer.Techniques;

	/// <inheritdoc/>
	public override string ToString(IFormatProvider? formatProvider)
	{
		var culture = formatProvider as CultureInfo;
		return string.Format(ResourceDictionary.Get("TechniqueSetConstraint", culture), Techniques.ToString(culture));
	}

	/// <inheritdoc/>
	protected override bool CheckCore(ConstraintCheckingContext context)
	{
		switch (Techniques)
		{
			case [Technique.FullHouse]:
			{
				return context.Grid.CanPrimaryFullHouse();
			}
			case [Technique.CrosshatchingBlock or Technique.HiddenSingleBlock]:
			{
				return context.Grid.CanPrimaryHiddenSingle(false);
			}
			case [Technique.CrosshatchingRow or Technique.CrosshatchingColumn or Technique.HiddenSingleRow or Technique.HiddenSingleColumn]:
			{
				return context.Grid.CanPrimaryHiddenSingle(true);
			}
			case [Technique.NakedSingle]:
			{
				return context.Grid.CanPrimaryNakedSingle();
			}
			default:
			{
				foreach (var step in context.AnalyzerResult)
				{
					if (!Techniques.Contains(step.Code))
					{
						return false;
					}
				}
				return true;
			}
		}
	}
}
