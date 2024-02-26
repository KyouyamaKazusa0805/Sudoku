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
		new MinimalConstraint { ShouldBeMinimal = false },
		new CountBetweenConstraint { Range = 24..28, CellState = CellState.Given, BetweenRule = BetweenRule.BothClosed }
	];
}
