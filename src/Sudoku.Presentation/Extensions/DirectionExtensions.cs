namespace Sudoku.Presentation.Nodes.Shapes;

/// <summary>
/// Provides with extension methods on <see cref="Direction"/>.
/// </summary>
/// <seealso cref="Direction"/>
public static class DirectionExtensions
{
	/// <summary>
	/// Gets the rotating angle. The result value is described using degrees.
	/// </summary>
	/// <param name="this">The direction.</param>
	/// <returns>The rotating angle (degrees).</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the value is not defined or <see cref="Direction.None"/>.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetRotatingAngle(this Direction @this)
		=> @this switch
		{
			Direction.Up => 0,
			Direction.TopRight => 45,
			Direction.Right => 90,
			Direction.BottomRight => 135,
			Direction.Down => 180,
			Direction.BottomLeft => 225,
			Direction.Left => 270,
			Direction.TopLeft => 315,
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
