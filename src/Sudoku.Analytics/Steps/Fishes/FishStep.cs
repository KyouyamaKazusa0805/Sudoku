namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Fish</b> technique.
/// </summary>
public abstract class FishStep(Conclusion[] conclusions, View[]? views, int digit, int baseSetsMask, int coverSetsMask) :
	Step(conclusions, views)
{
	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public int Digit { get; } = digit;

	/// <summary>
	/// Indicates the mask that contains the base sets.
	/// </summary>
	public int BaseSetsMask { get; } = baseSetsMask;

	/// <summary>
	/// Indicates the mask that contains the cover sets.
	/// </summary>
	public int CoverSetsMask { get; } = coverSetsMask;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	[AllowNull]
	[MaybeNull]
	public sealed override string Format => base.Format;

	/// <inheritdoc/>
	/// <remarks>
	/// The name of the corresponding names are:
	/// <list type="table">
	/// <item><term>2</term><description>X-Wing</description></item>
	/// <item><term>3</term><description>Swordfish</description></item>
	/// <item><term>4</term><description>Jellyfish</description></item>
	/// <item><term>5</term><description>Squirmbag (or Starfish)</description></item>
	/// <item><term>6</term><description>Whale</description></item>
	/// <item><term>7</term><description>Leviathan</description></item>
	/// </list>
	/// Other fishes of sizes not appearing in above don't have well-known names.
	/// </remarks>
	public int Size => PopCount((uint)BaseSetsMask);
}
