namespace SudokuStudio.Configuration;

/// <summary>
/// Represents a constraint preference group.
/// </summary>
public sealed partial class ConstraintPreferenceGroup : PreferenceGroup
{
	[Default]
	private static readonly ConstraintCollection ConstraintsDefaultValue = [];


	/// <summary>
	/// Indicates the constraints created.
	/// </summary>
	[AutoDependencyProperty]
	public partial ConstraintCollection Constraints { get; set; }
}
