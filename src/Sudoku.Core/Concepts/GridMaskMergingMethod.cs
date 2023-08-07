namespace Sudoku.Concepts;

/// <summary>
/// Represents a method that totals the masks in a list of cells up.
/// </summary>
public enum GridMaskMergingMethod
{
	/// <summary>
	/// Indicates the merging operation is <see langword="operator"/> <c><![CDATA[&]]></c> and <see langword="operator"/> <c><![CDATA[~]]></c>.
	/// </summary>
	AndNot,

	/// <summary>
	/// Indicates the merging operation is <see langword="operator"/> <c><![CDATA[&]]></c>.
	/// </summary>
	And,

	/// <summary>
	/// Indicates the merging operation is <see langword="operator"/> <c><![CDATA[|]]></c>.
	/// </summary>
	Or
}
