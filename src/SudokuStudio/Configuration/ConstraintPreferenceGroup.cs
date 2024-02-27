namespace SudokuStudio.Configuration;

/// <summary>
/// Represents a constraint preference group.
/// </summary>
[DependencyProperty<ConstraintCollection>("Constraints")]
public sealed partial class ConstraintPreferenceGroup : PreferenceGroup
{
	[Default]
	private static readonly ConstraintCollection ConstraintsDefaultValue = [
		new DifficultyLevelConstraint { DifficultyLevel = DifficultyLevel.Easy, Operator = ComparisonOperator.Equality },
		new SymmetryConstraint { SymmetricTypes = SymmetricType.Central },
		new CountBetweenConstraint { Range = 24..28, CellState = CellState.Given, BetweenRule = BetweenRule.BothClosed },
		new MinimalConstraint { ShouldBeMinimal = false },
		new PearlConstraint { ShouldBePearlOrDiamond = false },
		new DiamondConstraint { ShouldBePearlOrDiamond = false },
		new IttoryuConstraint { Operator = ComparisonOperator.Equality, Rounds = 1 },
		new TechniqueConstraint { Technique = Technique.NakedSingle, LimitCount = 1, Operator = ComparisonOperator.GreaterThanOrEqual },
	];
}
