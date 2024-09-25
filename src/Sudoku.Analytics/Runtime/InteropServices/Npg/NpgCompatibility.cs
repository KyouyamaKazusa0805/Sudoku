namespace Sudoku.Runtime.InteropServices.Npg;

/// <summary>
/// Represents some methods that are used for get the details supported and defined
/// by another program called Number-Place Generator.
/// </summary>
public static class NpgCompatibility
{
	/// <summary>
	/// Gets difficulty point of technique.
	/// </summary>
	/// <param name="this">The technique.</param>
	/// <returns>The difficulty rating.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetDifficultyPoint(this Technique @this)
		=> typeof(Technique).GetField(@this.ToString())!.GetCustomAttribute<NpgAttribute>()!.Rating;
}
