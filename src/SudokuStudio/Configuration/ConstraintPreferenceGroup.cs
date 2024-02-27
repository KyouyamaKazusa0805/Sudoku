namespace SudokuStudio.Configuration;

/// <summary>
/// Represents a constraint preference group.
/// </summary>
[DependencyProperty<ConstraintCollection>("Constraints")]
public sealed partial class ConstraintPreferenceGroup : PreferenceGroup
{
	[Default]
	private static readonly ConstraintCollection ConstraintsDefaultValue = [];
}
