namespace Sudoku.Measuring;

/// <summary>
/// Represents an attribute type that describes the current property can be used as a factor.
/// </summary>
/// <typeparam name="TFactor">The type of the factor.</typeparam>
/// <param name="resourceKey">The name of the property. The key points to a resource dictionary.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed partial class FactorPropertyAttribute<TFactor>([PrimaryConstructorParameter] string resourceKey) : Attribute;
