namespace System.SourceGeneration;

/// <summary>
/// Represents an attribute that can tell source generators the type is a large structure,
/// meaning the source generators will always append <see langword="scoped ref readonly"/> modifiers onto target-typed parameters.
/// </summary>
/// <remarks>
/// Please note that the type can be also consumed by interfaces. Such cases mean the target structure types implemented is a large structure.
/// However, if the interface types having marked this attribute have been implemented by class types, it'll be meaningless.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Interface)]
public sealed class LargeStructureAttribute : Attribute;
