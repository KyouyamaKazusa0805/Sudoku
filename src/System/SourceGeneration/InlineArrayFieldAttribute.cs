namespace System.SourceGeneration;

/// <summary>
/// Represents an attribute type that will be used by source generator,
/// telling the target source generator that the type must contain a fixed-sized buffer type,
/// with specified length and a generated field to visit it.
/// </summary>
/// <typeparam name="T">The type of the fixed-sized buffer element.</typeparam>
/// <param name="fieldName">Indicates the target field name.</param>
/// <param name="length">Indicates the length of the fixed-sized buffer type.</param>
[AttributeUsage(AttributeTargets.Struct, Inherited = false)]
public sealed partial class InlineArrayFieldAttribute<T>([PrimaryCosntructorParameter] string fieldName, [PrimaryCosntructorParameter] int length) : Attribute;
