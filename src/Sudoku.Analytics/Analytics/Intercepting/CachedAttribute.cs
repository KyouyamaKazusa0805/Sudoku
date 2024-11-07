namespace Sudoku.Analytics.Intercepting;

/// <summary>
/// (<b>Interceptor-related attribute</b>)<br/>
/// Represents an attribute type that can be applied to a method,
/// making source generator produces a new method that copies the code from this,
/// with <see cref="Grid"/> properties replaced with corresponding properties in <see cref="MemoryCachedData"/>.
/// </summary>
/// <remarks>
/// <para>
/// Please note that all cached members should be named starting with two underscore characters, like "<c>__BivalueCells</c>".
/// Interceptor source generator will detect such name and replace them with property <see cref="BivalueCells"/>.
/// </para>
/// <para>In addition, this attribute doesn't support for a method that uses expression body.</para>
/// </remarks>
/// <seealso cref="Grid"/>
/// <seealso cref="MemoryCachedData"/>
/// <seealso cref="BivalueCells"/>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class CachedAttribute : Attribute;
