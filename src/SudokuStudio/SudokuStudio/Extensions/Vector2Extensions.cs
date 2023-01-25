namespace System.Numerics;

/// <summary>
/// Provides with extension methods on <see cref="Vector2"/>.
/// </summary>
/// <seealso cref="Vector2"/>
public static partial class Vector2Extensions
{
	[GeneratedDeconstruction]
	public static partial void Deconstruct(
		this Vector2 @this,
		[GeneratedDeconstructionArgument(nameof(Vector2.X))] out float width,
		[GeneratedDeconstructionArgument(nameof(Vector2.Y))] out float height
	);
}
