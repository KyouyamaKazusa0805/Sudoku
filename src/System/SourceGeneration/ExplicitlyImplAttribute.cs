namespace System.SourceGeneration;

/// <summary>
/// Represents an attribute type that tells source generator that the current method marked this attribute type should implement an interface
/// type specified as argument <paramref name="interfaceType"/>. This type can also be consumed by operators.
/// </summary>
/// <param name="interfaceType">
/// Indicates the interface type. The type should be specified via <see langword="typeof"/> expression.
/// If the target interface type is a generic type, you can just specify its open-typed reference.
/// For example, if you want to explicitly implement <see cref="IEquatable{T}.Equals(T)"/>,
/// just pass value <c><![CDATA[typeof(IEquatable<>)]]></c>.
/// </param>
/// <remarks><b>
/// Please note that if a type implements multiple same interfaces without type parameters
/// (e.g. <c><![CDATA[I<A>]]></c> and <c><![CDATA[I<B>]]></c>), the target source generator won't work.
/// </b></remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed partial class ExplicitInterfaceImplAttribute([Data] Type interfaceType) : Attribute;
