namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides with the constants.
/// </summary>
internal static class Constants
{
	/// <summary>
	/// Indicates the invalid first set value
	/// after called <see cref="TrailingZeroCount(int)"/> and <see cref="TrailingZeroCount(uint)"/>.
	/// </summary>
	/// <remarks>
	/// For more details you want to learn about, please visit
	/// <see href="https://github.com/dotnet/runtime/blob/a67d5680186ead0c9afdab7e004389c979d5fc1f/src/libraries/System.Private.CoreLib/src/System/Numerics/BitOperations.cs#L467">this link</see>
	/// to get the inner code.
	/// </remarks>
	/// <seealso cref="TrailingZeroCount(int)"/>
	/// <seealso cref="TrailingZeroCount(uint)"/>
	public const int InvalidFirstSet = 32;

	/// <summary>
	/// Indicates the number of all possible Unique Square templates.
	/// </summary>
	public const int UniqueSquareTemplatesCount = 162;

	/// <summary>
	/// Indicates the total number of Qiu's Deadly Pattern possible templates.
	/// </summary>
	public const int QiuDeadlyPatternTemplatesCount = 972;

	/// <summary>
	/// Indicates the total number of Unique Polygon (Heptagon) possible templates of size 3.
	/// </summary>
	public const int BdpTemplatesSize3Count = 14580;

	/// <summary>
	/// Indicates the total number of Unique Polygon (Octagon) possible templates of size 4.
	/// </summary>
	public const int BdpTemplatesSize4Count = 11664;


	/// <summary>
	/// <para>The names of all subsets by their sizes.</para>
	/// <para>
	/// For example, if you want to get the name of the size 3, the code will be
	/// <code><![CDATA[string name = TechniqueStrings.SubsetNames[3];]]></code>
	/// Here the digit <c>3</c> is the size you want to get.
	/// </para>
	/// </summary>
	public static readonly string[] SubsetNames =
	{
		string.Empty, "Single", "Pair", "Triple", "Quadruple",
		"Quintuple", "Sextuple", "Septuple"
	};

	/// <summary>
	/// The names of all fishes by their sizes.
	/// </summary>
	public static readonly string[] FishNames =
	{
		string.Empty, "Cyclopsfish", "X-Wing", "Swordfish", "Jellyfish",
		"Squirmbag", "Whale", "Leviathan"
	};

	/// <summary>
	/// <para>The names of all regular wings by their sizes.</para>
	/// <para>
	/// For example, if you want to get the name of the size 3, the code will be
	/// <code><![CDATA[string name = RegularWingNames[3];]]></code>
	/// Here the digit <c>3</c> is the size you want to get.
	/// </para>
	/// </summary>
	public static readonly string[] RegularWingNames =
	{
		string.Empty, string.Empty, string.Empty, string.Empty, "WXYZ-Wing", "VWXYZ-Wing",
		"UVWXYZ-Wing", "TUVWXYZ-Wing", "STUVWXYZ-Wing", "RSTUVWXYZ-Wing"
	};
}
