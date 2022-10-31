namespace OneOf;

/// <summary>
/// Provides extension methods on <see cref="OneOf"/> types.
/// </summary>
/// <seealso cref="OneOf"/>
public static class OneOfExtensions
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct<T1, T2>(this OneOf<T1, T2> @this, out bool isT0, out bool isT1)
		=> (isT0, isT1) = (@this.IsT0, @this.IsT1);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct<T1, T2, T3>(this OneOf<T1, T2, T3> @this, out bool isT0, out bool isT1, out bool isT2)
		=> (isT0, isT1, isT2) = (@this.IsT0, @this.IsT1, @this.IsT2);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct<T1, T2, T3, T4>(this OneOf<T1, T2, T3, T4> @this, out bool isT0, out bool isT1, out bool isT2, out bool isT3)
		=> (isT0, isT1, isT2, isT3) = (@this.IsT0, @this.IsT1, @this.IsT2, @this.IsT3);
}
