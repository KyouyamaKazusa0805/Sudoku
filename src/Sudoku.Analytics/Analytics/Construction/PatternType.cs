namespace Sudoku.Analytics.Construction;

/// <summary>
/// Represents a pattern type.
/// </summary>
public enum PatternType : short
{
	/// <summary>
	/// Represents none.
	/// </summary>
	None,

	/// <summary>
	/// Indicates almost hidden set.
	/// </summary>
	AlmostHiddenSet,

	/// <summary>
	/// Indicates almost locked set.
	/// </summary>
	AlmostLockedSet,

	/// <summary>
	/// Indicates avoidable rectangle.
	/// </summary>
	AvoidableRectangle,

	/// <summary>
	/// Indicates bi-value oddagon.
	/// </summary>
	BivalueOddagon,

	/// <summary>
	/// Indicates Borescoper's deadly pattern.
	/// </summary>
	BorescoperDeadlyPattern,

	/// <summary>
	/// Indicates chromatic pattern.
	/// </summary>
	ChromaticPattern,

	/// <summary>
	/// Indicates domino loop.
	/// </summary>
	DominoLoop,

	/// <summary>
	/// Indicates extended rectangle.
	/// </summary>
	ExtendedRectangle,

	/// <summary>
	/// Indicates firework.
	/// </summary>
	Firework,

	/// <summary>
	/// Indicates fish.
	/// </summary>
	Fish,

	/// <summary>
	/// Indicates guardian.
	/// </summary>
	Guardian,

	/// <summary>
	/// Indicates multi-sector locked set.
	/// </summary>
	MultisectorLockedSet,

	/// <summary>
	/// Indicates Qiu's deadly pattern 1.
	/// </summary>
	QiuDeadlyPattern1,

	/// <summary>
	/// Indicates Qiu's deadly pattern 2.
	/// </summary>
	QiuDeadlyPattern2,

	/// <summary>
	/// Indicates unique loop.
	/// </summary>
	UniqueLoop,

	/// <summary>
	/// Indicates unique rectangle.
	/// </summary>
	UniqueRectangle,

	/// <summary>
	/// Indicates XYZ-Wing.
	/// </summary>
	XyzWing
}
