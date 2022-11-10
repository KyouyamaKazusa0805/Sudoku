namespace OneOf;

/// <summary>
/// Provides extension methods on <see cref="OneOf"/> types.
/// </summary>
/// <seealso cref="OneOf"/>
public static partial class OneOfExtensions
{
	[GeneratedDeconstruction]
	public static partial void Deconstruct<T1, T2>(this OneOf<T1, T2> @this, out bool isT0, out bool isT1);

	[GeneratedDeconstruction]
	public static partial void Deconstruct<T1, T2, T3>(this OneOf<T1, T2, T3> @this, out bool isT0, out bool isT1, out bool isT2);

	[GeneratedDeconstruction]
	public static partial void Deconstruct<T1, T2, T3, T4>(this OneOf<T1, T2, T3, T4> @this, out bool isT0, out bool isT1, out bool isT2, out bool isT3);
}
