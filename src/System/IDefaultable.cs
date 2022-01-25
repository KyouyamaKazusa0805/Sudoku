namespace System;

/// <summary>
/// Defines a type that holds a property with the default value of the type.
/// </summary>
/// <typeparam name="T">The type that contains the default instance.</typeparam>
public interface IDefaultable<[Self] T> where T : IDefaultable<T>
{
	/// <summary>
	/// <para>
	/// Indicates whether the current instance holds the default value that equals
	/// to the property <see cref="Default"/>.
	/// </para>
	/// <para>
	/// Generally, the implementation will be like <c>this == Default</c> or <c>Equals(Default)</c>.
	/// </para>
	/// </summary>
	/// <seealso cref="Default"/>
	bool IsDefault { get; }

	/// <summary>
	/// Indicates the default-value instance of this type.
	/// </summary>
	/// <remarks>
	/// Due to the design of the C# feature "<see langword="static abstract"/>s in interface",
	/// we suggest you use explicit implementation to implement this property, and makes the value
	/// referencing to the field that is named <c>Default</c>.
	/// Here is an example code to describe the above words:
	/// <code><![CDATA[
	/// // The static read-only field to introduce the default value.
	/// public static readonly T Default = default(T);
	/// 
	/// // An explicit implementation on the default-value property.
	/// static T IDefaultable<T>.Default => Default;
	/// ]]></code>
	/// </remarks>
	static abstract T Default { get; }
}
