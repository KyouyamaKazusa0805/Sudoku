namespace System;

/// <summary>
/// Provides with a mechanism to allow the type <typeparamref name="TFixedVariable"/>
/// using <see langword="fixed"/> statements.
/// </summary>
/// <typeparam name="TSelf">The type of an instance that supports <see langword="fixed"/> statements.</typeparam>
/// <typeparam name="TFixedVariable">The type of the fixed variable.</typeparam>
/// <remarks>
/// If a type implements this interface type, we can use <see langword="fixed"/> statements on that type:
/// <code><![CDATA[
/// fixed (TFixedVariable* pBlockStart = fixable)
/// {
///     // ...
/// }
/// ]]></code>
/// </remarks>
public interface IFixable<[Self] TSelf, TFixedVariable> : IEnumerable<TFixedVariable> where TSelf : notnull, IFixable<TSelf, TFixedVariable>
{
	/// <summary>
	/// <para>Indicates the reference to the block that references to the inner fixed buffer or the block used.</para>
	/// <para>
	/// The value can be <see langword="null"/> reference if the target type does not use fixed buffer
	/// to bind with pinnable references. Therefore you should check for the nullability of the property
	/// using both methods <see cref="IsNullRef{T}(ref T)"/> and <see cref="AsRef{T}(in T)"/>:
	/// <code><![CDATA[
	/// scoped ref readonly var @ref = ref instance.BlockRef;
	/// if (Unsafe.IsNullRef(ref Unsafe.AsRef(in @ref)))
	/// {
	///     // Here '@ref' is null reference. If we use it ignoring it being null,
	///     // we'll get a NullReferenceException.
	/// }
	/// else
	/// {
	///     // Not null. You can use it here.
	/// }
	/// ]]></code>
	/// </para>
	/// </summary>
	/// <seealso cref="IsNullRef{T}(ref T)"/>
	/// <seealso cref="AsRef{T}(in T)"/>
	public abstract ref readonly TFixedVariable BlockReference { get; }


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='custom-fixed']/target[@name='method']"/>
	public abstract ref TFixedVariable GetPinnableReference();
}
