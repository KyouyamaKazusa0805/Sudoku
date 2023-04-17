namespace System.Drawing;

/// <summary>
/// 提供关于 <see cref="Color"/> 类型实例的扩展方法。
/// </summary>
/// <seealso cref="Color"/>
public static class ColorExtensions
{
	/// <summary>
	/// 将指定的 <see cref="Color"/> 实例转换为等价的 <see cref="ColorIdentifier"/> 实例。
	/// </summary>
	/// <param name="this"><see cref="Color"/> 实例。</param>
	/// <returns>等价的 <see cref="ColorIdentifier"/> 实例。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ColorIdentifier ToIdentifier(this Color @this)
	{
		_ = @this is { A: var a, R: var r, G: var g, B: var b };
		return new ColorColorIdentifier(a, r, g, b);
	}
}
