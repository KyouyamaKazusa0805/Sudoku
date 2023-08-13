namespace System.SourceGeneration;

/// <summary>
/// Represents an attribute type that tells source generator that the current method marked this attribute type should implement an interface
/// type specified as argument <paramref name="interfaceType"/>.
/// </summary>
/// <param name="interfaceType">
/// Indicates the interface type. This parameter may not be explicitly specified if the containing type just implements one interface.
/// In other words, this parameter is optional one. By default it keeps <see langword="null"/> value. If you explicitly specified it,
/// you must give an interface type.
/// </param>
/// <remarks>
/// This type can also be consumed by operators; however, the interface type should be always explicitly specified because a type
/// may contain many operators that implements multiple interfaces.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed partial class ExplicitInterfaceImplAttribute([DataMember] Type? interfaceType = null) : Attribute;
